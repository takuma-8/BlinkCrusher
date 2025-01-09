using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Movement Parameters")]
    public float moveSpeed = 5.0f; // 通常の移動速度
    public float crouchSpeed = 3.0f; // しゃがみ時の移動速度
    public float maxDistanceToWall = 0.5f; // 壁との距離の閾値
    public string wallTag = "Wall";
    public bool isCrouching { get; private set; } // 外部から読み取り可能だが、内部でのみ変更可能
    private bool canStandUp = true;  // しゃがみ解除可能か

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

        // プレイヤーの移動速度設定
        float currentSpeed = isCrouching ? crouchSpeed : moveSpeed;

        // プレイヤーの移動
        Vector3 direction = transform.forward * vertical + transform.right * horizontal;
        direction = direction.normalized;

        // 壁チェック
        if (!IsWallInFront(direction))
        {
            rb.MovePosition(rb.position + direction * currentSpeed * Time.deltaTime);
        }

        // しゃがみ入力 (例: Cキーでしゃがむ/解除)
        if (Input.GetKeyDown(KeyCode.C) || Input.GetButtonDown("Fire2"))
        {
            if (!isCrouching)
            {
                Crouch(); // しゃがむ
            }
            else if (canStandUp) // しゃがみ解除が許可されている場合のみ実行
            {
                StandUp(); // しゃがみ解除
            }
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

    private void Crouch()
    {
        isCrouching = true;
        transform.localScale = new Vector3(1f, 0.5f, 1f); // プレイヤーの高さを半分にする
        transform.position = new Vector3(transform.position.x, 0.5f, transform.position.z); // y座標を0.5に設定
        Debug.Log("しゃがんだ");
    }

    private void StandUp()
    {
        isCrouching = false;
        transform.localScale = new Vector3(1f, 1f, 1f); // プレイヤーの高さを元に戻す
        transform.position = new Vector3(transform.position.x, 1f, transform.position.z); // y座標を1に戻す
        Debug.Log("立ち上がった");
    }

    // しゃがみ解除不可エリアに入った場合
    public void SetCrouchRestriction(bool restriction)
    {
        canStandUp = !restriction;
        Debug.Log(restriction ? "しゃがみ解除不可エリアに入った" : "しゃがみ解除不可エリアから出た");
    }
}