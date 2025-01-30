using UnityEngine;

public class CameraController : MonoBehaviour
{
    public float lookSpeedX = 200.0f; // 横方向の視点回転速度

    private bool isCameraControlEnabled = true; // カメラ制御の有効化フラグ
    private Camera playerCamera; // プレイヤーのカメラ

    void Start()
    {
        playerCamera = Camera.main; // メインカメラを取得
        if (playerCamera == null)
        {
            Debug.LogError("Main Camera が見つかりません！");
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
        // マウスやコントローラーの入力を取得
        float mouseX = Input.GetAxis("HorizontalLook") * lookSpeedX * Time.deltaTime;

        // プレイヤー本体の回転（左右）
        transform.Rotate(Vector3.up * mouseX);
    }

    public void SetPlayerRotation(Quaternion rotation)
    {
        // プレイヤーの横方向回転を適用
        transform.rotation = Quaternion.Euler(0, rotation.eulerAngles.y, 0);
    }
}
