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
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 10.0f);
        }

        if (Input.GetKeyDown(KeyCode.Space) && BlinkPoint > 0 && !isMoving_)
        {
            StartBlink();
            BlinkPoint--;
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
        targetPosition_ = rb.position + transform.forward * distance_;
        speed_ = blinkSpeed_;
        isMoving_ = true;

        // �^�O��ύX
        gameObject.tag = "Blink_player";

        // �v���C���[�ɃR���C�_�[��ǉ�
        AddBlinkCollider();
    }

    void Blink()
    {
        Vector3 direction_ = (targetPosition_ - rb.position).normalized;
        rb.MovePosition(rb.position + direction_ * speed_ * Time.fixedDeltaTime);

        if (Vector3.Distance(rb.position, targetPosition_) <= 0.7f)
        {
            rb.position = targetPosition_;
            transform.position = targetPosition_;

            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;

            isMoving_ = false;
            speed_ = normalSpeed_;

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
