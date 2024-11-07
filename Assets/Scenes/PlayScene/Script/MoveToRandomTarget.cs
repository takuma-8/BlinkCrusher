using UnityEngine;
using UnityEngine.AI;

public class MoveToRandomTarget : MonoBehaviour
{
    public float range = 10.0f; // ランダム範囲の半径
    public float minDistance = 5.0f; // 最小距離
    private NavMeshAgent agent;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        SetRandomDestination(); // ランダムな目的地を設定
    }

    void Update()
    {
        // オブジェクトがターゲットに向かっている間、移動方向に正面を向かせる
        if (agent.velocity.sqrMagnitude > Mathf.Epsilon)
        {
            transform.rotation = Quaternion.LookRotation(agent.velocity.normalized);
        }

        // エージェントが目標地点に到達しているか確認し、再設定
        if (!agent.pathPending && agent.remainingDistance < 0.5f)
        {
            SetRandomDestination(); // 目標に到達後、再度ランダムな場所を設定
        }
    }

    // ランダムな場所に目的地を設定するメソッド
    void SetRandomDestination()
    {
        Vector3 randomDirection;
        NavMeshHit hit;
        bool validPosition = false;

        // 有効な目的地が見つかるまで繰り返す
        do
        {
            randomDirection = Random.insideUnitSphere * range; // ランダムな方向を取得
            randomDirection += transform.position; // 現在位置を基準にする

            // NavMesh上でランダムポイントを見つけ、有効な場合はtrueが返る
            if (NavMesh.SamplePosition(randomDirection, out hit, range, NavMesh.AllAreas))
            {
                // 最小距離を満たしているか確認
                if (Vector3.Distance(transform.position, hit.position) >= minDistance)
                {
                    agent.SetDestination(hit.position);
                    validPosition = true;
                }
            }
        } while (!validPosition); // 有効な目的地が見つかるまでループ
    }
}
