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

    private Animator hammerAnimator;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            Debug.LogError("AudioSource is not attached to the GameObject.");
        }

        objectSpawner = FindObjectOfType<ObjectSpawner>();

        // ハンマーのAnimatorを取得
        Transform hammerTransform = transform.Find("Hammer");
        if (hammerTransform != null)
        {
            hammerAnimator = hammerTransform.GetComponent<Animator>();
        }

        if (hammerAnimator == null)
        {
            Debug.LogError("Hammer Animator is not found. Make sure the child object has an Animator.");
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.J) || Input.GetButtonDown("Fire1"))
        {
            if (!IsNearTargetObject()) return; // cap や kabin が近くになければ処理しない

            PlayHammerAnimation(); // ハンマーアニメーション再生
            StartCoroutine(DestroyAfterDelay(0.3f)); // 1秒後にオブジェクトを破壊
        }
    }

    void PlayHammerAnimation()
    {
        if (hammerAnimator != null)
        {
            hammerAnimator.SetTrigger("SwingHammer");
        }
    }

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

    private IEnumerator DestroyAfterDelay(float delay)
    {
        float elapsedTime = 0f; // 経過時間

        while (elapsedTime < delay)  // 指定した時間が経過するまで
        {
            elapsedTime += Time.deltaTime;  // 経過時間を加算
            yield return null;  // 次のフレームまで待機
        }

        // 経過時間後に破壊処理を行う
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

                // 破壊されるオブジェクトが判定されたら処理
                if (collider.CompareTag(kabin))
                {
                    score += 1000;
                    audioSource.PlayOneShot(sound1);
                }
                else if (collider.CompareTag(cap))
                {
                    score += 100;
                    audioSource.PlayOneShot(sound2);
                }

                // オブジェクトの破壊
                Destroy(collider.gameObject);

                // 周囲の敵に通知
                NotifyNearbyEnemies(frontPosition, 13);

                destroyed = true;
                break; // 最初に見つかったオブジェクトだけを破壊
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
}
