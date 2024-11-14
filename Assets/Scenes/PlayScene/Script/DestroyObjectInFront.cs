using UnityEngine;

public class DestroyObjectInFront : MonoBehaviour
{
    public float detectionRadius = 1.0f;  // 検出する半径
    public string targetTag = "Target";   // 破壊したいオブジェクトのタグ
    private ObjectSpawner objectSpawner;  // ObjectSpawnerのインスタンスを参照

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
        // Playerの正面1メートル先の位置を計算
        Vector3 frontPosition = transform.position + transform.forward * detectionRadius;

        // OverlapSphereで指定範囲内のコライダーを取得
        Collider[] colliders = Physics.OverlapSphere(frontPosition, detectionRadius);

        foreach (Collider collider in colliders)
        {
            if (collider.CompareTag(targetTag))  // タグが一致するオブジェクトを破壊
            {
                // ObjectSpawnerに通知してリストから削除
                if (objectSpawner != null)
                {
                    objectSpawner.RemoveSpawnedObject(collider.gameObject);
                }

                // オブジェクトを破壊
                Destroy(collider.gameObject);
                break;  // 1つだけ破壊する場合はループを終了
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
}
