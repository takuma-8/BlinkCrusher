using System.Collections;
using UnityEngine;

public class DestroyObjectInFront : MonoBehaviour
{
    public float detectionRadius = 1.0f;  // ���o���锼�a
    public string targetTag1 = "Target1"; // �^�[�Q�b�g1�̃^�O
    public string targetTag2 = "Target2"; // �^�[�Q�b�g2�̃^�O
    private ObjectSpawner objectSpawner;   // ObjectSpawner�̎Q��
    public static int score = 0;          // ���L�X�R�A�ϐ�

    public GameObject enemy2Prefab;        // Enemy2��Prefab
    private GameObject enemy2Instance;     // �������ꂽEnemy2�̃C���X�^���X

    void Start()
    {
        // ObjectSpawner�̃C���X�^���X���擾
        objectSpawner = FindObjectOfType<ObjectSpawner>();

        // Enemy2Prefab���ŏ��͔�A�N�e�B�u�ɂ��Ă���
        if (enemy2Prefab != null)
        {
            enemy2Prefab.SetActive(false);
        }
    }

    void Update()
    {
        // �X�y�[�X�L�[�܂��̓R���g���[���[B�{�^���������ꂽ�Ƃ�
        if (Input.GetKeyDown(KeyCode.J) || Input.GetButtonDown("Fire1"))
        {
            DetectAndDestroy();
        }
    }

    void DetectAndDestroy()
    {
        // �v���C���[�̐���1���[�g����̈ʒu���v�Z
        Vector3 frontPosition = transform.position + transform.forward * detectionRadius;

        // OverlapSphere�Ŏw��͈͓��̃R���C�_�[���擾
        Collider[] colliders = Physics.OverlapSphere(frontPosition, detectionRadius);

        foreach (Collider collider in colliders)
        {
            // �^�O��Target1�܂���Target2�̏ꍇ�ɔj��
            if (collider.CompareTag(targetTag1) || collider.CompareTag(targetTag2))
            {
                // ObjectSpawner�ɒʒm���ă��X�g����폜
                if (objectSpawner != null)
                {
                    objectSpawner.RemoveSpawnedObject(collider.gameObject);
                }

                // �^�O�ɉ����ăX�R�A�����Z
                if (collider.CompareTag(targetTag1))
                {
                    score += 100;  // Target1�ɂ�100�|�C���g
                }
                else if (collider.CompareTag(targetTag2))
                {
                    score += 500;   // Target2�ɂ�500�|�C���g
                    StartCoroutine(SpawnEnemy2WithDelay());
                }

                // �I�u�W�F�N�g��j��
                Destroy(collider.gameObject);
                break;  // 1�����j�󂷂�
            }
        }
    }


    // Gizmos�Ō��o�͈͂�\��
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Vector3 frontPosition = transform.position + transform.forward * detectionRadius;
        Gizmos.DrawWireSphere(frontPosition, detectionRadius);
    }

    // ���݂̃X�R�A���擾���邽�߂̐ÓI���\�b�h
    public static int GetScore()
    {
        return score;
    }

    public static void ResetScore()
    {
        score = 0;
    }

    void SpawnEnemy2()
{
    // �������ł�Enemy2���o�����Ă�����A�����������Ȃ�
    if (enemy2Instance != null)
    {
        Debug.Log("Enemy2 is already spawned.");
        return;  // ���łɏo�����Ă����牽�����Ȃ�
    }

    // Enemy2���w��̈ʒu�ɐ���
    if (enemy2Prefab != null)
    {
        Vector3 spawnPosition = new Vector3(44.0f, 1.0f, -2.0f);  // �����ʒu (1.0, 1.0, 16.0)
        enemy2Instance = Instantiate(enemy2Prefab, spawnPosition, Quaternion.identity);
        enemy2Instance.SetActive(true);  // �\�������悤�ɂ���

        // Debug���O
        Debug.Log($"Enemy2 spawned at {spawnPosition}");

        // Enemy2��EnemyController�ɐݒ�
        var enemyController = enemy2Instance.GetComponent<EnemyController>();
        if (enemyController != null)
        {
            enemyController.enemyType = EnemyController.EnemyType.Enemy2;
            Debug.Log("Enemy2 initialized with lifetime.");
        }
        else
        {
            Debug.LogError("Enemy2 prefab is missing EnemyController!");
        }
    }
    else
    {
        Debug.LogError("Enemy2 Prefab is not assigned!");
    }
}

   /*
    // Enemy2���o�������郁�\�b�h
    void SpawnEnemy2()
    {
        // �������ł�Enemy2���o�����Ă�����A�����������Ȃ�
        //if (enemy2Instance != null)
        //{
        //    Debug.Log("Enemy2 is already spawned.");
        //    return;  // ���łɏo�����Ă����牽�����Ȃ�
        //}

        // Enemy2���w��̈ʒu�ɐ���
        if (enemy2Prefab != null)
        {
            Vector3 spawnPosition = new Vector3(44.0f, 1.0f, -2.0f);  // �����ʒu (1.0, 1.0, 16.0)
            enemy2Instance = Instantiate(enemy2Prefab, spawnPosition, Quaternion.identity);
            enemy2Instance.SetActive(true);  // �\�������悤�ɂ���

            // Debug���O
            Debug.Log($"Enemy2 spawned at {spawnPosition}");

            // Enemy2��EnemyController�ɐݒ�
            var enemyController = enemy2Instance.GetComponent<EnemyController>();
            if (enemyController != null)
            {
                enemyController.enemyType = EnemyController.EnemyType.Enemy2;
                Debug.Log("Enemy2 initialized with lifetime.");
            }
            else
            {
                Debug.LogError("Enemy2 prefab is missing EnemyController!");
            }
        }
        else
        {
            Debug.LogError("Enemy2 Prefab is not assigned!");
        }
    }
   */
    private IEnumerator SpawnEnemy2WithDelay()
    {
        yield return new WaitForSeconds(2.0f);
        Debug.Log("��b�o���܂��� Enemy2 ���o�����܂�");
        SpawnEnemy2();
    }
    
}
