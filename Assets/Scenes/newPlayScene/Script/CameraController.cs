using UnityEngine;

public class CameraController : MonoBehaviour
{
    public float lookSpeedX = 2.0f;   // �������̎��_��]���x

    private bool isCameraControlEnabled = true; // �J��������̗L�����t���O
    private Camera playerCamera;                // �v���C���[�̃J����
    private float rotationX = 0;                // �c�����̉�]

    void Start()
    {
        playerCamera = Camera.main; // ���C���J�������擾
    }

    void Update()
    {
        if (isCameraControlEnabled)
        {
            HandleCameraRotation(); // �J��������
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
        // Time.deltaTime���g���ĉ�]���x���t���[�����[�g�Ɉˑ������Ȃ�
        float mouseX = Input.GetAxis("HorizontalLook") * lookSpeedX * Time.deltaTime;
        transform.Rotate(Vector3.up * mouseX); // ����]
    }

    public void SetCameraRotation(Quaternion rotation)
    {
        // �J�����̉�]��K�p
        playerCamera.transform.rotation = rotation;

        // rotationX ���X�V
        rotationX = playerCamera.transform.localRotation.eulerAngles.x;

        // 180�x������␳
        if (rotationX > 180)
        {
            rotationX -= 360;
        }
    }

    public void SetPlayerRotation(Quaternion rotation)
    {
        // �v���C���[�̉�������]��K�p
        transform.rotation = Quaternion.Euler(0, rotation.eulerAngles.y, 0);
    }
}
