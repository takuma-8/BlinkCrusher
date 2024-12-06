using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectSpawner : MonoBehaviour
{
    public GameObject kabin; // ��A�̃v���n�u
    public GameObject cap; // ��B�̃v���n�u
    public GameObject[] randomObjects; // �����_���ȃI�u�W�F�N�g���X�g
    private List<GameObject> spawnedObjects = new List<GameObject>(); // �������ꂽ�I�u�W�F�N�g��ǐ�
    private HashSet<GameObject> usedObjects = new HashSet<GameObject>(); // �g�p�ς݂̃I�u�W�F�N�g��ǐ�

    public GameObject kabin2; // �A�j���[�V�����t���̗�A�v���n�u
    public GameObject cap2; // �A�j���[�V�����t���̗�B�v���n�u

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
        usedObjects.Clear(); // �V�����X�|�[���̂��тɎg�p�ς݃��X�g�����Z�b�g

        for (int i = 0; i < 4; i++)
        {
            GameObject randomObjectA = GetUnusedRandomObject();
            if (randomObjectA != null)
            {
                // randomObjectA�̈ʒu��x, z�����̂܂܂ɂ��āAy���W�������w�� (��: y = 3f)
                Vector3 spawnPositionA = new Vector3(randomObjectA.transform.position.x, 1f, randomObjectA.transform.position.z);
                GameObject newObjectA = Instantiate(kabin, spawnPositionA, Quaternion.identity);
                spawnedObjects.Add(newObjectA);
                usedObjects.Add(randomObjectA);
            }
        }

        GameObject randomObjectB = GetUnusedRandomObject();
        if (randomObjectB != null)
        {
            // randomObjectB�̈ʒu��x, z�����̂܂܂ɂ��āAy���W�������w�� (��: y = 5f)
            Vector3 spawnPositionB = new Vector3(randomObjectB.transform.position.x, 1f, randomObjectB.transform.position.z);
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
            // �A�j���[�V�����t���I�u�W�F�N�g���X�|�[��
            StartCoroutine(SpawnAnimationAndDestroy(obj));

            // �I�u�W�F�N�g��j��
            Destroy(obj);
        }
    }

    private IEnumerator SpawnAnimationAndDestroy(GameObject destroyedObject)
    {
        

        // �A�j���[�V�����t���I�u�W�F�N�g�𐶐�
        GameObject animationObject = null;
        if (destroyedObject.CompareTag("kabin"))
        {
            animationObject = Instantiate(kabin2, destroyedObject.transform.position, Quaternion.identity);
        }
        else if (destroyedObject.CompareTag("cap"))
        {
            animationObject = Instantiate(cap2, destroyedObject.transform.position, Quaternion.identity);
        }

        // 1�b��ɃA�j���[�V�����t���I�u�W�F�N�g���폜
        yield return new WaitForSeconds(1f);
        if (animationObject != null)
        {
            Destroy(animationObject);
            
        }
    }
}
