using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectSpawner : MonoBehaviour
{
    public GameObject cap; // �ʏ�̃A�C�e���v���n�u (cap)
    public GameObject kabin; // �����_�A�C�e���v���n�u (kabin)
    public GameObject[] randomObjects; // �����_���ȃI�u�W�F�N�g���X�g
    private List<GameObject> spawnedObjects = new List<GameObject>(); // �������ꂽ�I�u�W�F�N�g��ǐ�
    private HashSet<GameObject> usedObjects = new HashSet<GameObject>(); // �g�p�ς݂̃I�u�W�F�N�g��ǐ�

    public GameObject cap2; // �ʏ�A�C�e���j��A�j���[�V���� (cap2)
    public GameObject kabin2; // �����_�A�C�e���j��A�j���[�V���� (kabin2)

    void Start()
    {
        SpawnObjects();
    }

    void Update()
    {
        if (spawnedObjects.Count == 0)
        {
            SpawnObjects();
        }
    }

    void SpawnObjects()
    {
        usedObjects.Clear(); // �V�����X�|�[���̂��тɎg�p�ς݃��X�g�����Z�b�g

        // �����_�A�C�e�����Œ�ʒu�ɔz�u
        Vector3 specialObjectPosition = new Vector3(0f, 1.5f, 0f);
        GameObject specialObject = Instantiate(kabin, specialObjectPosition, Quaternion.identity);
        spawnedObjects.Add(specialObject);

        // �ʏ�̃A�C�e���������_���ɔz�u
        for (int i = 0; i < 4; i++) // �ʏ�A�C�e��4���X�|�[��
        {
            GameObject randomObject = GetUnusedRandomObject();
            if (randomObject != null)
            {
                Vector3 spawnPosition = new Vector3(randomObject.transform.position.x, 1.5f, randomObject.transform.position.z);
                GameObject newObject = Instantiate(cap, spawnPosition, Quaternion.identity);
                spawnedObjects.Add(newObject);
                usedObjects.Add(randomObject);
            }
        }
    }

    GameObject GetUnusedRandomObject()
    {
        List<GameObject> unusedObjects = new List<GameObject>();
        foreach (GameObject obj in randomObjects)
        {
            if (!usedObjects.Contains(obj))
            {
                unusedObjects.Add(obj);
            }
        }

        if (unusedObjects.Count > 0)
        {
            return unusedObjects[Random.Range(0, unusedObjects.Count)];
        }

        return null;
    }

    public void RemoveSpawnedObject(GameObject obj)
    {
        if (spawnedObjects.Contains(obj))
        {
            spawnedObjects.Remove(obj);

            Debug.Log($"Destroyed Object Name: {obj.name}, Tag: {obj.tag}"); // �^�O�̊m�F

            // �A�j���[�V�������Đ�
            if (obj.CompareTag("kabin"))
            {
                Debug.Log("Playing kabin2 animation");
                StartCoroutine(SpawnAnimationAndDestroy(obj, kabin2)); // �����_�A�C�e���p�A�j���[�V���� (kabin2)
            }
            else if (obj.CompareTag("cap"))
            {
                Debug.Log("Playing cap2 animation");
                StartCoroutine(SpawnAnimationAndDestroy(obj, cap2)); // �ʏ�A�C�e���p�A�j���[�V���� (cap2)
            }
            else
            {
                Debug.LogError($"Unknown object destroyed! Name: {obj.name}, Tag: {obj.tag}");
            }

            // �I�u�W�F�N�g��j��
            Vector3 respawnPosition = obj.transform.position;
            string respawnType = obj.CompareTag("kabin") ? "kabin" : "cap";
            Destroy(obj);

            // 30�b��ɃI�u�W�F�N�g�𕜊�������
            StartCoroutine(RespawnObjectAfterDelay(respawnPosition, respawnType, 30f));
        }
    }

    private IEnumerator SpawnAnimationAndDestroy(GameObject destroyedObject, GameObject animationPrefab)
    {
        if (animationPrefab == null)
        {
            Debug.LogError("Animation prefab is NULL!");
            yield break;
        }

        Debug.Log($"Instantiating animation: {animationPrefab.name} at {destroyedObject.transform.position}");

        // �A�j���[�V�����t���I�u�W�F�N�g�𐶐�
        GameObject animationObject = Instantiate(animationPrefab, destroyedObject.transform.position, Quaternion.identity);

        // �A�j���[�V�����̒������擾
        Animator animator = animationObject.GetComponent<Animator>();
        if (animator != null)
        {
            AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);
            float animationLength = stateInfo.length;
            Debug.Log($"Animation {animationPrefab.name} length: {animationLength} seconds");

            yield return new WaitForSeconds(animationLength);
        }
        else
        {
            Debug.LogWarning("Animator not found on animationPrefab. Defaulting to 1 second.");
            yield return new WaitForSeconds(1f);
        }

        if (animationObject != null)
        {
            Destroy(animationObject);
        }
    }


    private IEnumerator RespawnObjectAfterDelay(Vector3 position, string type, float delay)
    {
        Debug.Log($"Respawning {type} at {position} after {delay} seconds...");
        yield return new WaitForSeconds(delay);

        GameObject prefabToSpawn = (type == "kabin") ? kabin : cap;
        GameObject newObject = Instantiate(prefabToSpawn, position, Quaternion.identity);

        spawnedObjects.Add(newObject);
        Debug.Log($"{type} respawned at {position}!");
    }
}
