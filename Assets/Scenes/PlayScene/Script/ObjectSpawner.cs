using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectSpawner : MonoBehaviour
{
    public GameObject exampleA; // ��A�̃v���n�u
    public GameObject exampleB; // ��B�̃v���n�u
    public GameObject[] randomObjects; // �����_���ȃI�u�W�F�N�g���X�g
    private List<GameObject> spawnedObjects = new List<GameObject>(); // �������ꂽ�I�u�W�F�N�g��ǐ�

    void Start()
    {
        SpawnObjects();
    }

    void Update()
    {
        // ���������I�u�W�F�N�g�����ׂĔj�����ꂽ�ꍇ�ɍĐ���
        if (spawnedObjects.Count == 0)
        {
            SpawnObjects();
        }
    }

    void SpawnObjects()
    {
        // ��A��4�����i�قȂ�I�u�W�F�N�g�̏�Ɂj
        for (int i = 0; i < 4; i++)
        {
            // �����_���ȃI�u�W�F�N�g��1�I��
            GameObject randomObjectA = randomObjects[Random.Range(0, randomObjects.Length)];
            Vector3 spawnPositionA = randomObjectA.transform.position + Vector3.up;
            GameObject newObjectA = Instantiate(exampleA, spawnPositionA, Quaternion.identity);
            spawnedObjects.Add(newObjectA);
        }

        // ��B��1�����i�ʂ̃����_���ȃI�u�W�F�N�g�̏�Ɂj
        GameObject randomObjectB = randomObjects[Random.Range(0, randomObjects.Length)];
        Vector3 spawnPositionB = randomObjectB.transform.position + Vector3.up;
        GameObject newObjectB = Instantiate(exampleB, spawnPositionB, Quaternion.identity);
        spawnedObjects.Add(newObjectB);
    }

    public void RemoveSpawnedObject(GameObject obj)
    {
        if (spawnedObjects.Contains(obj))
        {
            spawnedObjects.Remove(obj);
            Destroy(obj);

            // Check if all objects are destroyed and trigger spawn if necessary
            if (spawnedObjects.Count == 0)
            {
                SpawnObjects();
            }
        }
    }
}
