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

    void Awake()
    {
        isAnyEnemyChasing = false; // シーンをまたいだらリセット
        soundManager = GetComponent<EnemySoundManager>();
        if (soundManager == null)
        {
            Debug.LogError("EnemySoundManager がアタッチされていません！");
        }
    }

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

    void StopChaseIfNoEnemies()
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
            Debug.Log("StopChaseEnd() を実行！（遅延処理）");
        }
    }

    void Update()
    {
        if (player == null) return;
        if (!player.CompareTag("Player")) return;

        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        if (distanceToPlayer <= chaseRange)
        {
            if (distanceToPlayer <= detectionRange)
            {
                if (!isChasing)
                {
                    isChasing = true;
                    timeSinceLastSeen = 0f;
                    soundManager.SetChaseMode(true);

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
                navAgent.ResetPath();
                soundManager.SetChaseMode(false);

                // **一定時間後にチェックする**
                Invoke(nameof(StopChaseIfNoEnemies), 0.2f);
            }
        }

        if (isChasing)
        {
            timeSinceLastSeen += Time.deltaTime;
            if (timeSinceLastSeen >= loseSightTime)
            {
                isChasing = false;
                navAgent.ResetPath();
                soundManager.SetChaseMode(false);

                // **一定時間後にチェックする**
                Invoke(nameof(StopChaseIfNoEnemies), 0.2f);
            }
        }

        float moveSpeed = navAgent.velocity.magnitude;
        animator.SetFloat("Speed", moveSpeed);
    }
}
