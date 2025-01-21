using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Movement Parameters")]
    public float moveSpeed = 5.0f; // 通常の移動速度
    public float maxDistanceToWall = 0.5f; // 壁との距離の閾値
    public string wallTag = "Wall";

    private Rigidbody rb;
    private bool isActionLocked = false; // 操作ロックフラグ

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        if (isActionLocked) return; // 操作ロック中は何もしない

        // Xboxコントローラーの右スティックで移動
        float horizontal = Input.GetAxis("Horizontal"); // 右スティックの左右
        float vertical = Input.GetAxis("Vertical"); // 右スティックの上下

        // プレイヤーの移動
        Vector3 direction = transform.forward * vertical + transform.right * horizontal;
        direction = direction.normalized;

        // 壁チェック
        if (!IsWallInFront(direction))
        {
            rb.MovePosition(rb.position + direction * moveSpeed * Time.deltaTime);
        }
    }

    /// <summary>
    /// 壁が目の前にあるかどうかをチェック
    /// </summary>
    /// <param name="direction">進行方向</param>
    /// <returns>壁がある場合はtrue</returns>
    bool IsWallInFront(Vector3 direction)
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, direction, out hit, maxDistanceToWall))
        {
            if (hit.collider.CompareTag(wallTag))
            {
                Debug.Log("Wall detected!");
                return true;
            }
        }
        return false;
    }

    public void LockPlayerActions()
    {
        isActionLocked = true;
        rb.isKinematic = true; // 動きを完全に停止
    }

    public void UnlockPlayerActions()
    {
        isActionLocked = false;
        rb.isKinematic = false; // 動作を再開
    }

    // プレイヤーのタグを変更
    public void ChangePlayerTag(string newTag)
    {
        gameObject.tag = newTag;
    }
}
