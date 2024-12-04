using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy1Manager : MonoBehaviour
{
    public GameObject enemy1Prefab;   // Enemy1‚ÌPrefab
    public float spawnInterval = 30f; // “G‚ª‘‚¦‚éŠÔŠui•bj

    private int enemy1Count = 1;      // Œ»İ‚ÌEnemy1‚Ì”iÅ‰‚É1‘ÌoŒ»j

    private Vector3 spawnPosition = new Vector3(44.0f, 1.0f, -2.5f); // ŒÅ’è‚ÌoŒ»ˆÊ’u

    private void Start()
    {
        // Å‰‚Ì“G‚ğ¶¬
        SpawnEnemy1();

        // ˆê’èŠÔŠu‚Å“G‚ğ¶¬
        StartCoroutine(SpawnEnemy1Routine());
    }

    private void SpawnEnemy1()
    {
        // enemy1Count‚Ì”‚¾‚¯“G‚ğ¶¬
        Instantiate(enemy1Prefab, spawnPosition, Quaternion.identity);

        Debug.Log($"Spawned {enemy1Count} Enemy1 units at position {spawnPosition}.");
    }

    private IEnumerator SpawnEnemy1Routine()
    {
        while (true)
        {
            yield return new WaitForSeconds(spawnInterval);

            // “G‚ğ1‘Ì‘‚â‚·
            enemy1Count++;
            SpawnEnemy1(); // 1‘Ì‚¾‚¯¶¬
        }
    }
}
