using UnityEngine;
using System.Collections;

public class DestroyObjectInFront : MonoBehaviour
{
    public float detectionRadius = 1.0f;
    public string kabin = "kabin";
    public string cap = "cap";
    private ObjectSpawner objectSpawner;
    public static int score = 0;
    public AudioClip sound1;
    public AudioClip sound2;
    AudioSource audioSource;

    public GameObject enemy2Prefab;
    private GameObject enemy2Instance;

    private bool isFrozen = false;
    private CharacterController characterController;
    private Rigidbody playerRigidbody;

    public Camera mainCamera; // 通常時のカメラ
    public Camera playerCamera; // プレイヤー動作停止中に使うカメラ
    private MonoBehaviour cameraControlScript; // カメラの制御スクリプト（例: FPSカメラコントローラー）
    private float range = 2.5f;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            Debug.LogError("AudioSource is not attached to the GameObject.");
        }
        objectSpawner = FindObjectOfType<ObjectSpawner>();
        characterController = GetComponent<CharacterController>();
        playerRigidbody = GetComponent<Rigidbody>();

        // カメラ制御スクリプトを取得 (仮にFPSカメラ用のスクリプトがあると仮定)
        cameraControlScript = mainCamera?.GetComponent<MonoBehaviour>();

        if (enemy2Prefab != null)
        {
            enemy2Prefab.SetActive(false);
        }

        if (mainCamera != null && playerCamera != null)
        {
            mainCamera.enabled = true;
            playerCamera.enabled = false;
        }
    }

    void Update()
    {
        if (isFrozen) return;

        if (Input.GetKeyDown(KeyCode.J) || Input.GetButtonDown("Fire1"))
        {
            DetectAndDestroy();
        }
    }

    void DetectAndDestroy()
    {
        Vector3 frontPosition = transform.position + transform.forward * detectionRadius;
        Collider[] colliders = Physics.OverlapSphere(frontPosition, detectionRadius);

        foreach (Collider collider in colliders)
        {
            if (collider.CompareTag(kabin) || collider.CompareTag(cap))
            {
                if (objectSpawner != null)
                {
                    objectSpawner.RemoveSpawnedObject(collider.gameObject);
                }

                if (collider.CompareTag(kabin))
                {
                    score += 100;
                    // 効果音
                    audioSource.PlayOneShot(sound1);
                }
                else if (collider.CompareTag(cap))
                {
                    score += 500;
                    audioSource.PlayOneShot(sound2);
                    StartCoroutine(SpawnEnemy2WithDelay());
                }

                Destroy(collider.gameObject);

                // 壊れた後に範囲内の敵を反応させる
                NotifyNearbyEnemies(frontPosition, range); // 半径2.5 (直径5) の範囲

                StartCoroutine(FreezePlayer(1.0f));
                break;
            }
        }
    }

    private IEnumerator FreezePlayer(float freezeDuration)
    {
        isFrozen = true;

        // PlayerController の操作を停止
        PlayerController playerController = GetComponent<PlayerController>();
        if (playerController != null)
        {
            playerController.LockPlayerActions();
        }

        // キャラクターコントローラーを無効化
        if (characterController != null)
        {
            characterController.enabled = false;
        }
        if (playerRigidbody != null)
        {
            playerRigidbody.isKinematic = true;
            playerRigidbody.velocity = Vector3.zero;
        }

        // カメラ制御スクリプトを無効化
        if (cameraControlScript != null)
        {
            cameraControlScript.enabled = false;
        }

        // カメラを切り替え
        if (mainCamera != null && playerCamera != null)
        {
            mainCamera.enabled = false;
            playerCamera.enabled = true;
        }

        Debug.Log("Player is frozen, camera switched and control disabled.");
        yield return new WaitForSeconds(freezeDuration);

        // カメラを元に戻す
        if (mainCamera != null && playerCamera != null)
        {
            mainCamera.enabled = true;
            playerCamera.enabled = false;
        }

        // カメラ制御スクリプトを再有効化
        if (cameraControlScript != null)
        {
            cameraControlScript.enabled = true;
        }

        // キャラクターコントローラーを再有効化
        if (characterController != null)
        {
            characterController.enabled = true;
        }
        if (playerRigidbody != null)
        {
            playerRigidbody.isKinematic = false;
        }

        // PlayerController の操作を再開
        if (playerController != null)
        {
            playerController.UnlockPlayerActions();
        }

        isFrozen = false;
        Debug.Log("Player can move again, camera restored and control re-enabled.");
    }

    //void OnDrawGizmosSelected()
    //{
    //    Gizmos.color = Color.red;
    //    Vector3 frontPosition = transform.position + transform.forward * detectionRadius;
    //    Gizmos.DrawWireSphere(frontPosition, detectionRadius);
    //}

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
        if (enemy2Instance != null)
        {
            Debug.Log("Enemy2 is already spawned.");
            return;
        }

        if (enemy2Prefab != null)
        {
            Vector3 spawnPosition = new Vector3(44.0f, 1.0f, -2.0f);
            enemy2Instance = Instantiate(enemy2Prefab, spawnPosition, Quaternion.identity);
            enemy2Instance.SetActive(true);

            var enemyController = enemy2Instance.GetComponent<EnemyController>();
            if (enemyController != null)
            {
                enemyController.enemyType = EnemyController.EnemyType.Enemy2;
            }
        }
        else
        {
            Debug.LogError("Enemy2 Prefab is not assigned!");
        }
    }

    private IEnumerator SpawnEnemy2WithDelay()
    {
        yield return new WaitForSeconds(2.0f);
        SpawnEnemy2();
    }

    void NotifyNearbyEnemies(Vector3 position, float radius)
    {
        Collider[] enemies = Physics.OverlapSphere(position, radius);

        foreach (Collider enemyCollider in enemies)
        {
            EnemyController enemyController = enemyCollider.GetComponent<EnemyController>();
            if (enemyController != null)
            {
                // 指定位置に移動し、その近辺をうろうろする
                enemyController.MoveToPositionAndWander(position, 5f, 2f);
            }
        }
    }


    void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(0, 1, 0, 0.5f); // 半透明の緑
        Vector3 frontPosition = transform.position + transform.forward * detectionRadius;
        Gizmos.DrawWireSphere(frontPosition, range); // 反応範囲を描画
    }
}
