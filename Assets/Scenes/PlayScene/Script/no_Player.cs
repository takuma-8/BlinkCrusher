using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float speed = 5.0f;
    private CharacterController controller;

    void Start()
    {
        controller = GetComponent<CharacterController>();
    }

    void Update()
    {
        // �v���C���[�̈ړ���������͂���擾
        float moveX = Input.GetAxis("Horizontal");
        float moveZ = Input.GetAxis("Vertical");

        // �ړ��x�N�g�����쐬
        Vector3 move = new Vector3(moveX, 0, moveZ);

        // CharacterController�ňړ�
        controller.Move(move * speed * Time.deltaTime);

        // �ړ����Ă���ꍇ�Ɍ����𒲐�
        if (move != Vector3.zero)
        {
            // �ړ������Ɍ�������
            transform.rotation = Quaternion.LookRotation(move);
        }
    }
}
