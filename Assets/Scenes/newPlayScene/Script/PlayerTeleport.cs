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

    private void Start()
    {
        // PlayerControllerを取得
        playerController = GetComponent<PlayerController>();

        // リストが空でないかを確認
        if (targetObjects.Count == 0 || targetColliders.Count == 0 || targetMeshRenderers.Count == 0)
        {
            Debug.LogError("ターゲットオブジェクトのリストが設定されていません！");
        }
    }

    private void Update()
    {
        if (targetObjects.Count == 0) return;

        // プレイヤーとターゲットの距離を計算
        foreach (Transform targetObject in targetObjects)
        {
            float distance = Vector3.Distance(transform.position, targetObject.position);

            // 距離が範囲内かつFire2ボタンが押された場合
            if (distance <= triggerRange && Input.GetButtonDown(fireButton))
            {
                if (isEmperor)
                {
                    // エンペラー状態を解除
                    ExitEmperorState();
                }
                else
                {
                    // エンペラー状態にする
                    EnterEmperorState();
                    TeleportToTarget(targetObject);
                    DisableTriggerObject(targetObject);
                }
            }
        }
    }

    private void EnterEmperorState()
    {
        // プレイヤーを動けなくし、タグを変更する
        playerController.LockPlayerActions();
        playerController.ChangePlayerTag("not_Player"); // タグをnot_Playerに変更
        isEmperor = true; // エンペラー状態に設定

        // トリガーオブジェクトのisTriggerをtrueにする
        for (int i = 0; i < targetColliders.Count; i++)
        {
            if (targetColliders[i] != null)
            {
                targetColliders[i].isTrigger = true; // ColliderのisTriggerをtrueに設定
            }
        }

        Debug.Log("プレイヤーはエンペラー状態になり、動けなくなりました！");
    }

    private void ExitEmperorState()
    {
        // プレイヤーの動きをアンロックし、タグを元に戻す
        playerController.UnlockPlayerActions();
        playerController.ChangePlayerTag("Player"); // タグをPlayerに戻す
        isEmperor = false; // エンペラー状態を解除

        // トリガーオブジェクトのColliderとMeshRendererを有効化
        for (int i = 0; i < targetColliders.Count; i++)
        {
            if (targetColliders[i] != null)
            {
                targetColliders[i].isTrigger = false; // ColliderのisTriggerをfalseに戻す
                targetColliders[i].enabled = true; // Colliderを再度有効化
            }

            if (targetMeshRenderers[i] != null)
            {
                targetMeshRenderers[i].enabled = true; // MeshRendererを有効化
            }
        }

        Debug.Log("エンペラー状態が解除され、プレイヤーは動けるようになりました！");
    }

    private void TeleportToTarget(Transform targetObject)
    {
        Vector3 targetPosition = targetObject.position;

        // 衝突チェック：ターゲット位置が壁の中にないかチェック
        if (IsPositionBlocked(targetPosition))
        {
            // 衝突している場合、少し前に移動させる（または他の適切な調整方法）
            targetPosition = GetSafePosition(targetPosition);
            Debug.Log("プレイヤーが壁に衝突したため、位置を調整しました！");
        }

        // プレイヤーをターゲットの位置に移動
        transform.position = targetPosition;
        Debug.Log("プレイヤーが移動しました！ 新しい位置: " + transform.position);
    }

    private bool IsPositionBlocked(Vector3 position)
    {
        // 衝突チェック：指定した位置から少し下に向けてRaycastを行う
        RaycastHit hit;
        if (Physics.Raycast(position, Vector3.down, out hit, collisionCheckDistance))
        {
            // 衝突した場合はtrueを返す
            return true;
        }
        return false;
    }

    private Vector3 GetSafePosition(Vector3 targetPosition)
    {
        // 位置を調整するために前方に少し移動させる
        Vector3 safePosition = targetPosition + transform.forward * collisionCheckDistance;

        // もう一度位置が安全か確認
        if (!IsPositionBlocked(safePosition))
        {
            return safePosition;
        }

        // 再度チェックして安全な位置を見つけるロジック
        return targetPosition; // もしそれでも問題があれば元の位置を返す
    }

    private void DisableTriggerObject(Transform targetObject)
    {
        // 対象のColliderとMeshRendererを無効化
        int index = targetObjects.IndexOf(targetObject);

        if (index >= 0 && index < targetColliders.Count)
        {
            // Collider自体を無効化する
            if (targetColliders[index] != null)
            {
                targetColliders[index].enabled = false; // Colliderを無効化
                targetColliders[index].isTrigger = false; // isTriggerも無効にする
            }

            // MeshRendererを非表示にする
            if (targetMeshRenderers[index] != null)
            {
                targetMeshRenderers[index].enabled = false; // MeshRendererを非表示にする
            }
        }

        Debug.Log("トリガーオブジェクトの機能を無効化しました！");
    }

    private void OnDrawGizmosSelected()
    {
        // トリガー範囲をSceneビューで視覚化（範囲が分かるように）
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, triggerRange);
    }
}
