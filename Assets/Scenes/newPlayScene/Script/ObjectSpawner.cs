using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectSpawner : MonoBehaviour
{
    public GameObject kabin; // kabinプレハブ
    public GameObject[] randomObjects; // ランダムなオブジェクトリスト
    private List<GameObject> spawnedObjects = new List<GameObject>(); // 生成されたオブジェクトを追跡
    private HashSet<GameObject> usedObjects = new HashSet<GameObject>(); // 使用済みのオブジェクトを追跡

    public GameObject kabin2; // アニメーション付きのkabinプレハブ

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
            GameObject randomObject = GetUnusedRandomObject();
            if (randomObject != null)
            {
                // randomObjectの位置のx, zをそのままにして、y座標だけを指定 (例: y = 1f)
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

            // アニメーション付きオブジェクトをスポーン
            StartCoroutine(SpawnAnimationAndDestroy(obj));

            // オブジェクトを破壊
            Destroy(obj);
        }
    }

    private IEnumerator SpawnAnimationAndDestroy(GameObject destroyedObject)
    {
        // アニメーション付きオブジェクトを生成
        GameObject animationObject = Instantiate(kabin2, destroyedObject.transform.position, Quaternion.identity);

        // 1秒後にアニメーション付きオブジェクトを削除
        yield return new WaitForSeconds(1f);
        if (animationObject != null)
        {
            Destroy(animationObject);
        }
    }
}
