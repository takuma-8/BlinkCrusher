using UnityEngine;

public class CameraController : MonoBehaviour
{
    public float lookSpeedX = 2.0f;   // 横方向の視点回転速度
    public float lookSpeedY = 2.0f;   // 縦方向の視点回転速度


    private Camera playerCamera;      // プレイヤーのカメラ
    private float rotationX = 0;      // 縦方向の回転

    void Start()
    {
        playerCamera = Camera.main; // メインカメラを取得
    }

    void Update()
    {
        // カメラの回転
        float mouseX = Input.GetAxis("HorizontalLook") * lookSpeedX;
        float mouseY = -Input.GetAxis("VerticalLook") * lookSpeedY;  // 縦回転を反転

        rotationX -= mouseY;
        rotationX = Mathf.Clamp(rotationX, -90f, 90f); // 縦方向の回転制限
        playerCamera.transform.localRotation = Quaternion.Euler(rotationX, 0f, 0f); // カメラの縦回転
        transform.Rotate(Vector3.up * mouseX); // プレイヤーの横回転
    }
}
