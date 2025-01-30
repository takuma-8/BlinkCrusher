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
    private PlayerSoundManager soundManager;

    private Animator hammerAnimator;

    private bool isFrozen = false;
    private CharacterController characterController;
    private Rigidbody playerRigidbody;
    private int range = 8;

    private bool isNearObject = false;  // キャッシュ用

    void Start()
    {
        soundManager = GetComponent<PlayerSoundManager>();
        if (soundManager == null)
        {
            Debug.LogError("PlayerSoundManager is not attached to the GameObject.");
        }

        objectSpawner = FindObjectOfType<ObjectSpawner>();

        // ハンマーのAnimatorを取得
        Transform hammerTransform = transform.Find("Hammer");
        if (hammerTransform != null)
        {
            hammerAnimator = hammerTransform.GetComponent<Animator>();
            hammerAnimator.updateMode = AnimatorUpdateMode.AnimatePhysics; // AnimatorのUpdateModeを変更
        }

        if (hammerAnimator == null)
        {
            Debug.LogError("Hammer Animator is not found. Make sure the child object has an Animator.");
        }
    }

    void Update()
    {
        // IsNearTargetObject() をキャッシュ
        isNearObject = IsNearTargetObject();

        if (Input.GetKeyDown(KeyCode.J) || Input.GetButtonDown("Fire1"))
        {
            if (!isNearObject) return;  // 近くにオブジェクトがなければ処理しない

            PlayHammerAnimation();
            StartCoroutine(DestroyAfterDelay(0.3f));  // 1秒後にオブジェクトを破壊
        }
    }

    // アニメーションの即時再生
    void PlayHammerAnimation()
    {
        if (hammerAnimator != null && hammerAnimator.runtimeAnimatorController != null)
        {
            hammerAnimator.Play("SwingHammer", 0, 0f);  // 0フレーム目から再生
        }
    }

    // オブジェクトが近くにあるかどうかを確認
    private bool IsNearTargetObject()
    {
        Vector3 frontPosition = transform.position + transform.forward * detectionRadius;
        Collider[] colliders = Physics.OverlapSphere(frontPosition, detectionRadius);

        foreach (Collider collider in colliders)
        {
            if (collider.CompareTag(kabin) || collider.CompareTag(cap))
            {
                return true;
            }
        }
        return false;
    }

    // アニメーションイベントで呼び出すメソッド
    public void DestroyObject()
    {
        Vector3 frontPosition = transform.position + transform.forward * detectionRadius;
        Collider[] colliders = Physics.OverlapSphere(frontPosition, detectionRadius);

        bool destroyed = false;

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
                    score += 1000;
                    soundManager.PlayBreakSound("kabin");
                }
                else if (collider.CompareTag(cap))
                {
                    score += 100;
                    soundManager.PlayBreakSound("cap");
                }

                Destroy(collider.gameObject);
                NotifyNearbyEnemies(frontPosition, 13);
                destroyed = true;
                break;
            }
        }

        if (!destroyed)
        {
            Debug.LogWarning("破壊対象のオブジェクトが見つかりませんでした。");
        }
    }

    public static int GetScore()
    {
        return score;
    }

    public static void ResetScore()
    {
        score = 0;
    }

    // 近くの敵に通知
    void NotifyNearbyEnemies(Vector3 position, float radius)
    {
        Collider[] enemies = Physics.OverlapSphere(position, radius);

        foreach (Collider enemyCollider in enemies)
        {
            EnemyController enemyController = enemyCollider.GetComponent<EnemyController>();
            if (enemyController != null)
            {
                enemyController.MoveToPositionAndWander(position, 5f, 2f);
            }
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(0, 1, 0, 0.5f);
        Vector3 frontPosition = transform.position + transform.forward * detectionRadius;
        Gizmos.DrawWireSphere(frontPosition, 13);
    }

    // 物理演算と同期した更新
    private void FixedUpdate()
    {
        isNearObject = IsNearTargetObject(); // FixedUpdateで物理演算と同期
    }

    // 破壊を遅延させるコルーチン
    private IEnumerator DestroyAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);  // 指定した遅延を待つ

        DestroyObject();  // 遅延後にオブジェクトを破壊
    }
}
