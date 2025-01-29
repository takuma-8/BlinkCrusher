using UnityEngine;
using System.Collections;
[System.Serializable]
public class DestroyObjectInFront : MonoBehaviour
{
    public float detectionRadius = 1.0f;
    public string kabin = "kabin";
    public string cap = "cap";
    private ObjectSpawner objectSpawner;
    public static int score = 0;
    private SoundManager soundManager;

    private bool isFrozen = false;
    private CharacterController characterController;
    private Rigidbody playerRigidbody;
    private int range = 8;

    void Start()
    {
        soundManager = GetComponent<SoundManager>();
        if (soundManager == null)
        {
            Debug.LogError("SoundManager is not attached to the GameObject.");
        }

        objectSpawner = FindObjectOfType<ObjectSpawner>();
        characterController = GetComponent<CharacterController>();
        playerRigidbody = GetComponent<Rigidbody>();
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
                    soundManager.PlayBreakSound("kabin");  // 花瓶を壊す音
                }
                else if (collider.CompareTag(cap))
                {
                    score += 1000;
                    soundManager.PlayBreakSound("cap");  // キャップを壊す音
                }

                Destroy(collider.gameObject);
                NotifyNearbyEnemies(frontPosition, range);

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

       

        Debug.Log("Player is frozen, camera switched and control disabled.");
        yield return new WaitForSeconds(freezeDuration);

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
