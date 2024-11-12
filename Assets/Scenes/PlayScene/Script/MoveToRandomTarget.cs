using UnityEngine;
using UnityEngine.AI;

public class MoveToRandomTarget : MonoBehaviour
{
    public float minRange = 5.0f; // 最低範囲
    public float maxRange = 10.0f; // 最高範囲
    private NavMeshAgent agent;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        SetRandomDestination(); // ランダムな目的地を設定
    }

    void Update()
    {
        // オブジェクトが移動中に、移動方向に正面を向かせる
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
        float randomDistance = Random.Range(minRange, maxRange); // 最低と最高範囲の間でランダムな距離を生成
        Vector3 randomDirection = Random.insideUnitSphere.normalized * randomDistance; // ランダムな方向と距離
        randomDirection += transform.position; // 現在位置を基準にする

        NavMeshHit hit;
        // NavMesh上でランダムポイントを見つける
        if (NavMesh.SamplePosition(randomDirection, out hit, maxRange, NavMesh.AllAreas))
        {
            agent.SetDestination(hit.position);
        }
    }
}
