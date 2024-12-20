using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy1Manager : MonoBehaviour
{
    public GameObject enemy1Prefab;   // Enemy1のPrefab
    public float spawnInterval = 30f; // 敵が増える間隔（秒）

    private int enemy1Count = 1;      // 現在のEnemy1の数（最初に1体出現）

    private Vector3 spawnPosition = new Vector3(44.0f, 1.0f, -2.5f); // 固定の出現位置

    private void Start()
    {
        // 最初の敵を生成
        SpawnEnemy1();

        // 一定間隔で敵を生成
        StartCoroutine(SpawnEnemy1Routine());
    }

    private void SpawnEnemy1()
    {
        // enemy1Countの数だけ敵を生成
        Instantiate(enemy1Prefab, spawnPosition, Quaternion.identity);

        Debug.Log($"Spawned {enemy1Count} Enemy1 units at position {spawnPosition}.");
    }

    private IEnumerator SpawnEnemy1Routine()
    {
        while (true)
        {
            yield return new WaitForSeconds(spawnInterval);

            // 敵を1体増やす
            enemy1Count++;
            SpawnEnemy1(); // 1体だけ生成
        }
    }
}
