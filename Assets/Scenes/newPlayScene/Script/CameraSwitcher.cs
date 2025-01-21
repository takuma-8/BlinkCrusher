using UnityEngine;

public class CameraSwitcher : MonoBehaviour
{
    public Camera mainCamera; // 通常時のカメラ
    public Camera secondaryCamera; // 切り替え用のカメラ

    void Start()
    {
        // 初期化：メインカメラを有効化、セカンダリカメラを無効化
        SetCameraState(true);
    }

    void Update()
    {
        // XboxコントローラーのYボタン（joystick button 3）を押している間だけセカンダリカメラを有効化
        if (Input.GetKey(KeyCode.JoystickButton3))
        {
            SetCameraState(false); // セカンダリカメラに切り替え
        }
        else
        {
            SetCameraState(true); // メインカメラに戻す
        }
    }

    void SetCameraState(bool isMainActive)
    {
        // カメラの状態を切り替える
        if (mainCamera != null) mainCamera.enabled = isMainActive;
        if (secondaryCamera != null) secondaryCamera.enabled = !isMainActive;
    }
}
