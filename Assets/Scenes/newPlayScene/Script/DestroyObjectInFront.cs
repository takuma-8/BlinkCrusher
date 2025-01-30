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

    private bool isNearObject = false;  // �L���b�V���p

    void Start()
    {
        soundManager = GetComponent<PlayerSoundManager>();
        if (soundManager == null)
        {
            Debug.LogError("PlayerSoundManager is not attached to the GameObject.");
        }

        objectSpawner = FindObjectOfType<ObjectSpawner>();

        // �n���}�[��Animator���擾
        Transform hammerTransform = transform.Find("Hammer");
        if (hammerTransform != null)
        {
            hammerAnimator = hammerTransform.GetComponent<Animator>();
            hammerAnimator.updateMode = AnimatorUpdateMode.AnimatePhysics; // Animator��UpdateMode��ύX
        }

        if (hammerAnimator == null)
        {
            Debug.LogError("Hammer Animator is not found. Make sure the child object has an Animator.");
        }
    }

    void Update()
    {
        // IsNearTargetObject() ���L���b�V��
        isNearObject = IsNearTargetObject();

        if (Input.GetKeyDown(KeyCode.J) || Input.GetButtonDown("Fire1"))
        {
            if (!isNearObject) return;  // �߂��ɃI�u�W�F�N�g���Ȃ���Ώ������Ȃ�

            PlayHammerAnimation();
            StartCoroutine(DestroyAfterDelay(0.3f));  // 1�b��ɃI�u�W�F�N�g��j��
        }
    }

    // �A�j���[�V�����̑����Đ�
    void PlayHammerAnimation()
    {
        if (hammerAnimator != null && hammerAnimator.runtimeAnimatorController != null)
        {
            hammerAnimator.Play("SwingHammer", 0, 0f);  // 0�t���[���ڂ���Đ�
        }
    }

    // �I�u�W�F�N�g���߂��ɂ��邩�ǂ������m�F
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

    // �A�j���[�V�����C�x���g�ŌĂяo�����\�b�h
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
            Debug.LogWarning("�j��Ώۂ̃I�u�W�F�N�g��������܂���ł����B");
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

    // �߂��̓G�ɒʒm
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

    // �������Z�Ɠ��������X�V
    private void FixedUpdate()
    {
        isNearObject = IsNearTargetObject(); // FixedUpdate�ŕ������Z�Ɠ���
    }

    // �j���x��������R���[�`��
    private IEnumerator DestroyAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);  // �w�肵���x����҂�

        DestroyObject();  // �x����ɃI�u�W�F�N�g��j��
    }
}
