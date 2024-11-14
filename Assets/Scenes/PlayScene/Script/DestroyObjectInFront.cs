using UnityEngine;

public class DestroyObjectInFront : MonoBehaviour
{
    public float detectionRadius = 1.0f;  // 検出する半径
    public string targetTag1 = "Target1"; // ターゲット1のタグ
    public string targetTag2 = "Target2"; // ターゲット2のタグ
    private ObjectSpawner objectSpawner;   // ObjectSpawnerの参照
    public static int score = 0;          // 共有スコア変数

    void Start()
    {
        // ObjectSpawnerのインスタンスを取得
        objectSpawner = FindObjectOfType<ObjectSpawner>();
    }

    void Update()
    {
        DetectAndDestroy();
    }

    void DetectAndDestroy()
    {
        // プレイヤーの正面1メートル先の位置を計算
        Vector3 frontPosition = transform.position + transform.forward * detectionRadius;

        // OverlapSphereで指定範囲内のコライダーを取得
        Collider[] colliders = Physics.OverlapSphere(frontPosition, detectionRadius);

        foreach (Collider collider in colliders)
        {
            // タグがTarget1またはTarget2の場合に破壊
            if (collider.CompareTag(targetTag1) || collider.CompareTag(targetTag2))
            {
                // ObjectSpawnerに通知してリストから削除
                if (objectSpawner != null)
                {
                    objectSpawner.RemoveSpawnedObject(collider.gameObject);
                }

                // タグに応じてスコアを加算
                if (collider.CompareTag(targetTag1))
                {
                    score += 100;  // Target1には10ポイント
                }
                else if (collider.CompareTag(targetTag2))
                {
                    score += 500;   // Target2には5ポイント
                }

                // オブジェクトを破壊
                Destroy(collider.gameObject);
                break;  // 1つだけ破壊する
            }
        }
    }

    // Gizmosで検出範囲を表示
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Vector3 frontPosition = transform.position + transform.forward * detectionRadius;
        Gizmos.DrawWireSphere(frontPosition, detectionRadius);
    }

    // 現在のスコアを取得するための静的メソッド
    public static int GetScore()
    {
        return score;
    }
}
