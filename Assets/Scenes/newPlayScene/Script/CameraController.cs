using UnityEngine;

public class CameraController : MonoBehaviour
{
    public float lookSpeedX = 2.0f;   // 横方向の視点回転速度

    private bool isCameraControlEnabled = true; // カメラ制御の有効化フラグ
    private Camera playerCamera;                // プレイヤーのカメラ
    private float rotationX = 0;                // 縦方向の回転

    void Start()
    {
        playerCamera = Camera.main; // メインカメラを取得
    }

    void Update()
    {
        if (isCameraControlEnabled)
        {
            HandleCameraRotation(); // カメラ制御
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
        // Time.deltaTimeを使って回転速度をフレームレートに依存させない
        float mouseX = Input.GetAxis("HorizontalLook") * lookSpeedX * Time.deltaTime;
        transform.Rotate(Vector3.up * mouseX); // 横回転
    }

    public void SetCameraRotation(Quaternion rotation)
    {
        // カメラの回転を適用
        playerCamera.transform.rotation = rotation;

        // rotationX を更新
        rotationX = playerCamera.transform.localRotation.eulerAngles.x;

        // 180度超えを補正
        if (rotationX > 180)
        {
            rotationX -= 360;
        }
    }

    public void SetPlayerRotation(Quaternion rotation)
    {
        // プレイヤーの横方向回転を適用
        transform.rotation = Quaternion.Euler(0, rotation.eulerAngles.y, 0);
    }
}
