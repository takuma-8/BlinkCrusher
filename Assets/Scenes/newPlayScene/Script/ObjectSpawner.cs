using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectSpawner : MonoBehaviour
{
    public GameObject cap; // 通常のアイテムプレハブ (cap)
    public GameObject kabin; // 高得点アイテムプレハブ (kabin)
    public GameObject[] randomObjects; // ランダムなオブジェクトリスト
    private List<GameObject> spawnedObjects = new List<GameObject>(); // 生成されたオブジェクトを追跡

    public GameObject cap2; // 通常アイテム破壊アニメーション (cap2)
    public GameObject kabin2; // 高得点アイテム破壊アニメーション (kabin2)

    private const float minimumDistance = 1.0f; // 重複防止の最小距離

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
        // 高得点アイテムを固定位置に配置
        Vector3 specialObjectPosition = new Vector3(0f, 1.5f, 0f);
        GameObject specialObject = Instantiate(kabin, specialObjectPosition, Quaternion.identity);
        specialObject.tag = "kabin";
        spawnedObjects.Add(specialObject);

        // 通常のアイテムをランダムに配置
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
        int maxAttempts = 10; // 位置を再試行する最大回数
        int attempts = 0;

        do
        {
            // ランダムな位置を生成
            if (randomObjects.Length > 0)
            {
                GameObject randomObj = randomObjects[Random.Range(0, randomObjects.Length)];
                spawnPosition = new Vector3(randomObj.transform.position.x, 1.5f, randomObj.transform.position.z);
            }
            else
            {
                spawnPosition = new Vector3(Random.Range(-5f, 5f), 1.5f, Random.Range(-5f, 5f)); // フォールバックとしてランダム位置
            }

            attempts++;

            // 最小距離を満たしていればループを抜ける
        } while (IsPositionOccupied(spawnPosition) && attempts < maxAttempts);

        return spawnPosition;
    }

    bool IsPositionOccupied(Vector3 position)
    {
        foreach (var obj in spawnedObjects)
        {
            if (Vector3.Distance(obj.transform.position, position) < minimumDistance)
            {
                return true; // 他のオブジェクトと近すぎる
            }
        }
        return false; // 十分離れている
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
                StartCoroutine(RespawnObjectAfterDelay(GetRandomPosition(), "cap", 15f)); // ランダム位置で再生成
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
