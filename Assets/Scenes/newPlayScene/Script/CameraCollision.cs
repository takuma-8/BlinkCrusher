using UnityEngine;

public class CameraCollision : MonoBehaviour
{
    public Transform player; // �v���C���[��Transform
    public Transform cameraTransform; // �J������Transform
    public float cameraRadius = 0.5f; // �J�����̃R���W�����p�̔��a
    public float smoothSpeed = 10f; // �J�����ړ��̃X���[�Y��
    public LayerMask collisionLayer; // �ǂ��Q�������o���邽�߂̃��C���[

    private Vector3 defaultCameraOffset; // �J�����̏����I�t�Z�b�g
    private Vector3 currentCameraPosition; // ���݂̃J�����ʒu

    private void Start()
    {
        if (player == null || cameraTransform == null)
        {
            Debug.LogError("�v���C���[�܂��̓J������Transform���ݒ肳��Ă��܂���I");
            enabled = false;
            return;
        }

        // �����J�����I�t�Z�b�g��ۑ�
        defaultCameraOffset = cameraTransform.position - player.position;
        currentCameraPosition = cameraTransform.position;
    }

    private void LateUpdate()
    {
        Vector3 desiredCameraPosition = player.position + defaultCameraOffset; // ���z�I�ȃJ�����ʒu
        Vector3 direction = desiredCameraPosition - player.position; // �v���C���[����J�����ւ̕���

        // �J�����̏Փ˂��`�F�b�N
        if (Physics.SphereCast(player.position, cameraRadius, direction, out RaycastHit hit, direction.magnitude, collisionLayer))
        {
            // �Փˎ��A�J������ǂ̎�O�ɔz�u
            currentCameraPosition = hit.point - direction.normalized * cameraRadius;
        }
        else
        {
            // �Փ˂��Ȃ��ꍇ�́A�ʏ�̈ʒu�ɖ߂�
            currentCameraPosition = desiredCameraPosition;
        }

        // �J�������X���[�Y�Ɉړ�
        cameraTransform.position = Vector3.Lerp(cameraTransform.position, currentCameraPosition, Time.deltaTime * smoothSpeed);
        cameraTransform.LookAt(player); // �J�������v���C���[������悤�ɂ���
    }

    private void OnDrawGizmos()
    {
        if (player != null && cameraTransform != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawLine(player.position, cameraTransform.position);
            Gizmos.DrawWireSphere(cameraTransform.position, cameraRadius);
        }
    }
}