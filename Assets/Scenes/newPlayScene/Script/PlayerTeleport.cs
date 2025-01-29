using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;

public class PlayerTeleport : MonoBehaviour
{
    public List<Transform> targetObjects; // 複数のトリガーオブジェクトのTransform
    public string fireButton = "Fire2"; // コントローラーのFire2ボタン
    public float triggerRange = 5f; // トリガー範囲の半径
    public List<Vector3> cameraDirections; // ロッカーごとのカメラ方向
    public Image locker; // ロッカー内の演出用Canvas

    private PlayerController playerController;
    private CameraController cameraController; // CameraController を参照
    private bool isEmperor = false;
    public Camera playerCamera;
    public Camera emperorCamera;

    private void Start()
    {
        playerController = GetComponent<PlayerController>();
        cameraController = GetComponent<CameraController>(); // CameraController を取得

        if (playerController == null)
            Debug.LogError("PlayerControllerが見つかりません！");

        if (locker != null)
            locker.enabled = false; // 初期状態では非表示
    }

    private void EnterEmperorState(Transform targetObject)
    {
        playerController.LockPlayerActions();
        playerController.ChangePlayerTag("not_Player");
        isEmperor = true;

        // カメラ操作を無効化
        cameraController.DisableCameraControl();

        // メインカメラをリセット
        if (playerCamera != null)
        {
            playerCamera.transform.rotation = Quaternion.Euler(0, 0, 0);
            playerCamera.gameObject.SetActive(false);
        }

        // エンペラーカメラをリセット
        if (emperorCamera != null)
        {
            if (!emperorCamera.gameObject.activeInHierarchy)
            {
                emperorCamera.gameObject.SetActive(true);
            }

            // プレイヤーの向きをカメラの向きに合わせる
            Vector3 cameraDirection = cameraDirections[targetObjects.IndexOf(targetObject)];
            cameraDirection.x = 0;
            cameraDirection.z = 0;

            // プレイヤーの回転を先に設定
            Vector3 playerRotation = new Vector3(0, cameraDirection.y, 0);
            transform.rotation = Quaternion.Euler(playerRotation);

            // 次にカメラの回転を設定
            emperorCamera.transform.rotation = Quaternion.Euler(cameraDirection);
        }

        // ロッカー内演出
        UpdateLockerVisibility();

        Debug.Log("エンペラー状態に入りました。");
    }

    private void ExitEmperorState()
    {
        playerController.UnlockPlayerActions();
        playerController.ChangePlayerTag("Player");
        isEmperor = false;

        // エンペラーカメラの回転をメインカメラに適用
        if (emperorCamera != null && playerCamera != null)
        {
            Quaternion emperorRotation = emperorCamera.transform.rotation;

            // プレイヤーのY軸回転のみエンペラーカメラの回転に合わせる
            Vector3 cameraEulerAngles = emperorRotation.eulerAngles;
            Vector3 playerRotation = new Vector3(0, cameraEulerAngles.y, 0);
            transform.rotation = Quaternion.Euler(playerRotation); // プレイヤーの回転をY軸だけ更新

            // メインカメラの回転も更新
            playerCamera.transform.rotation = emperorRotation;
            Debug.Log($"エンペラーカメラの回転をメインカメラに適用しました: {emperorRotation.eulerAngles}");
        }
        else
        {
            Debug.LogWarning("エンペラーカメラまたはメインカメラが設定されていません。");
        }

        // メインカメラを有効化
        if (playerCamera != null)
        {
            playerCamera.gameObject.SetActive(true);
        }

        // エンペラーカメラを無効化
        if (emperorCamera != null)
        {
            emperorCamera.gameObject.SetActive(false);
        }

        // カメラコントロールを復元
        cameraController.EnableCameraControl();

        // プレイヤーが向いている方向に一歩前に出る
        Vector3 forwardDirection = transform.forward; // プレイヤーの向きに基づく
        transform.position += forwardDirection; // 一歩前に進む

        // ロッカー内演出を解除
        UpdateLockerVisibility();

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

        // エンペラー状態に応じたロッカー表示の更新 (念のため)
        UpdateLockerVisibility();
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

    private void UpdateLockerVisibility()
    {
        if (locker != null)
        {
            locker.enabled = isEmperor;
        }
    }
}
