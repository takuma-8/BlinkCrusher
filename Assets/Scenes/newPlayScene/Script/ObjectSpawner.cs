using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectSpawner : MonoBehaviour
{
    public GameObject cap; // 通常のアイテムプレハブ (cap)
    public GameObject kabin; // 高得点アイテムプレハブ (kabin)
    public GameObject[] randomObjects; // ランダムなオブジェクトリスト
    private List<GameObject> spawnedObjects = new List<GameObject>(); // 生成されたオブジェクトを追跡
    private HashSet<GameObject> usedObjects = new HashSet<GameObject>(); // 使用済みのオブジェクトを追跡

    public GameObject cap2; // 通常アイテム破壊アニメーション (cap2)
    public GameObject kabin2; // 高得点アイテム破壊アニメーション (kabin2)

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

        // 高得点アイテムを固定位置に配置
        Vector3 specialObjectPosition = new Vector3(0f, 1.5f, 0f);
        GameObject specialObject = Instantiate(kabin, specialObjectPosition, Quaternion.identity);
        spawnedObjects.Add(specialObject);

        // 通常のアイテムをランダムに配置
        for (int i = 0; i < 4; i++) // 通常アイテム4つをスポーン
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

            Debug.Log($"Destroyed Object Name: {obj.name}, Tag: {obj.tag}"); // タグの確認

            // アニメーションを再生
            if (obj.CompareTag("kabin"))
            {
                Debug.Log("Playing kabin2 animation");
                StartCoroutine(SpawnAnimationAndDestroy(obj, kabin2)); // 高得点アイテム用アニメーション (kabin2)
            }
            else if (obj.CompareTag("cap"))
            {
                Debug.Log("Playing cap2 animation");
                StartCoroutine(SpawnAnimationAndDestroy(obj, cap2)); // 通常アイテム用アニメーション (cap2)
            }
            else
            {
                Debug.LogError($"Unknown object destroyed! Name: {obj.name}, Tag: {obj.tag}");
            }

            // オブジェクトを破壊
            Vector3 respawnPosition = obj.transform.position;
            string respawnType = obj.CompareTag("kabin") ? "kabin" : "cap";
            Destroy(obj);

            // 30秒後にオブジェクトを復活させる
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

        // アニメーション付きオブジェクトを生成
        GameObject animationObject = Instantiate(animationPrefab, destroyedObject.transform.position, Quaternion.identity);

        // アニメーションの長さを取得
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
