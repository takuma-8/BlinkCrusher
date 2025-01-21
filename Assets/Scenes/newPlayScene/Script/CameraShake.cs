using UnityEngine;

public class CameraShake : MonoBehaviour
{
    public Transform player; // �v���C���[��Transform
    public float shakeAmount = 0.1f; // �h��̑傫��
    public float shakeSpeed = 5f; // �h��̑���

    private Vector3 originalPosition; // �J�����̌��̈ʒu
    private float shakeTimer = 0f; // �h��p�̃^�C�}�[

    void Start()
    {
        // �J�����̏����ʒu���L�^
        originalPosition = transform.localPosition;
    }

    void Update()
    {
        // �v���C���[���ړ������ǂ������m�F�i��: �v���C���[�̑��x�����ȏォ�j
        if (IsPlayerMoving())
        {
            // �h����v�Z
            shakeTimer += Time.deltaTime * shakeSpeed;
            float offsetX = Mathf.Sin(shakeTimer) * shakeAmount;
            float offsetY = Mathf.Cos(shakeTimer) * shakeAmount * 0.5f; // ���h����c�h���������
            transform.localPosition = originalPosition + new Vector3(offsetX, offsetY, 0);
        }
        else
        {
            // �v���C���[����~���̏ꍇ�A�J���������̈ʒu�ɖ߂�
            transform.localPosition = Vector3.Lerp(transform.localPosition, originalPosition, Time.deltaTime * 10f);
            shakeTimer = 0f;
        }
    }

    // �v���C���[���ړ������ǂ����𔻒�
    private bool IsPlayerMoving()
    {
        if (player.TryGetComponent(out Rigidbody rb))
        {
            return rb.velocity.magnitude > 0.1f; // �ړ����Ă���Ɣ��f���鑬�x��臒l
        }
        else if (player.TryGetComponent(out CharacterController controller))
        {
            return controller.velocity.magnitude > 0.1f; // CharacterController���g���Ă���ꍇ
        }

        // �v���C���[�̈ړ���Ԃ�ʂ̊�Ń`�F�b�N����ꍇ�͂�����ύX
        return false;
    }
}
