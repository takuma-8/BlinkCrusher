using System.Collections;
using UnityEngine;
using UnityEngine.AI; // NavMeshAgent を使用する場合

public class EnemyController : MonoBehaviour
{
    public enum EnemyType { Enemy1, Enemy2 };
    public EnemyType enemyType;

    public GameObject visionObject;       // 視界オブジェクト
    private bool isStunned = false;
    private NavMeshAgent agent;          // NavMeshAgent を使用する場合
    private Renderer enemyRenderer;      // 敵のマテリアルを切り替えるための Renderer

    private Vector3 wanderTarget;
    private bool isWandering = false;


    private void Start()
    {
        // NavMeshAgent を取得
        agent = GetComponent<NavMeshAgent>();

        // Renderer を取得
        enemyRenderer = GetComponent<Renderer>();

        if (enemyRenderer == null)
        {
            Debug.LogError("Enemy does not have a Renderer component!");
        }
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
