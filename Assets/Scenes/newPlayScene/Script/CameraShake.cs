using UnityEngine;

public class CameraShake : MonoBehaviour
{
    public Transform player; // プレイヤーのTransform
    public float shakeAmount = 0.1f; // 揺れの大きさ
    public float shakeSpeed = 5f; // 揺れの速さ

    private Vector3 originalPosition; // カメラの元の位置
    private float shakeTimer = 0f; // 揺れ用のタイマー

    void Start()
    {
        // カメラの初期位置を記録
        originalPosition = transform.localPosition;
    }

    void Update()
    {
        // プレイヤーが移動中かどうかを確認（例: プレイヤーの速度が一定以上か）
        if (IsPlayerMoving())
        {
            // 揺れを計算
            shakeTimer += Time.deltaTime * shakeSpeed;
            float offsetX = Mathf.Sin(shakeTimer) * shakeAmount;
            float offsetY = Mathf.Cos(shakeTimer) * shakeAmount * 0.5f; // 横揺れより縦揺れを小さく
            transform.localPosition = originalPosition + new Vector3(offsetX, offsetY, 0);
        }
        else
        {
            // プレイヤーが停止中の場合、カメラを元の位置に戻す
            transform.localPosition = Vector3.Lerp(transform.localPosition, originalPosition, Time.deltaTime * 10f);
            shakeTimer = 0f;
        }
    }

    // プレイヤーが移動中かどうかを判定
    private bool IsPlayerMoving()
    {
        if (player.TryGetComponent(out Rigidbody rb))
        {
            return rb.velocity.magnitude > 0.1f; // 移動していると判断する速度の閾値
        }
        else if (player.TryGetComponent(out CharacterController controller))
        {
            return controller.velocity.magnitude > 0.1f; // CharacterControllerを使っている場合
        }

        // プレイヤーの移動状態を別の基準でチェックする場合はここを変更
        return false;
    }
}
