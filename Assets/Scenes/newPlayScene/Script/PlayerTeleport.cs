using UnityEngine;
using System.Collections.Generic;

public class PlayerTeleport : MonoBehaviour
{
    public List<Transform> targetObjects; // 複数のトリガーオブジェクトのTransform
    public List<Collider> targetColliders; // 複数のトリガーオブジェクトのCollider
    public List<MeshRenderer> targetMeshRenderers; // 複数のトリガーオブジェクトのMeshRenderer
    public string fireButton = "Fire2"; // コントローラーのFire2ボタン
    public float triggerRange = 5f; // トリガー範囲の半径（デフォルト5）
    public float collisionCheckDistance = 1f; // 衝突チェックの距離（デフォルト1）

    private PlayerController playerController; // プレイヤーのController
    private bool isEmperor = false; // プレイヤーがエンペラー状態かどうかを管理
    private Vector3 originalPosition; // エンペラー状態前のプレイヤー位置
    private CameraCollision cameraCollision;

    public Camera playerCamera; // プレイヤーのカメラ（Inspectorから指定）
    public Camera emperorCamera; // エンペラー状態用カメラ（Inspectorから指定）

    private void Start()
    {
        // PlayerControllerを取得
        playerController = GetComponent<PlayerController>();
        if (playerController == null)
        {
            Debug.LogError("PlayerControllerが見つかりません！このスクリプトをアタッチするGameObjectにPlayerControllerコンポーネントを追加してください。");
        }

        // CameraCollisionを無効化
        cameraCollision = FindObjectOfType<CameraCollision>();
        if (cameraCollision == null)
        {
            Debug.LogError("CameraCollisionが見つかりません！シーン内に存在するか確認してください。");
        }
        else
        {
            cameraCollision.enabled = false; // 初期状態で無効化
        }

        // プレイヤーカメラとエンペラーカメラが指定されていない場合はエラーメッセージを出す
        if (playerCamera == null)
        {
            Debug.LogError("playerCameraが設定されていません！Inspectorで設定してください。");
        }
        if (emperorCamera == null)
        {
            Debug.LogError("emperorCameraが設定されていません！Inspectorで設定してください。");
        }
    }
    private bool IsPositionBlocked(Vector3 position)
    {
        // プレイヤーのキャラクターのサイズを考慮したレイキャスト
        RaycastHit hit;
        float playerHeight = 2f;  // プレイヤーの高さ（適切な値を設定）
        if (Physics.Raycast(position + Vector3.up * playerHeight * 0.5f, Vector3.down, out hit, collisionCheckDistance + playerHeight))
        {
            return true; // 衝突があれば位置がブロックされている
        }
        return false;
    }

    private void EnterEmperorState()
    {
        if (playerController == null)
        {
            Debug.LogError("playerControllerがnullです。PlayerControllerがアタッチされているか確認してください。");
            return;
        }

        originalPosition = transform.position; // エンペラー状態前の位置を記録
        playerController.LockPlayerActions();
        playerController.ChangePlayerTag("not_Player");
        isEmperor = true;

        // カメラの切り替え
        if (playerCamera != null) playerCamera.gameObject.SetActive(false); // Main Cameraを無効にする
        if (emperorCamera != null) emperorCamera.gameObject.SetActive(true); // エンペラー用カメラを有効にする

        if (cameraCollision != null)
        {
            cameraCollision.enabled = true; // エンペラー状態で有効化
            cameraCollision.cameraTransform = emperorCamera.transform; // エンペラー用カメラを設定
        }

        Debug.Log("プレイヤーはエンペラー状態になり、動けなくなりました！");
    }

    private void ExitEmperorState()
    {
        if (playerController == null)
        {
            Debug.LogError("playerControllerがnullです。PlayerControllerがアタッチされているか確認してください。");
            return;
        }

        playerController.UnlockPlayerActions();
        playerController.ChangePlayerTag("Player");
        isEmperor = false;

        // カメラの切り替え
        if (playerCamera != null) playerCamera.gameObject.SetActive(true); // Main Cameraを再表示
        if (emperorCamera != null) emperorCamera.gameObject.SetActive(false); // エンペラー用カメラを無効にする

        if (cameraCollision != null)
        {
            cameraCollision.enabled = false; // 通常状態で無効化
        }

        // プレイヤーをエンペラー状態前の位置に戻す
        transform.position = originalPosition;

        // トリガーオブジェクトの状態を元に戻す
        RestoreTriggerObjectState();

        Debug.Log("エンペラー状態が解除され、プレイヤーは元の位置に戻りました！");
    }

    private void RestoreTriggerObjectState()
    {
        // すべてのトリガーオブジェクトの状態を元に戻す
        for (int i = 0; i < targetObjects.Count; i++)
        {
            if (targetColliders[i] != null)
            {
                targetColliders[i].isTrigger = true; // isTriggerを元に戻す
            }

            if (targetMeshRenderers[i] != null)
            {
                targetMeshRenderers[i].enabled = true; // MeshRendererを元に戻す
            }
        }
    }

    private void Update()
    {
        // targetObjectsがnullまたは空でないかを確認
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
                    EnterEmperorState();
                    TeleportToTarget(targetObject);
                    DisableTriggerObject(targetObject);
                }
            }
        }
    }

    private void TeleportToTarget(Transform targetObject)
    {
        Vector3 targetPosition = targetObject.position;

        if (IsPositionBlocked(targetPosition))
        {
            targetPosition = GetSafePosition(targetPosition);
            Debug.Log("プレイヤーが壁に衝突したため、位置を調整しました！");
        }

        transform.position = targetPosition;
        Debug.Log("プレイヤーが移動しました！ 新しい位置: " + transform.position);
    }

    

    private Vector3 GetSafePosition(Vector3 targetPosition)
    {
        Vector3 safePosition = targetPosition + transform.forward * collisionCheckDistance;

        if (!IsPositionBlocked(safePosition))
        {
            return safePosition;
        }

        return targetPosition;
    }

    private void DisableTriggerObject(Transform targetObject)
    {
        int index = targetObjects.IndexOf(targetObject);

        if (index >= 0 && index < targetColliders.Count)
        {
            if (targetColliders[index] != null)
            {
                targetColliders[index].enabled = false;
                targetColliders[index].isTrigger = false;
            }

            if (targetMeshRenderers[index] != null)
            {
                targetMeshRenderers[index].enabled = false;
            }
        }

        Debug.Log("トリガーオブジェクトの機能を無効化しました！");
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