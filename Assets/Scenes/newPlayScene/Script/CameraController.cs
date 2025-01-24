using UnityEngine;

public class CameraController : MonoBehaviour
{
    public float lookSpeedX = 2.0f;   // �������̎��_��]���x
    public float lookSpeedY = 2.0f;   // �c�����̎��_��]���x

    private bool isCameraControlEnabled = true; // Flag to check if camera controls are enabled

    private Camera playerCamera;      // �v���C���[�̃J����
    private float rotationX = 0;      // �c�����̉�]

    void Start()
    {
        playerCamera = Camera.main; // ���C���J�������擾
    }

    void Update()
    {
        if (isCameraControlEnabled)
        {
            HandleCameraRotation(); // Your camera control logic here
        }
        // �J�����̉�]
        
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
        float mouseX = Input.GetAxis("HorizontalLook") * lookSpeedX;
        float mouseY = -Input.GetAxis("VerticalLook") * lookSpeedY;  // �c��]�𔽓]

        rotationX -= mouseY;
        rotationX = Mathf.Clamp(rotationX, -90f, 90f); // �c�����̉�]����
        playerCamera.transform.localRotation = Quaternion.Euler(rotationX, 0f, 0f); // �J�����̏c��]
        transform.Rotate(Vector3.up * mouseX); // �v���C���[�̉���]
    }
}
