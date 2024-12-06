using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectSpawner : MonoBehaviour
{
    public GameObject kabin; // 例Aのプレハブ
    public GameObject cap; // 例Bのプレハブ
    public GameObject[] randomObjects; // ランダムなオブジェクトリスト
    private List<GameObject> spawnedObjects = new List<GameObject>(); // 生成されたオブジェクトを追跡
    private HashSet<GameObject> usedObjects = new HashSet<GameObject>(); // 使用済みのオブジェクトを追跡

    public GameObject kabin2; // アニメーション付きの例Aプレハブ
    public GameObject cap2; // アニメーション付きの例Bプレハブ

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
        usedObjects.Clear(); // 新しいスポーンのたびに使用済みリストをリセット

        for (int i = 0; i < 4; i++)
        {
            GameObject randomObjectA = GetUnusedRandomObject();
            if (randomObjectA != null)
            {
                Vector3 spawnPositionA = randomObjectA.transform.position + new Vector3(0, 1, 0);
                GameObject newObjectA = Instantiate(kabin, spawnPositionA, Quaternion.identity);
                spawnedObjects.Add(newObjectA);
                usedObjects.Add(randomObjectA);
            }
        }

        GameObject randomObjectB = GetUnusedRandomObject();
        if (randomObjectB != null)
        {
            Vector3 spawnPositionB = randomObjectB.transform.position + new Vector3(0, 1, 0);
            GameObject newObjectB = Instantiate(cap, spawnPositionB, Quaternion.identity);
            spawnedObjects.Add(newObjectB);
            usedObjects.Add(randomObjectB);
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

            // アニメーション付きオブジェクトをスポーン
            StartCoroutine(SpawnAnimationAndDestroy(obj));

            // オブジェクトを破壊
            Destroy(obj);
        }
    }

    private IEnumerator SpawnAnimationAndDestroy(GameObject destroyedObject)
    {
        

        // アニメーション付きオブジェクトを生成
        GameObject animationObject = null;
        if (destroyedObject.CompareTag("kabin"))
        {
            animationObject = Instantiate(kabin2, destroyedObject.transform.position, Quaternion.identity);
        }
        else if (destroyedObject.CompareTag("cap"))
        {
            animationObject = Instantiate(cap2, destroyedObject.transform.position, Quaternion.identity);
        }

        // 1秒後にアニメーション付きオブジェクトを削除
        yield return new WaitForSeconds(1f);
        if (animationObject != null)
        {
            Destroy(animationObject);
        }
    }
}
