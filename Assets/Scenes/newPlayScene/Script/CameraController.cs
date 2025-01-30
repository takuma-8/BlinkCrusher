using UnityEngine;

public class CameraController : MonoBehaviour
{
    public float lookSpeedX = 200.0f; // �������̎��_��]���x

    private bool isCameraControlEnabled = true; // �J��������̗L�����t���O
    private Camera playerCamera; // �v���C���[�̃J����

    void Start()
    {
        playerCamera = Camera.main; // ���C���J�������擾
        if (playerCamera == null)
        {
            Debug.LogError("Main Camera ��������܂���I");
        }
    }

    void Update()
    {
        if (isCameraControlEnabled)
        {
            HandleCameraRotation();
        }
    }

    public void DisableCameraControl()
    {
        isCameraControlEnabled = false;
    }

    public void EnableCameraControl()
    {
        isCameraControlEnabled = true;
    }

    void HandleCameraRotation()
    {
        // �}�E�X��R���g���[���[�̓��͂��擾
        float mouseX = Input.GetAxis("HorizontalLook") * lookSpeedX * Time.deltaTime;

        // �v���C���[�{�̂̉�]�i���E�j
        transform.Rotate(Vector3.up * mouseX);
    }

    public void SetPlayerRotation(Quaternion rotation)
    {
        // �v���C���[�̉�������]��K�p
        transform.rotation = Quaternion.Euler(0, rotation.eulerAngles.y, 0);
    }
}
