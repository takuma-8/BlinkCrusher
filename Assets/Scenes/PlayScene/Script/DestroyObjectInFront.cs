using System.Collections;
using UnityEngine;

public class DestroyObjectInFront : MonoBehaviour
{
    public float detectionRadius = 1.0f;  // 検出する半径
    public string targetTag1 = "Target1"; // ターゲット1のタグ
    public string targetTag2 = "Target2"; // ターゲット2のタグ
    private ObjectSpawner objectSpawner;   // ObjectSpawnerの参照
    public static int score = 0;          // 共有スコア変数

    public GameObject enemy2Prefab;        // Enemy2のPrefab
    private GameObject enemy2Instance;     // 生成されたEnemy2のインスタンス

    void Start()
    {
        // ObjectSpawnerのインスタンスを取得
        objectSpawner = FindObjectOfType<ObjectSpawner>();

        // Enemy2Prefabを最初は非アクティブにしておく
        if (enemy2Prefab != null)
        {
            enemy2Prefab.SetActive(false);
        }
    }

    void Update()
    {
        // スペースキーまたはコントローラーBボタンが押されたとき
        if (Input.GetKeyDown(KeyCode.J) || Input.GetButtonDown("Fire1"))
        {
            DetectAndDestroy();
        }
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
                    score += 100;  // Target1には100ポイント
                }
                else if (collider.CompareTag(targetTag2))
                {
                    score += 500;   // Target2には500ポイント
                    StartCoroutine(SpawnEnemy2WithDelay());
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

    public static void ResetScore()
    {
        score = 0;
    }

    void SpawnEnemy2()
{
    // もしすでにEnemy2が出現していたら、もう生成しない
    if (enemy2Instance != null)
    {
        Debug.Log("Enemy2 is already spawned.");
        return;  // すでに出現していたら何もしない
    }

    // Enemy2を指定の位置に生成
    if (enemy2Prefab != null)
    {
        Vector3 spawnPosition = new Vector3(44.0f, 1.0f, -2.0f);  // 初期位置 (1.0, 1.0, 16.0)
        enemy2Instance = Instantiate(enemy2Prefab, spawnPosition, Quaternion.identity);
        enemy2Instance.SetActive(true);  // 表示されるようにする

        // Debugログ
        Debug.Log($"Enemy2 spawned at {spawnPosition}");

        // Enemy2のEnemyControllerに設定
        var enemyController = enemy2Instance.GetComponent<EnemyController>();
        if (enemyController != null)
        {
            enemyController.enemyType = EnemyController.EnemyType.Enemy2;
            Debug.Log("Enemy2 initialized with lifetime.");
        }
        else
        {
            Debug.LogError("Enemy2 prefab is missing EnemyController!");
        }
    }
    else
    {
        Debug.LogError("Enemy2 Prefab is not assigned!");
    }
}

   /*
    // Enemy2を出現させるメソッド
    void SpawnEnemy2()
    {
        // もしすでにEnemy2が出現していたら、もう生成しない
        //if (enemy2Instance != null)
        //{
        //    Debug.Log("Enemy2 is already spawned.");
        //    return;  // すでに出現していたら何もしない
        //}

        // Enemy2を指定の位置に生成
        if (enemy2Prefab != null)
        {
            Vector3 spawnPosition = new Vector3(44.0f, 1.0f, -2.0f);  // 初期位置 (1.0, 1.0, 16.0)
            enemy2Instance = Instantiate(enemy2Prefab, spawnPosition, Quaternion.identity);
            enemy2Instance.SetActive(true);  // 表示されるようにする

            // Debugログ
            Debug.Log($"Enemy2 spawned at {spawnPosition}");

            // Enemy2のEnemyControllerに設定
            var enemyController = enemy2Instance.GetComponent<EnemyController>();
            if (enemyController != null)
            {
                enemyController.enemyType = EnemyController.EnemyType.Enemy2;
                Debug.Log("Enemy2 initialized with lifetime.");
            }
            else
            {
                Debug.LogError("Enemy2 prefab is missing EnemyController!");
            }
        }
        else
        {
            Debug.LogError("Enemy2 Prefab is not assigned!");
        }
    }
   */
    private IEnumerator SpawnEnemy2WithDelay()
    {
        yield return new WaitForSeconds(2.0f);
        Debug.Log("二秒経ちました Enemy2 が出現します");
        SpawnEnemy2();
    }
    
}
