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
    private float chaseCooldown = 1.0f; // 追跡音のクールダウン時間
    private float lastChaseEndTime = -100f; // 最後に StopChaseEnd() を呼んだ時間

    private static bool isAnyEnemyChasing = false;

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
                    timeSinceLastSeen = 0f;
                    soundManager.SetChaseMode(true);

                    // 誰も追跡していないなら音を鳴らす
                    if (!isAnyEnemyChasing)
                    {
                        soundManager.PlayChaseStart();
                        isAnyEnemyChasing = true;
                        Debug.Log("PlayChaseStart() を実行！");
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
                soundManager.SetChaseMode(false);

                // 全エネミーが追跡をやめたら音を止める
                if (isAnyEnemyChasing)
                {
                    bool anyStillChasing = false;
                    foreach (EnemyAi enemy in FindObjectsOfType<EnemyAi>())
                    {
                        if (enemy.isChasing)
                        {
                            anyStillChasing = true;
                            break;
                        }
                    }

                    if (!anyStillChasing)
                    {
                        soundManager.StopChaseEnd();
                        isAnyEnemyChasing = false;
                        Debug.Log("StopChaseEnd() を実行！");
                    }
                }
                if (isSoundPlaying && soundManager != null && isAnyEnemyChasing)
                {
                    soundManager.StopChaseEnd();
                    isSoundPlaying = false;
                    lastChaseEndTime = Time.time; // 追跡音の終了時刻を記録
                    Debug.Log("StopChaseEnd() を実行！");
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
                soundManager.SetChaseMode(false);

                // 全エネミーが追跡をやめたら音を止める
                if (isAnyEnemyChasing)
                {
                    bool anyStillChasing = false;
                    foreach (EnemyAi enemy in FindObjectsOfType<EnemyAi>())
                    {
                        if (enemy.isChasing)
                        {
                            anyStillChasing = true;
                            break;
                        }
                    }

                    if (!anyStillChasing)
                    {
                        soundManager.StopChaseEnd();
                        isAnyEnemyChasing = false;
                        Debug.Log("StopChaseEnd() を実行！");
                    }
                }
            }
        }

        // 💡【アニメーション制御】ここが追加部分！
        float moveSpeed = navAgent.velocity.magnitude; // 現在の移動速度
        animator.SetFloat("Speed", moveSpeed);  // Animator の `Speed` パラメータを更新
    }
}
