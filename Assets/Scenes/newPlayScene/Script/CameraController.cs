using UnityEngine;

public class CameraController : MonoBehaviour
{
    public float lookSpeedX = 2.0f;   // ‰¡•ûŒü‚Ì‹“_‰ñ“]‘¬“x
    public float lookSpeedY = 2.0f;   // c•ûŒü‚Ì‹“_‰ñ“]‘¬“x

    private bool isCameraControlEnabled = true; // Flag to check if camera controls are enabled

    private Camera playerCamera;      // ƒvƒŒƒCƒ„[‚ÌƒJƒƒ‰
    private float rotationX = 0;      // c•ûŒü‚Ì‰ñ“]

    void Start()
    {
        playerCamera = Camera.main; // ƒƒCƒ“ƒJƒƒ‰‚ğæ“¾
    }

    void Update()
    {
        if (isCameraControlEnabled)
        {
            HandleCameraRotation(); // Your camera control logic here
        }
        // ƒJƒƒ‰‚Ì‰ñ“]
        
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
        float mouseY = -Input.GetAxis("VerticalLook") * lookSpeedY;  // c‰ñ“]‚ğ”½“]

        rotationX -= mouseY;
        rotationX = Mathf.Clamp(rotationX, -90f, 90f); // c•ûŒü‚Ì‰ñ“]§ŒÀ
        playerCamera.transform.localRotation = Quaternion.Euler(rotationX, 0f, 0f); // ƒJƒƒ‰‚Ìc‰ñ“]
        transform.Rotate(Vector3.up * mouseX); // ƒvƒŒƒCƒ„[‚Ì‰¡‰ñ“]
    }
}
