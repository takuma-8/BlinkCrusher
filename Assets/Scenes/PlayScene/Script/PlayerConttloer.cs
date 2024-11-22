using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public GameObject[] BlinkArray = new GameObject[3];
    private int BlinkPoint = 3;
    public List<GameObject> WallList;

    public float speed_ = 5.0f;
    public float normalSpeed_ = 5.0f;
    public float blinkSpeed_ = 8.0f;
    public float distance_ = 1.0f;

    private Rigidbody rb;
    private Vector3 targetPosition_;
    [HideInInspector] public bool isMoving_ = false;

    private BoxCollider blinkCollider; // �u�����N�p�̈ꎞ�R���C�_�[
    private string originalTag; // ���̃^�O��ۑ�

    private Vector3 originalPosition; // ���̈ʒu��ێ�
    private float groundCheckDistance = 1.5f; // �n�ʂƂ̋������m�F����͈�

   

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
            rb.rotation = Quaternion.Slerp(rb.rotation, targetRotation, Time.deltaTime * 8.0f);
        }

        if ((Input.GetKeyDown(KeyCode.Space) || Input.GetButtonDown("B")) && BlinkPoint > 0 && !isMoving_)
        {
            // BlinkPoint--;
            StartBlink();
            UpdateBlinkUI();
        }
    }

  

    void Start()
    {
        // Rigidbody �̐ݒ�
        rb = GetComponent<Rigidbody>();
        rb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;  // �Փ˔��胂�[�h�� ContinuousDynamic �ɐݒ�

        // �v���C���[�̏����ʒu�𒲐��i�����n�ʂɂ߂荞��ł���ꍇ�j
        Vector3 initialPosition = transform.position;
        RaycastHit hit;
        if (Physics.Raycast(initialPosition, Vector3.down, out hit, 2f)) // 2 ���j�b�g�ȓ��ɒn�ʂ������
        {
            // �n�ʂɏ�����Ɉړ�������
            transform.position = new Vector3(initialPosition.x, hit.point.y + 0.5f, initialPosition.z);
        }

        // ���̏���������
        speed_ = normalSpeed_;
        originalTag = gameObject.tag; // ���̃^�O��ۑ�
        UpdateBlinkUI();
    }

    void Blink()
    {
        // ���݈ʒu����ڕW�ʒu�܂ł̕������v�Z
        Vector3 direction_ = (targetPosition_ - rb.position).normalized;

        // Raycast�ŏ�Q�������m
        RaycastHit hit;
        if (Physics.Raycast(rb.position, direction_, out hit, blinkSpeed_ * Time.fixedDeltaTime))
        {
            if (hit.collider.CompareTag("Wall"))
            {
                // �ǂɓ��������ꍇ�A���̒n�_�Ńu�����N���I��
                rb.MovePosition(hit.point - direction_ * 0.1f); // �ǂɂԂ������班����O�Œ�~
                Debug.Log("Blink stopped by Wall");
                EndBlink();
                return;
            }
        }

        // �ڕW�ʒu�Ɍ����ĕ����I�Ɉړ�
        Vector3 newPosition = Vector3.MoveTowards(rb.position, targetPosition_, blinkSpeed_ * Time.fixedDeltaTime);
        rb.MovePosition(newPosition);

        // �ڕW�ʒu�ɏ\���߂Â�����ړ����~
        if (Vector3.Distance(rb.position, targetPosition_) <= 0.7f)
        {
            rb.MovePosition(targetPosition_);
            EndBlink();
        }
    }

    void StartBlink()
    {
        // �u�����N��ɏ�Q�����Ȃ����`�F�b�N
        RaycastHit hit;
        if (Physics.Raycast(rb.position, transform.forward, out hit, distance_))
        {
            if (hit.collider.CompareTag("Wall"))
            {
                Debug.Log("Obstacle detected, Blink cancelled!");
                BlinkPoint++; // �u�����N�L�����Z�����Ƀ|�C���g����
                return;
            }
        }

        // �u�����N�J�n���ɑ��x��ύX
        speed_ = blinkSpeed_;  // �ʏ푬�x�ł͂Ȃ��A�u�����N���x�ɐݒ�
        targetPosition_ = rb.position + transform.forward * distance_;
        isMoving_ = true;

        rb.isKinematic = true;
        gameObject.tag = "Blink_player";
        AddBlinkCollider();
    }

    void EndBlink()
    {
        // �u�����N�I�����̏���
        isMoving_ = false;
        speed_ = normalSpeed_;  // �u�����N�I����Ɍ��̑��x�ɖ߂�
        rb.isKinematic = false;

        // �������~�߂�
        rb.velocity = Vector3.zero;

        gameObject.tag = originalTag;
        RemoveBlinkCollider();
    }
    void FixedUpdate()
    {
        if (isMoving_)
        {
            Blink();  // �u�����N�̏���
        }
        else
        {
            // �ʏ�̈ړ�����
            HandleNormalMovement();
            AdjustPositionAfterBlink();  // �u�����N�I����ɒn�ʏC��
        }
    }

    void AdjustPositionAfterBlink()
    {
        // �u�����N�I����Ƀv���C���[���n�ʂɂ��Ȃ��ꍇ�ɏC��
        RaycastHit hit;
        if (Physics.Raycast(transform.position, Vector3.down, out hit, 2f)) // 2���j�b�g�ȓ��ɒn�ʂ������
        {
            // �n�ʂ�������Έʒu�𒲐�
            Vector3 correctedPosition = new Vector3(transform.position.x, hit.point.y + 0.5f, transform.position.z);
            rb.MovePosition(correctedPosition);
        }
        else
        {
            // �n�ʂ��Ȃ��ꍇ�͋󒆂ɂ���Ɖ��肵�āA���̈ʒu��������ɒ���
            Vector3 correctedPosition = new Vector3(transform.position.x, originalPosition.y + 0.5f, transform.position.z);
            rb.MovePosition(correctedPosition);
        }
    }

    void HandleNormalMovement()
    {
        // �ʏ�̈ړ�����
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        Vector3 direction = new Vector3(horizontal, 0, vertical).normalized;

        if (direction.magnitude > 0.1f)
        {
            // �ړ������ɏ]���Ĉړ�
            Vector3 move = direction * speed_ * Time.fixedDeltaTime;
            rb.MovePosition(rb.position + move);

            // �ړ������Ɍ����ĉ�]
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            rb.rotation = Quaternion.Slerp(rb.rotation, targetRotation, Time.fixedDeltaTime * 8.0f);
        }
    }


    private void AddBlinkCollider()
    {
        blinkCollider = gameObject.AddComponent<BoxCollider>();
        blinkCollider.isTrigger = true;
        blinkCollider.center = new Vector3(0, 0, 0.75f);
        blinkCollider.size = new Vector3(1.5f, 1.5f, 1.5f);
    }

    private void RemoveBlinkCollider()
    {
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

