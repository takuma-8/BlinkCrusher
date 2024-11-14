using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerConttloer : MonoBehaviour
{
    public float speed_ = 5.0f;         // ���̃X�s�[�h
    public float normalSpeed_ = 5.0f;   // �ʏ펞�̃X�s�[�h
    public float blinkSpeed_ = 8.0f;    // �u�����N���̃X�s�[�h
    public float distance_ = 1.0f;   // �u�����N�ňړ����鋗��

    private Rigidbody rb;
    private Vector3 targetPosition_;
    private bool isMoving_ = false;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        speed_ = normalSpeed_;
    }

    // Update is called once per frame
    void Update()
    {
        // ���͂̎擾
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        // �ړ��x�N�g���̌v�Z
        Vector3 direction = new Vector3(horizontal, 0, vertical).normalized;

        if (!isMoving_)
        {
            // �v���C���[�ʒu�̍X�V
            rb.MovePosition(rb.position + direction * speed_ * Time.deltaTime);
        }

        Vector3 movement = new Vector3(horizontal, 0.0f, vertical);




        // �v���C���[��]����
        if (movement.magnitude > 0.1f)  // �����Ȓl���Ɖ�]���Ȃ��炵��
        {
            // �ړ������Ƀ��f������]
            Quaternion targetRotation = Quaternion.LookRotation(movement);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 10.0f);
        }

        // �u�����N
        //if (Input.GetButtonDown("B") && !isMoving_)
        //{
        //    StartBlink();
        //}


        if (Input.GetKeyDown(KeyCode.Space))
        {
            StartBlink();
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
        �@
        isMoving_ = true;
    }


    void Blink()
    {
        Vector3 direction_ = (targetPosition_ - rb.position).normalized;
        rb.MovePosition(rb.position + direction_ * speed_ * Time.fixedDeltaTime);

        Debug.Log($"Current Position: {rb.position}, Target Position: {targetPosition_}");

        if (Vector3.Distance(rb.position, targetPosition_) <= 0.7f)
        {
            // �ŏI�ʒu���Z�b�g
            rb.position = targetPosition_;
            transform.position = targetPosition_;

            rb.velocity = Vector3.zero;        // �ړ����x���[���Ƀ��Z�b�g
            rb.angularVelocity = Vector3.zero; // ��]���x���[���Ƀ��Z�b�g

         
            isMoving_ = false;

            // �X�s�[�h��߂�
            speed_ = normalSpeed_;

            // �f�o�b�O�Ŋm�F
            Debug.Log("Blink�I��");
        }
    }

}
