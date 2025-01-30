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
    private PlayerController playerController;

    private Animator hammerAnimator;
    private bool isNearObject = false;

    void Start()
    {
        soundManager = GetComponent<PlayerSoundManager>();
        objectSpawner = FindObjectOfType<ObjectSpawner>();
        playerController = GetComponent<PlayerController>();

        Transform hammerTransform = transform.Find("Hammer");
        if (hammerTransform != null)
        {
            hammerAnimator = hammerTransform.GetComponent<Animator>();
            hammerAnimator.updateMode = AnimatorUpdateMode.AnimatePhysics;
        }
    }

    void Update()
    {
        isNearObject = IsNearTargetObject();

        if (Input.GetKeyDown(KeyCode.J) || Input.GetButtonDown("Fire1"))
        {
            if (!isNearObject) return;

            StartCoroutine(FreezePlayer());
            PlayHammerAnimation();
            StartCoroutine(DestroyAfterDelay(0.3f));
        }
    }

    private IEnumerator FreezePlayer()
    {
        if (playerController != null)
        {
            playerController.LockPlayerActions();
            yield return new WaitForSeconds(1f);
            playerController.UnlockPlayerActions();
        }
    }

    void PlayHammerAnimation()
    {
        if (hammerAnimator != null && hammerAnimator.runtimeAnimatorController != null)
        {
            hammerAnimator.Play("SwingHammer", 0, 0f);
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

    public void DestroyObject()
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
                    score += 1000;
                    soundManager.PlayBreakSound("kabin");
                }
                else if (collider.CompareTag(cap))
                {
                    score += 100;
                    soundManager.PlayBreakSound("cap");
                }

                Destroy(collider.gameObject);
                break;
            }
        }
    }

    private IEnumerator DestroyAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        DestroyObject();
    }

    // 🔥 **追加: スコアを取得するメソッド**
    public static int GetScore()
    {
        return score;
    }

    // 🔥 **追加: スコアをリセットするメソッド**
    public static void ResetScore()
    {
        score = 0;
    }
}
