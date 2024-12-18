using System.Collections;
using UnityEngine;
using UnityEngine.AI; // NavMeshAgent を使用する場合

public class EnemyController : MonoBehaviour
{
    public enum EnemyType { Enemy1, Enemy2 };
    public EnemyType enemyType;

    public GameObject visionObject;       // 視界オブジェクト
    public float stunnedDuration = 10f;  // 気絶時間
    public Material blinkMaterial;       // 点滅時のマテリアル（必須）
    public Material normalMaterial;      // 通常時のマテリアル（必須）

    private bool isStunned = false;
    private NavMeshAgent agent;          // NavMeshAgent を使用する場合
    private Renderer enemyRenderer;      // 敵のマテリアルを切り替えるための Renderer

    // Enemy2用のパラメータ
    public float lifetime = 10.0f; // 表示される時間（秒）
    private bool isTemporary; // Enemy2かどうかを判定
    private bool isDisappearing = false; // 消える動作中か判定

    [SerializeField]
    private Vector3 disappearPosition = new Vector3(44.0f, 1.0f, 20.0f); // Enemy2が消える位置

    private Vector3 wanderTarget;
    private bool isWandering = false;


    private void Start()
    {
        disappearPosition = new Vector3(44.0f, 1.0f, 20.0f);

        // NavMeshAgent を取得
        agent = GetComponent<NavMeshAgent>();

        // Renderer を取得
        enemyRenderer = GetComponent<Renderer>();

        if (enemyRenderer == null)
        {
            Debug.LogError("Enemy does not have a Renderer component!");
        }

        if(enemyType == EnemyType.Enemy1) 
        {
            InitializeEnemy1();
        }
        else if(enemyType == EnemyType.Enemy2)
        {
            InitializeEnemy2();
        }
    }

    private void InitializeEnemy1()
    {
        Debug.Log("Enemy1 initialized.");
        // Enemy1専用の初期化処理（必要なら追加）
    }

    private void InitializeEnemy2()
    {
        Debug.Log("Enemy2 initialized with lifetime.");
        Debug.Log($"Disappear position is set to: {disappearPosition}"); // デバッグ用ログ
        isTemporary = true;
        StartCoroutine(DeactivateAfterLifetime(lifetime));
    }

    private IEnumerator DeactivateAfterLifetime(float time)
    {
        agent.stoppingDistance = 1.0f;
        yield return new WaitForSeconds(time);

        if (isDisappearing) yield break;

        isDisappearing = true;

        if (agent != null)
        {
            // 当たり判定を無効化
            Collider collider = GetComponent<Collider>();
            if (collider != null) collider.enabled = false;

            // NavMeshAgent を使用して移動
            agent.isStopped = false;
            agent.SetDestination(disappearPosition);

            // 移動中のデバッグ
            Debug.Log($"{gameObject.name} is moving to disappear position: {disappearPosition}");

            // 所定の位置に到達するまで待機
            while (agent.pathPending || agent.remainingDistance > agent.stoppingDistance)
            {
                Debug.Log($"Remaining Distance: {agent.remainingDistance}, Path Status: {agent.pathStatus}");
                yield return null;
            }
        }
        else
        {
            transform.position = disappearPosition;
            Debug.Log($"Enemy moved directly to: {disappearPosition}");
        }

        // 到着後にオブジェクトを削除
        Debug.Log($"{gameObject.name} has disappeared at position {disappearPosition}.");
        Destroy(gameObject);
    }

    public void Stun()
    {
        if (isStunned) return; // 既に気絶中なら処理しない

        isStunned = true;

        // デバッグメッセージ
        Debug.Log($"{gameObject.name} is stunned for {stunnedDuration} seconds!");

        // 視界オブジェクトを非アクティブ化
        if (visionObject != null)
        {
            visionObject.SetActive(false);
        }

        // NavMeshAgent を停止する（もし使用していれば）
        if (agent != null)
        {
            agent.isStopped = true;
        }

        // 点滅処理を開始
        StartCoroutine(BlinkEffect());

        // 気絶状態を再現
        StartCoroutine(StunnedCoroutine());
    }

    private IEnumerator StunnedCoroutine()
    {
        // 気絶中は動作しない
        yield return new WaitForSeconds(stunnedDuration);

        // 気絶解除
        isStunned = false;

        // 視界オブジェクトを再度アクティブ化
        if (visionObject != null)
        {
            visionObject.SetActive(true);
        }

        // NavMeshAgent を再開する（もし使用していれば）
        if (agent != null)
        {
            agent.isStopped = false;
        }

        // 通常状態のマテリアルに戻す
        if (enemyRenderer != null && normalMaterial != null)
        {
            enemyRenderer.material = normalMaterial;  // 正しいマテリアルを設定
        }

        // デバッグメッセージ
        Debug.Log($"{gameObject.name} has recovered from being stunned.");
    }

