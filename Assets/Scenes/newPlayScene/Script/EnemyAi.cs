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

    private EnemySoundManager soundManager;
    private bool isSoundPlaying = false; // 追跡中の音が再生されているかを判定する

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player")?.transform;  // Playerタグを持つオブジェクトを探す
        if (player == null)
        {
            Debug.LogWarning("Playerタグを持つオブジェクトが見つかりません！");
            return;
        }

        soundManager = GetComponent<EnemySoundManager>();

        if (soundManager == null)
        {
            Debug.LogError("EnemySoundManager がアタッチされていません！");
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

                    if (!soundManager.chaseSource.isPlaying) // isSoundPlaying ではなく AudioSource を直接チェック
                    {
                        soundManager.PlayChaseStart();
                    }
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

                // 追跡終了時に音を止める
                if (isSoundPlaying && soundManager != null)
                {
                    soundManager.StopChaseEnd();
                    isSoundPlaying = false; // フラグをリセット
                }
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

                // 追跡終了時に音を止める（見失った場合）
                if (isSoundPlaying && soundManager != null)
                {
                    soundManager.StopChaseEnd();
                    isSoundPlaying = false;
                }
            }
        }

        // 💡【アニメーション制御】ここが追加部分！
        float moveSpeed = navAgent.velocity.magnitude; // 現在の移動速度
        animator.SetFloat("Speed", moveSpeed);  // Animator の `Speed` パラメータを更新
    }
}
