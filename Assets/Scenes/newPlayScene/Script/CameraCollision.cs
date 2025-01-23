using UnityEngine;

public class CameraCollision : MonoBehaviour
{
    public Transform player; // プレイヤーのTransform
    public Transform cameraTransform; // カメラのTransform
    public float cameraRadius = 0.5f; // カメラのコリジョン用の半径
    public float smoothSpeed = 10f; // カメラ移動のスムーズさ
    public LayerMask collisionLayer; // 壁や障害物を検出するためのレイヤー

    private Vector3 defaultCameraOffset; // カメラの初期オフセット
    private Vector3 currentCameraPosition; // 現在のカメラ位置

    private void Start()
    {
        if (player == null || cameraTransform == null)
        {
            Debug.LogError("プレイヤーまたはカメラのTransformが設定されていません！");
            enabled = false;
            return;
        }

        // 初期カメラオフセットを保存
        defaultCameraOffset = cameraTransform.position - player.position;
        currentCameraPosition = cameraTransform.position;
    }

    private void LateUpdate()
    {
        Vector3 desiredCameraPosition = player.position + defaultCameraOffset; // 理想的なカメラ位置
        Vector3 direction = desiredCameraPosition - player.position; // プレイヤーからカメラへの方向

        // カメラの衝突をチェック
        if (Physics.SphereCast(player.position, cameraRadius, direction, out RaycastHit hit, direction.magnitude, collisionLayer))
        {
            // 衝突時、カメラを壁の手前に配置
            currentCameraPosition = hit.point - direction.normalized * cameraRadius;
        }
        else
        {
            // 衝突がない場合は、通常の位置に戻す
            currentCameraPosition = desiredCameraPosition;
        }

        // カメラをスムーズに移動
        cameraTransform.position = Vector3.Lerp(cameraTransform.position, currentCameraPosition, Time.deltaTime * smoothSpeed);
        cameraTransform.LookAt(player); // カメラがプレイヤーを見るようにする
    }

    private void OnDrawGizmos()
    {
        if (player != null && cameraTransform != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawLine(player.position, cameraTransform.position);
            Gizmos.DrawWireSphere(cameraTransform.position, cameraRadius);
        }
    }
}