using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectSpawner : MonoBehaviour
{
    public GameObject exampleA; // 例Aのプレハブ
    public GameObject exampleB; // 例Bのプレハブ
    public GameObject[] randomObjects; // ランダムなオブジェクトリスト
    private List<GameObject> spawnedObjects = new List<GameObject>(); // 生成されたオブジェクトを追跡

    void Start()
    {
        SpawnObjects();
    }

    void Update()
    {
        // 生成したオブジェクトがすべて破棄された場合に再生成
        if (spawnedObjects.Count == 0)
        {
            SpawnObjects();
        }
    }

    void SpawnObjects()
    {
        // 例Aを4つ生成（異なるオブジェクトの上に）
        for (int i = 0; i < 4; i++)
        {
            // ランダムなオブジェクトを1つ選択
            GameObject randomObjectA = randomObjects[Random.Range(0, randomObjects.Length)];
            Vector3 spawnPositionA = randomObjectA.transform.position + Vector3.up;
            GameObject newObjectA = Instantiate(exampleA, spawnPositionA, Quaternion.identity);
            spawnedObjects.Add(newObjectA);
        }

        // 例Bを1つ生成（別のランダムなオブジェクトの上に）
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
