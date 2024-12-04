using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy1Manager : MonoBehaviour
{
    public GameObject enemy1Prefab;   // Enemy1��Prefab
    public float spawnInterval = 30f; // �G��������Ԋu�i�b�j

    private int enemy1Count = 1;      // ���݂�Enemy1�̐��i�ŏ���1�̏o���j

    private Vector3 spawnPosition = new Vector3(44.0f, 1.0f, -2.5f); // �Œ�̏o���ʒu

    private void Start()
    {
        // �ŏ��̓G�𐶐�
        SpawnEnemy1();

        // ���Ԋu�œG�𐶐�
        StartCoroutine(SpawnEnemy1Routine());
    }

    private void SpawnEnemy1()
    {
        // enemy1Count�̐������G�𐶐�
        Instantiate(enemy1Prefab, spawnPosition, Quaternion.identity);

        Debug.Log($"Spawned {enemy1Count} Enemy1 units at position {spawnPosition}.");
    }

    private IEnumerator SpawnEnemy1Routine()
    {
        while (true)
        {
            yield return new WaitForSeconds(spawnInterval);

            // �G��1�̑��₷
            enemy1Count++;
            SpawnEnemy1(); // 1�̂�������
        }
    }
}
