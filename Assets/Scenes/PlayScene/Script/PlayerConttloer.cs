using UnityEngine;

public class PlayerController : MonoBehaviour
{
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

    void StopBlink()
    {
        isMoving_ = false; // �u�����N�I��
        speed_ = normalSpeed_; // �ʏ푬�x�ɖ߂�
        rb.isKinematic = false; // Rigidbody �����ɖ߂�
        BlinkPoint--; // �u�����N�|�C���g�����炷
        UpdateBlinkUI();
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
}








/*

using System.Collections;
using UnityEngine;

public class PlayerConttloer : MonoBehaviour
{
    public GameObject[] BlinkArray = new GameObject[3];
    private int BlinkPoint = 3;

    public float speed_ = 5.0f;
    public float normalSpeed_ = 5.0f;
    public float blinkSpeed_ = 8.0f;
    public float distance_ = 1.0f;

    private Rigidbody rb;
    private Vector3 targetPosition_;
    [HideInInspector] public bool isMoving_ = false;

    private BoxCollider blinkCollider; // �u�����N�p�̈ꎞ�R���C�_�[
    private string originalTag; // ���̃^�O��ۑ�

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        speed_ = normalSpeed_;
        originalTag = gameObject.tag; // ���̃^�O��ۑ�
        UpdateBlinkUI();
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
            BlinkPoint--;
            StartBlink();
            UpdateBlinkUI();
        }
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
        // �u�����N��ɏ�Q�����Ȃ����`�F�b�N
        RaycastHit hit;
        if (Physics.Raycast(transform.position, transform.forward, out hit, distance_))
        {
            if (hit.collider.CompareTag("Wall"))
            {
                // �ǂ�����ꍇ�A�u�����N���L�����Z��
                Debug.Log("Obstacle detected, Blink cancelled!");
                BlinkPoint++;
                return;
            }
        }

        targetPosition_ = rb.position + transform.forward * distance_;
        speed_ = blinkSpeed_;
        isMoving_ = true;

        rb.isKinematic = true;
        gameObject.tag = "Blink_player";
        AddBlinkCollider();
    }

    void Blink()
    {
        // �u�����N��܂ł̕������v�Z
        Vector3 direction_ = (targetPosition_ - transform.position).normalized;

        // MovePosition�ŕ����I�Ɉړ�
        Vector3 newPosition = Vector3.MoveTowards(transform.position, targetPosition_, blinkSpeed_ * Time.fixedDeltaTime);
        rb.MovePosition(newPosition);

        // �ڕW�ʒu�ɏ\���߂Â�����ړ����~
        if (Vector3.Distance(transform.position, targetPosition_) <= 0.7f)
        {
            rb.MovePosition(targetPosition_);
            EndBlink();
        }
    }

    /*
    void Blink()
    {
        // �u�����N��܂ł̕������v�Z
        Vector3 direction_ = (targetPosition_ - transform.position).normalized;

        // �u�����N�ړ��𒼐�transform.position�ōs��
        transform.position = Vector3.MoveTowards(transform.position, targetPosition_, blinkSpeed_ * Time.fixedDeltaTime);

        // �ڕW�ʒu�ɏ\���߂Â�����ړ����~
        if (Vector3.Distance(transform.position, targetPosition_) <= 0.7f)
        {
            transform.position = targetPosition_;

            isMoving_ = false;
            speed_ = normalSpeed_;

            // Rigidbody �� isKinematic �����ɖ߂�
            rb.isKinematic = false;

            // �^�O�����ɖ߂�
            gameObject.tag = originalTag;

            // �R���C�_�[���폜
            RemoveBlinkCollider();

            
        }
        
    }




    private void AddBlinkCollider()
    {
        // �R���C�_�[�𐶐�
        blinkCollider = gameObject.AddComponent<BoxCollider>();
        blinkCollider.isTrigger = true;

        // �R���C�_�[�̃T�C�Y�ƈʒu��ݒ�
        blinkCollider.center = new Vector3(0, 0, 0.75f);
        blinkCollider.size = new Vector3(1.5f, 1.5f, 1.5f);
    }

    private void RemoveBlinkCollider()
    {
        // �R���C�_�[���폜
        if (blinkCollider != null)
        {
            Destroy(blinkCollider);
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
        if (other.CompareTag("Enemy1") && isMoving_)
        {
            Debug.Log("Enemy hit during Blink!");
            EnemyController enemyController = other.GetComponent<EnemyController>();
            if (enemyController != null)
            {
                enemyController.Stun();
            }
        }
    }
}
*/

