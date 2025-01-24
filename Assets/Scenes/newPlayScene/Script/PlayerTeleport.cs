using UnityEngine;
using System.Collections.Generic;

public class PlayerTeleport : MonoBehaviour
{
    public List<Transform> targetObjects; // 複数のトリガーオブジェクトのTransform
    public string fireButton = "Fire2"; // コントローラーのFire2ボタン
    public float triggerRange = 5f; // トリガー範囲の半径
    public List<Vector3> cameraDirections; // ロッカーごとのカメラ方向
    public Canvas emperorCanvas; // ロッカー内の演出用Canvas

    private PlayerController playerController;
    private CameraController cameraController; // CameraController を参照
    private bool isEmperor = false;
    private Vector3 originalPosition;
    private Quaternion originalCameraRotation;
    public Camera playerCamera;
    public Camera emperorCamera;

    private void Start()
    {
        playerController = GetComponent<PlayerController>();
        cameraController = GetComponent<CameraController>(); // CameraController を取得
        if (playerController == null)
            Debug.LogError("PlayerControllerが見つかりません！");

        if (emperorCanvas != null)
            emperorCanvas.enabled = false; // 初期状態では非表示
    }

    private void EnterEmperorState(Transform targetObject)
    {
        originalPosition = transform.position;
        originalCameraRotation = playerCamera.transform.rotation;

        playerController.LockPlayerActions();
        playerController.ChangePlayerTag("not_Player");
        isEmperor = true;

        // カメラ操作を無効化
        cameraController.DisableCameraControl(); // CameraController のメソッドを呼び出し

        // カメラ切り替え
        if (playerCamera != null) playerCamera.gameObject.SetActive(false);
        if (emperorCamera != null)
        {
            emperorCamera.gameObject.SetActive(true);
            int index = targetObjects.IndexOf(targetObject);
            if (index >= 0 && index < cameraDirections.Count)
            {
                // カメラ方向を設定し、特定の軸を固定
                Vector3 cameraDirection = cameraDirections[index];
                cameraDirection.x = 0; // ピッチ（上下方向の回転）を固定
                cameraDirection.z = 0; // ロール（横傾き）を固定
                emperorCamera.transform.rotation = Quaternion.Euler(cameraDirection);
            }
        }

        // ロッカー内演出
        if (emperorCanvas != null)
            emperorCanvas.enabled = true;

        Debug.Log("エンペラー状態に入りました。");
    }

    private void ExitEmperorState()
    {
        playerController.UnlockPlayerActions();
        playerController.ChangePlayerTag("Player");
        isEmperor = false;

        // カメラ操作を復元
        cameraController.EnableCameraControl(); // CameraController のメソッドを呼び出し

        // カメラ切り替え
        if (playerCamera != null) playerCamera.gameObject.SetActive(true);
        if (emperorCamera != null)
            emperorCamera.gameObject.SetActive(false);

        // 元の位置とカメラ方向に戻す
        transform.position = originalPosition;
        playerCamera.transform.rotation = originalCameraRotation;

        // ロッカー内演出解除
        if (emperorCanvas != null)
            emperorCanvas.enabled = false;

        Debug.Log("エンペラー状態が解除されました。");
    }

    private void Update()
    {
        if (targetObjects == null || targetObjects.Count == 0) return;

        foreach (Transform targetObject in targetObjects)
        {
            float distance = Vector3.Distance(transform.position, targetObject.position);

            if (distance <= triggerRange && Input.GetButtonDown(fireButton))
            {
                if (isEmperor)
                {
                    ExitEmperorState();
                }
                else
                {
                    EnterEmperorState(targetObject);
                    TeleportToTarget(targetObject);
                }
            }
        }
    }

    private void TeleportToTarget(Transform targetObject)
    {
        transform.position = targetObject.position;
        Debug.Log("指定されたロッカーにテレポートしました！");
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, triggerRange);
    }

    public bool IsEmperor()
    {
        return isEmperor;
    }
}
