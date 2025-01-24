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
    public AudioClip sound1;
    public AudioClip sound2;
    AudioSource audioSource;

    private bool isFrozen = false;
    private CharacterController characterController;
    private Rigidbody playerRigidbody;
    private int range = 8;
   

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
        PlayerController playerController = GetComponent<PlayerController>();

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
                    score += 1000;
                    audioSource.PlayOneShot(sound2);
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
