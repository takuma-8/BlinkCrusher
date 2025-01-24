using UnityEngine;

public class CameraCollision : MonoBehaviour
{
    public Transform cameraTransform; // �J������Transform
    public Camera mainCamera; // �ʏ펞�̃J����
    public Camera emperorCamera; // �G���y���[��ԗp�̃J����

    private bool isEmperorMode = false; // �G���y���[��Ԃ��ǂ���

    private void LateUpdate()
    {
        if (isEmperorMode)
        {
            // �G���y���[��Ԃł̓J����������~
            return;
        }
    }

    public void SetEmperorMode(bool isActive, Vector3 fixedDirection)
    {
        isEmperorMode = isActive;
        if (isEmperorMode)
        {
            if (mainCamera != null) mainCamera.gameObject.SetActive(false);
            if (emperorCamera != null)
            {
                emperorCamera.gameObject.SetActive(true);
                emperorCamera.transform.rotation = Quaternion.Euler(fixedDirection);
                Debug.Log("�G���y���[��Ԃ̃J�����ɐ؂�ւ��A�������Œ肵�܂����B");
            }
        }
        else
        {
            if (mainCamera != null) mainCamera.gameObject.SetActive(true);
            if (emperorCamera != null) emperorCamera.gameObject.SetActive(false);
            Debug.Log("�ʏ��Ԃ̃J�����ɖ߂�܂����B");
        }
    }

    private void OnDrawGizmos()
    {
        if (cameraTransform != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(cameraTransform.position, 0.5f);
        }
    }
}
