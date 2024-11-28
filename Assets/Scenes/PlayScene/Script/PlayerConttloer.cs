using UnityEngine;
using UnityEngine.UI;
public class PlayerController : MonoBehaviour
{
    public Image cooldownCircle; // �~�`�Q�[�W�p��UI
    public Image colorCircle;    // �N�[���^�C�����̕⏕�摜
    public Image Blinkimage;     // �펞�\�������摜
    public RectTransform[] BlinkUIPositions; // �u�����NUI��RectTransform�z��

    private bool isCooldown = false; // �N�[���^�C�����̏�Ԃ��Ǘ�
    private float cooldownTimer = 0f; // ���݂̃N�[���^�C���i�s
    private float coolTime = 30f; // �N�[���^�C���̕b��
    public GameObject wallCheck; // wallcheck �q�I�u�W�F�N�g
    public string wallTag = "Wall"; // �ǔ���Ɏg�p����^�O

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
            colorCircle.enabled = false; // �摜���\���ɐݒ�
            Blinkimage.enabled = false;
        }
    }

    void Update()
    {
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

        CoolTime(); // �N�[���^�C���̏�����ǉ�
    }


    private void FixedUpdate()
    {
        if (isMoving_)
        {
            Blink();
        }
    }

    void StartBlink()
    {
        // �u�����N�̈ړ����J�n
        targetPosition_ = rb.position + transform.forward * distance_;
        speed_ = blinkSpeed_;
        isMoving_ = true; // �u�����N���̈ړ��t���O�𗧂Ă�
        rb.isKinematic = true; // Rigidbody �̕������Z�𖳌���
    }

    void Blink()
    {
        // wallcheck ���ǂɐڐG���Ă��邩����
        Collider[] hitColliders = Physics.OverlapSphere(wallCheck.transform.position, 0.1f);
        foreach (var hitCollider in hitColliders)
        {
            if (hitCollider.CompareTag(wallTag))
            {
                // �ǂɂԂ������ꍇ�A���̏�Œ�~
                Debug.Log("Wall detected, stopping blink!");
                StopBlink();
                return;
            }
        }

        // �u�����N�ړ�
        transform.position = Vector3.MoveTowards(transform.position, targetPosition_, blinkSpeed_ * Time.fixedDeltaTime);

        // �ڕW�ʒu�ɓ��B�����ꍇ�A�u�����N���~
        if (Vector3.Distance(transform.position, targetPosition_) <= 0.1f)
        {
            StopBlink();
        }
    }
    void CoolTime()
    {
        // BlinkPoint ���ő�l�ł͂Ȃ��ꍇ�A�N�[���_�E����i�s
        if (BlinkPoint < BlinkArray.Length)
        {
            isCooldown = true; // �N�[���_�E����
            cooldownTimer += Time.deltaTime; // �^�C�}�[��i�s
            cooldownCircle.fillAmount = cooldownTimer / coolTime; // �Q�[�W���X�V

            if (!colorCircle.enabled)
            {
                colorCircle.enabled = true; // �N�[���_�E�����̉摜��\��
            }

            if (!Blinkimage.enabled)
            {
                Blinkimage.enabled = true; // Blinkimage ��\��
            }

            // BlinkArray �̈ʒu�ɃT�[�N�����ړ�
            if (BlinkArray.Length > BlinkPoint)
            {
                // BlinkArray �̈ʒu�� cooldownCircle ���ړ�
                cooldownCircle.rectTransform.position = BlinkArray[BlinkPoint].transform.position;

                // colorCircle �� Blinkimage �̈ʒu�����l�Ɉړ�
                colorCircle.rectTransform.position = BlinkArray[BlinkPoint].transform.position;
                Blinkimage.rectTransform.position = BlinkArray[BlinkPoint].transform.position;
            }

            // ���̓����x�ɐݒ�i�Ⴆ�΁A50%�����ɐݒ�j
            SetBlinkImageTransparency(0.5f); // �����x��50%�ɐݒ�

            if (cooldownTimer >= coolTime) // �N�[���^�C��������
            {
                BlinkPoint++; // �|�C���g����
                cooldownTimer = 0f; // �^�C�}�[�����Z�b�g
                UpdateBlinkUI(); // UI���X�V

                // �ő�l�ɒB�����ꍇ�A�N�[���_�E���I��
                if (BlinkPoint == BlinkArray.Length)
                {
                    cooldownCircle.fillAmount = 0f; // �Q�[�W�����Z�b�g
                    colorCircle.enabled = false; // �摜���\��
                    Blinkimage.enabled = false; // Blinkimage ���\��
                    isCooldown = false;
                }
            }
        }
        else
        {
            // BlinkPoint ���ő�̏ꍇ�A�N�[���_�E���𖳌���
            isCooldown = false;
            colorCircle.enabled = false; // �摜���\��
            Blinkimage.enabled = true;  // Blinkimage �͕\�����ꂽ�܂�
            cooldownCircle.fillAmount = 0f; // �Q�[�W�����Z�b�g
        }
    }






    void StopBlink()
    {
        isMoving_ = false;
        speed_ = normalSpeed_;
        rb.isKinematic = false;
        BlinkPoint--;
        UpdateBlinkUI();

        if (!isCooldown) // �N�[���^�C�����n�܂��Ă��Ȃ��ꍇ
        {
            isCooldown = true; // �N�[���^�C�����J�n
            cooldownTimer = 0f; // �^�C�}�[�����Z�b�g
            colorCircle.enabled = true;
        }
    }


    private void UpdateBlinkUI()
    {
        for (int i = 0; i < BlinkArray.Length; i++)
        {
            BlinkArray[i].SetActive(i < BlinkPoint);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        // �u�����N���ɃG�l�~�[�ɓ��������ꍇ
        if ((other.CompareTag("Enemy1") || other.CompareTag("Enemy2")) && isMoving_)
        {
            Debug.Log("Enemy hit during Blink!");
            EnemyController enemyController = other.GetComponent<EnemyController>();
            if (enemyController != null)
            {
                enemyController.Stun(); // �G�l�~�[���X�^�������鏈��
            }
        }
    }

    void SetBlinkImageTransparency(float alpha)
    {
        Color currentColor = Blinkimage.color;
        currentColor.a = alpha;  // �����x���w�肳�ꂽ�l�ɐݒ�
        Blinkimage.color = currentColor;
    }

}