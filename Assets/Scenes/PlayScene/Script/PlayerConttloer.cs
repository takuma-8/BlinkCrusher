using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    [Header("Blink Parameters")]
    public float blinkRange = 2.0f;
    public Vector3 blinkOffset = new Vector3(0, 0, 1.5f);

    private bool isBlinking = false;
    private bool isActionLocked = false; // 破壊中の操作ロック
    public Image cooldownCircle;
    public Image colorCircle;
    public Image Blinkimage;
    public RectTransform[] BlinkUIPositions;

    private bool isCooldown = false;
    private float cooldownTimer = 0f;
    private float coolTime = 30f;
    public GameObject wallCheck;
    public string wallTag = "Wall";

    public GameObject[] BlinkArray = new GameObject[3];
    private int BlinkPoint = 3;

    public float speed_ = 5.0f;
    public float normalSpeed_ = 5.0f;
    public float blinkSpeed_ = 8.0f;
    public float distance_ = 1.0f;
    private Rigidbody rb;
    private Vector3 targetPosition_;
    [HideInInspector] public bool isMoving_ = false;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        speed_ = normalSpeed_;
        UpdateBlinkUI();

        if (colorCircle != null)
        {
            colorCircle.enabled = false;
            Blinkimage.enabled = false;
        }
    }

    void Update()
    {
        if (isActionLocked) return; // 操作ロック中は何も受け付けない

        if (isBlinking)
        {
            DetectEnemiesInBlinkRange();
        }

        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        Vector3 direction = new Vector3(horizontal, 0, vertical).normalized;

        if (!isMoving_)
        {
            rb.MovePosition(rb.position + direction * speed_ * Time.deltaTime);
        }

        Vector3 movement = new Vector3(horizontal, 0.0f, vertical);

        if (movement.magnitude > 0.1f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(movement);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 8.0f);
        }

        if ((Input.GetKeyDown(KeyCode.Space) || Input.GetButtonDown("B")) && BlinkPoint > 0 && !isMoving_)
        {
            StartBlink();
            UpdateBlinkUI();
        }

        CoolTime();
    }

    private void FixedUpdate()
    {
        if (isActionLocked) return; // 操作ロック中は動作しない

        if (isMoving_)
        {
            Blink();
        }
    }

    void StartBlink()
    {
        gameObject.tag = "PlayerBlinking";
        isBlinking = true;
        targetPosition_ = rb.position + transform.forward * distance_;
        speed_ = blinkSpeed_;
        isMoving_ = true;
        rb.isKinematic = true;
    }

    void Blink()
    {
        Collider[] hitColliders = Physics.OverlapSphere(wallCheck.transform.position, 0.1f);
        foreach (var hitCollider in hitColliders)
        {
            if (hitCollider.CompareTag(wallTag))
            {
                Debug.Log("Wall detected, stopping blink!");
                StopBlink();
                return;
            }
        }

        transform.position = Vector3.MoveTowards(transform.position, targetPosition_, blinkSpeed_ * Time.fixedDeltaTime);

        if (Vector3.Distance(transform.position, targetPosition_) <= 0.1f)
        {
            StopBlink();
        }
    }

    void CoolTime()
    {
        if (BlinkPoint < BlinkArray.Length)
        {
            isCooldown = true;
            cooldownTimer += Time.deltaTime;
            cooldownCircle.fillAmount = cooldownTimer / coolTime;

            if (!colorCircle.enabled)
            {
                colorCircle.enabled = true;
            }

            if (!Blinkimage.enabled)
            {
                Blinkimage.enabled = true;
            }

            if (BlinkArray.Length > BlinkPoint)
            {
                cooldownCircle.rectTransform.position = BlinkArray[BlinkPoint].transform.position;
                colorCircle.rectTransform.position = BlinkArray[BlinkPoint].transform.position;
                Blinkimage.rectTransform.position = BlinkArray[BlinkPoint].transform.position;
            }

            SetBlinkImageTransparency(0.5f);

            if (cooldownTimer >= coolTime)
            {
                BlinkPoint++;
                cooldownTimer = 0f;
                UpdateBlinkUI();

                if (BlinkPoint == BlinkArray.Length)
                {
                    cooldownCircle.fillAmount = 0f;
                    colorCircle.enabled = false;
                    Blinkimage.enabled = false;
                    isCooldown = false;
                }
            }
        }
        else
        {
            isCooldown = false;
            colorCircle.enabled = false;
            Blinkimage.enabled = true;
            cooldownCircle.fillAmount = 0f;
        }
    }

    public void LockPlayerActions()
    {
        isActionLocked = true;
        rb.isKinematic = true; // 動きを完全に停止
    }

    public void UnlockPlayerActions()
    {
        isActionLocked = false;
        rb.isKinematic = false; // 動作を再開
    }

    void StopBlink()
    {
        gameObject.tag = "Player";
        isBlinking = false;
        isMoving_ = false;
        speed_ = normalSpeed_;
        rb.isKinematic = false;
        BlinkPoint--;
        UpdateBlinkUI();

        if (!isCooldown)
        {
            isCooldown = true;
            cooldownTimer = 0f;
            colorCircle.enabled = true;
        }
    }

    void DetectEnemiesInBlinkRange()
    {
        Vector3 detectionCenter = transform.position + transform.TransformDirection(blinkOffset);
        Collider[] hitColliders = Physics.OverlapSphere(detectionCenter, blinkRange);

        foreach (var hitCollider in hitColliders)
        {
            if (hitCollider.CompareTag("Enemy1") || hitCollider.CompareTag("Enemy2"))
            {
                Debug.Log("Enemy hit during Blink!");
                EnemyController enemyController = hitCollider.GetComponent<EnemyController>();
                if (enemyController != null)
                {
                    enemyController.Stun();
                }
            }
        }
    }

    void OnDrawGizmos()
    {
        Gizmos.color = isBlinking ? Color.red : Color.green;
        Vector3 detectionCenter = transform.position + transform.TransformDirection(blinkOffset);
        Gizmos.DrawWireSphere(detectionCenter, blinkRange);
    }

    private void UpdateBlinkUI()
    {
        for (int i = 0; i < BlinkArray.Length; i++)
        {
            BlinkArray[i].SetActive(i < BlinkPoint);
        }
    }

    void SetBlinkImageTransparency(float alpha)
    {
        Color currentColor = Blinkimage.color;
        currentColor.a = alpha;
        Blinkimage.color = currentColor;
    }
}
