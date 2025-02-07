using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectSpawner : MonoBehaviour
{
    public GameObject cap;
    public GameObject kabin;
    public GameObject[] randomObjects;
    private List<GameObject> spawnedObjects = new List<GameObject>();
    private List<Vector3> recentlyDestroyedPositions = new List<Vector3>();
    public GameObject cap2;
    public GameObject kabin2;

    private const float minimumDistance = 1.0f;
    private const float spawnBlockDuration = 10f;

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
        Vector3 specialObjectPosition = new Vector3(0f, 0.64f, 0f); // Set height to 0.64 for kabin
        GameObject specialObject = Instantiate(kabin, specialObjectPosition, Quaternion.identity);
        specialObject.tag = "kabin";
        spawnedObjects.Add(specialObject);

        for (int i = 0; i < 7; i++)
        {
            Vector3 spawnPosition = GetValidRandomPosition();
            GameObject newObject = Instantiate(cap, spawnPosition, Quaternion.identity);
            newObject.tag = "cap";
            spawnedObjects.Add(newObject);
        }
    }


    Vector3 GetValidRandomPosition()
    {
        Vector3 spawnPosition;
        int maxAttempts = 20;
        int attempts = 0;

        do
        {
            spawnPosition = GetRandomPosition();
            attempts++;
        } while ((IsPositionOccupied(spawnPosition) || IsPositionRecentlyDestroyed(spawnPosition)) && attempts < maxAttempts);

        return spawnPosition;
    }

    Vector3 GetRandomPosition()
    {
        if (randomObjects.Length > 0)
        {
            GameObject randomObj = randomObjects[Random.Range(0, randomObjects.Length)];
            return new Vector3(randomObj.transform.position.x, 1.5f, randomObj.transform.position.z);
        }
        else
        {
            return new Vector3(Random.Range(-5f, 5f), 1.5f, Random.Range(-5f, 5f));
        }
    }

    bool IsPositionOccupied(Vector3 position)
    {
        foreach (var obj in spawnedObjects)
        {
            if (Vector3.Distance(obj.transform.position, position) < minimumDistance)
            {
                return true;
            }
        }
        return false;
    }

    bool IsPositionRecentlyDestroyed(Vector3 position)
    {
        foreach (var pos in recentlyDestroyedPositions)
        {
            if (Vector3.Distance(pos, position) < minimumDistance)
            {
                return true;
            }
        }
        return false;
    }

    public void RemoveSpawnedObject(GameObject obj)
    {
        if (spawnedObjects.Contains(obj))
        {
            spawnedObjects.Remove(obj);
            recentlyDestroyedPositions.Add(obj.transform.position);
            StartCoroutine(RemoveDestroyedPositionAfterDelay(obj.transform.position, spawnBlockDuration));

            if (obj.CompareTag("kabin"))
            {
                StartCoroutine(SpawnAnimationAndDestroy(obj, kabin2));
                StartCoroutine(RespawnObjectAfterDelay(obj.transform.position, "kabin", 30f));
            }
            else if (obj.CompareTag("cap"))
            {
                StartCoroutine(SpawnAnimationAndDestroy(obj, cap2));
                StartCoroutine(RespawnObjectAfterDelay(GetValidRandomPosition(), "cap", 15f));
            }
            Destroy(obj);
        }
    }

    private IEnumerator SpawnAnimationAndDestroy(GameObject destroyedObject, GameObject animationPrefab)
    {
        GameObject animationObject = Instantiate(animationPrefab, destroyedObject.transform.position, Quaternion.identity);
        yield return new WaitForSeconds(3f);  // Wait for 5 seconds before destroying kabin2
        Destroy(animationObject);
    }

    private IEnumerator RespawnObjectAfterDelay(Vector3 position, string type, float delay)
    {
        yield return new WaitForSeconds(delay);
        GameObject prefabToSpawn = (type == "kabin") ? kabin : cap;
        GameObject newObject = Instantiate(prefabToSpawn, position, Quaternion.identity);
        newObject.tag = type;
        spawnedObjects.Add(newObject);
    }

    private IEnumerator RemoveDestroyedPositionAfterDelay(Vector3 position, float delay)
    {
        yield return new WaitForSeconds(delay);
        recentlyDestroyedPositions.Remove(position);
    }
}