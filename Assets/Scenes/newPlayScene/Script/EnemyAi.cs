using UnityEngine;
using UnityEngine.AI;

public class EnemyAi : MonoBehaviour
{
    public float detectionRange = 5f;  // プレイヤーを検出する範囲
    public float chaseRange = 10f;     // プレイヤーを追跡する範囲
    public float loseSightTime = 2f;   // プレイヤーが範囲に入らなければ見失うまでの時間
    public float speed = 5f;           // エネミーの移動速度

    private Transform player;          // プレイヤーのTransform
    private NavMeshAgent navAgent;     // エネミーのNavMeshAgent
    private Animator animator;         // アニメーターコンポーネント
    private bool isChasing = false;    // エネミーがプレイヤーを追跡しているかどうか
    private float timeSinceLastSeen = 0f;  // プレイヤーを見失ってから経過した時間

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player")?.transform;  // Playerタグを持つオブジェクトを探す
        if (player == null)
        {
            Debug.LogWarning("Playerタグを持つオブジェクトが見つかりません！");
            return;
        }

        navAgent = GetComponent<NavMeshAgent>();  // NavMeshAgentコンポーネントを取得
        navAgent.speed = speed;  // エネミーの移動速度を設定
        animator = GetComponent<Animator>();  // Animator コンポーネントを取得
    }

    void Update()
    {
        if (player == null) return; // Playerが存在しない場合、処理を中断
        if (!player.CompareTag("Player")) return; // "Player"タグがない場合、追跡しない

        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        // 追跡ロジック
        if (distanceToPlayer <= chaseRange)
        {
            if (distanceToPlayer <= detectionRange)
            {
                if (!isChasing)
                {
                    isChasing = true;
                    timeSinceLastSeen = 0f;  // 見失った時間をリセット
                }
                navAgent.SetDestination(player.position);
            }
        }
        else
        {
            if (isChasing)
            {
                isChasing = false;
                navAgent.ResetPath();  // 追跡を停止
            }
        }

        // 見失い処理
        if (isChasing)
        {
            timeSinceLastSeen += Time.deltaTime;
            if (timeSinceLastSeen >= loseSightTime)
            {
                isChasing = false;
                navAgent.ResetPath();
            }
        }

        // 💡【アニメーション制御】ここが追加部分！
        float moveSpeed = navAgent.velocity.magnitude; // 現在の移動速度
        animator.SetFloat("Speed", moveSpeed);  // Animator の `Speed` パラメータを更新
    }
}
