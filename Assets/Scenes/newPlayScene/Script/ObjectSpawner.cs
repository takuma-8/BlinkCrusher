using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectSpawner : MonoBehaviour
{
    public GameObject cap; // �ʏ�̃A�C�e���v���n�u (cap)
    public GameObject kabin; // �����_�A�C�e���v���n�u (kabin)
    public GameObject[] randomObjects; // �����_���ȃI�u�W�F�N�g���X�g
    private List<GameObject> spawnedObjects = new List<GameObject>(); // �������ꂽ�I�u�W�F�N�g��ǐ�

    public GameObject cap2; // �ʏ�A�C�e���j��A�j���[�V���� (cap2)
    public GameObject kabin2; // �����_�A�C�e���j��A�j���[�V���� (kabin2)

    private const float minimumDistance = 1.0f; // �d���h�~�̍ŏ�����

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
        // �����_�A�C�e�����Œ�ʒu�ɔz�u
        Vector3 specialObjectPosition = new Vector3(0f, 1.5f, 0f);
        GameObject specialObject = Instantiate(kabin, specialObjectPosition, Quaternion.identity);
        specialObject.tag = "kabin";
        spawnedObjects.Add(specialObject);

        // �ʏ�̃A�C�e���������_���ɔz�u
        for (int i = 0; i < 5; i++)
        {
            Vector3 spawnPosition = GetRandomPosition();
            GameObject newObject = Instantiate(cap, spawnPosition, Quaternion.identity);
            newObject.tag = "cap";
            spawnedObjects.Add(newObject);
        }
    }

    Vector3 GetRandomPosition()
    {
        Vector3 spawnPosition;
        int maxAttempts = 10; // �ʒu���Ď��s����ő��
        int attempts = 0;

        do
        {
            // �����_���Ȉʒu�𐶐�
            if (randomObjects.Length > 0)
            {
                GameObject randomObj = randomObjects[Random.Range(0, randomObjects.Length)];
                spawnPosition = new Vector3(randomObj.transform.position.x, 1.5f, randomObj.transform.position.z);
            }
            else
            {
                spawnPosition = new Vector3(Random.Range(-5f, 5f), 1.5f, Random.Range(-5f, 5f)); // �t�H�[���o�b�N�Ƃ��ă����_���ʒu
            }

            attempts++;

            // �ŏ������𖞂����Ă���΃��[�v�𔲂���
        } while (IsPositionOccupied(spawnPosition) && attempts < maxAttempts);

        return spawnPosition;
    }

    bool IsPositionOccupied(Vector3 position)
    {
        foreach (var obj in spawnedObjects)
        {
            if (Vector3.Distance(obj.transform.position, position) < minimumDistance)
            {
                return true; // ���̃I�u�W�F�N�g�Ƌ߂�����
            }
        }
        return false; // �\������Ă���
    }

    public void RemoveSpawnedObject(GameObject obj)
    {
        if (spawnedObjects.Contains(obj))
        {
            spawnedObjects.Remove(obj);
            Debug.Log($"Destroyed Object Name: {obj.name}, Tag: {obj.tag}");

            if (obj.CompareTag("kabin"))
            {
                StartCoroutine(SpawnAnimationAndDestroy(obj, kabin2));
                StartCoroutine(RespawnObjectAfterDelay(obj.transform.position, "kabin", 30f));
            }
            else if (obj.CompareTag("cap"))
            {
                StartCoroutine(SpawnAnimationAndDestroy(obj, cap2));
                StartCoroutine(RespawnObjectAfterDelay(GetRandomPosition(), "cap", 15f)); // �����_���ʒu�ōĐ���
            }
            Destroy(obj);
        }
    }

    private IEnumerator SpawnAnimationAndDestroy(GameObject destroyedObject, GameObject animationPrefab)
    {
        GameObject animationObject = Instantiate(animationPrefab, destroyedObject.transform.position, Quaternion.identity);
        yield return new WaitForSeconds(1f);
        Destroy(animationObject);
    }

    private IEnumerator RespawnObjectAfterDelay(Vector3 position, string type, float delay)
    {
        Debug.Log($"Respawning {type} at {position} after {delay} seconds...");
        yield return new WaitForSeconds(delay);

        GameObject prefabToSpawn = (type == "kabin") ? kabin : cap;
        GameObject newObject = Instantiate(prefabToSpawn, position, Quaternion.identity);
        newObject.tag = type;
        spawnedObjects.Add(newObject);
        Debug.Log($"{type} respawned at {position}!");
    }
}
