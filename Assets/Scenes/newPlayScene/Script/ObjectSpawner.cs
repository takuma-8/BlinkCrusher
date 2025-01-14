using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectSpawner : MonoBehaviour
{
    public GameObject kabin; // kabin�v���n�u
    public GameObject[] randomObjects; // �����_���ȃI�u�W�F�N�g���X�g
    private List<GameObject> spawnedObjects = new List<GameObject>(); // �������ꂽ�I�u�W�F�N�g��ǐ�
    private HashSet<GameObject> usedObjects = new HashSet<GameObject>(); // �g�p�ς݂̃I�u�W�F�N�g��ǐ�

    public GameObject kabin2; // �A�j���[�V�����t����kabin�v���n�u

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

        for (int i = 0; i < 4; i++)
        {
            GameObject randomObject = GetUnusedRandomObject();
            if (randomObject != null)
            {
                // randomObject�̈ʒu��x, z�����̂܂܂ɂ��āAy���W�������w�� (��: y = 1f)
                Vector3 spawnPosition = new Vector3(randomObject.transform.position.x, 1f, randomObject.transform.position.z);
                GameObject newObject = Instantiate(kabin, spawnPosition, Quaternion.identity);
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

            // �A�j���[�V�����t���I�u�W�F�N�g���X�|�[��
            StartCoroutine(SpawnAnimationAndDestroy(obj));

            // �I�u�W�F�N�g��j��
            Destroy(obj);
        }
    }

    private IEnumerator SpawnAnimationAndDestroy(GameObject destroyedObject)
    {
        // �A�j���[�V�����t���I�u�W�F�N�g�𐶐�
        GameObject animationObject = Instantiate(kabin2, destroyedObject.transform.position, Quaternion.identity);

        // 1�b��ɃA�j���[�V�����t���I�u�W�F�N�g���폜
        yield return new WaitForSeconds(1f);
        if (animationObject != null)
        {
            Destroy(animationObject);
        }
    }
}
