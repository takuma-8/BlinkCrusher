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

    private void Start()
    {
        playerController = GetComponent<PlayerController>();

        if (targetObjects.Count == 0 || targetColliders.Count == 0 || targetMeshRenderers.Count == 0)
        {
            Debug.LogError("ターゲットオブジェクトのリストが設定されていません！");
        }
    }

    private void Update()
    {
        if (targetObjects.Count == 0) return;

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
                    originalPosition = transform.position; // 現在位置を記録
                    TeleportToTarget(targetObject);
                    DisableTriggerObject(targetObject);
                }
            }
        }
    }

    private void EnterEmperorState()
    {
        playerController.LockPlayerActions();
        playerController.ChangePlayerTag("not_Player");
        isEmperor = true;

        for (int i = 0; i < targetColliders.Count; i++)
        {
            if (targetColliders[i] != null)
            {
                targetColliders[i].isTrigger = true;
            }
        }

        Debug.Log("プレイヤーはエンペラー状態になり、動けなくなりました！");
    }

    private void ExitEmperorState()
    {
        playerController.UnlockPlayerActions();
        playerController.ChangePlayerTag("Player");
        isEmperor = false;

        transform.position = originalPosition; // 元の位置に戻す

        for (int i = 0; i < targetColliders.Count; i++)
        {
            if (targetColliders[i] != null)
            {
                targetColliders[i].isTrigger = false;
                targetColliders[i].enabled = true;
            }

            if (targetMeshRenderers[i] != null)
            {
                targetMeshRenderers[i].enabled = true;
            }
        }

        Debug.Log("エンペラー状態が解除され、プレイヤーは元の位置に戻りました！");
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

    private bool IsPositionBlocked(Vector3 position)
    {
        RaycastHit hit;
        if (Physics.Raycast(position, Vector3.down, out hit, collisionCheckDistance))
        {
            return true;
        }
        return false;
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
}