    private IEnumerator BlinkEffect()
    {
        if (enemyRenderer == null || blinkMaterial == null || normalMaterial == null)
        {
            Debug.LogError("BlinkEffect: Renderer or materials are not set properly.");
            yield break;
        }

        float elapsedTime = 0f;
        bool isBlinking = false;

        while (isStunned)
        {
            enemyRenderer.material = isBlinking ? normalMaterial : blinkMaterial;
            isBlinking = !isBlinking;

            // 点滅間隔を設定
            yield return new WaitForSeconds(0.3f);

            elapsedTime += 0.3f;
        }

        // 気絶が終わったら通常のマテリアルに戻す
        enemyRenderer.material = normalMaterial;
    }
    public void WanderForSeconds(Vector3 origin, float duration)
    {
        if (agent == null || isStunned) return;

        StartCoroutine(WanderCoroutine(origin, duration));
    }

    private IEnumerator WanderCoroutine(Vector3 origin, float duration)
    {
        float elapsed = 0f;

        while (elapsed < duration)
        {
            // ランダムなポイントを生成
            Vector3 randomPoint = origin + new Vector3(
                Random.Range(-2.5f, 2.5f),
                0,
                Random.Range(-2.5f, 2.5f)
            );

            // NavMesh 上の有効な位置に変換
            if (NavMesh.SamplePosition(randomPoint, out NavMeshHit hit, 2.5f, NavMesh.AllAreas))
            {
                agent.SetDestination(hit.position);
            }

            // 次の移動まで少し待つ
            yield return new WaitForSeconds(1f);

            elapsed += 1f;
        }

        // うろつき終了後、停止させる（必要なら調整）
        agent.SetDestination(transform.position);
    }
    //public void MoveToPositionAndStop(Vector3 targetPosition)
    //{
    //    if (agent == null || isStunned) return;

    //    // NavMeshAgentで指定位置に移動
    //    agent.SetDestination(targetPosition);

    //    // 停止をチェックするコルーチンを開始
    //    StartCoroutine(StopAtTargetCoroutine(targetPosition));
    //}

    public void MoveToPositionAndWander(Vector3 targetPosition, float wanderDuration = 5f, float wanderRadius = 2f)
    {
        if (agent == null || isStunned) return;

        ////色を変更
        //if (enemyRenderer != null)
        //{
        //    enemyRenderer.material.color = Color.red;
        //}

        // NavMeshAgentで指定位置に移動
        agent.SetDestination(targetPosition);

        StartCoroutine(WanderAfterReachingTarget(targetPosition, wanderDuration, wanderRadius));
    }

    private IEnumerator WanderAfterReachingTarget(Vector3 targetPosition, float wanderDuration, float wanderRadius)
    {
        while (agent.pathPending || agent.remainingDistance > agent.stoppingDistance)
        {
            Debug.Log($"{gameObject.name} Remaining Distance: {agent.remainingDistance}, Stopping Distance: {agent.stoppingDistance}");
            yield return null;
        }


        Debug.Log($"{gameObject.name} has reached position {targetPosition}");

        // うろうろを開始
        float elapsedTime = 0f;
        while (elapsedTime < wanderDuration)
        {
            Vector3 randomPoint = targetPosition + new Vector3(
                Random.Range(-wanderRadius, wanderRadius),
                0,
                Random.Range(-wanderRadius, wanderRadius)
            );

            // NavMesh 上の有効な位置を確認
            if (NavMesh.SamplePosition(randomPoint, out NavMeshHit hit, wanderRadius, NavMesh.AllAreas))
            {
                agent.SetDestination(hit.position);
                Debug.Log($"{gameObject.name} is wandering to {hit.position}");
            }
            else
            {
                Debug.LogWarning($"Failed to find valid NavMesh position near {randomPoint}");
            }


            // 次の移動まで待機
            float wanderInterval = Random.Range(1f, 2f);
            yield return new WaitForSeconds(wanderInterval);

            elapsedTime += wanderInterval;

            // 進行状況のデバッグ
            Debug.Log($"{gameObject.name} has been wandering for {elapsedTime} seconds.");
        }

        Debug.Log($"{gameObject.name} finished wandering near {targetPosition}");
    }



    private IEnumerator StopAtTargetCoroutine(Vector3 targetPosition)
    {
        // 到達するまで待つ
        while (agent.pathPending || agent.remainingDistance > agent.stoppingDistance)
        {
            yield return null;
        }

        // 到達後、NavMeshAgentを停止
        agent.isStopped = true;

        // デバッグ用ログ
        Debug.Log($"{gameObject.name} has stopped at position {targetPosition}");
    }
}
