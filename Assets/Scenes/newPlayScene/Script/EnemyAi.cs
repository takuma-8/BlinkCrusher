using System.Collections;
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
    private bool isChasing = false;    // エネミーがプレイヤーを追跡しているかどうか
    private float timeSinceLastSeen = 0f;  // プレイヤーを見失ってから経過した時間

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player")?.transform;  // Playerタグを持つオブジェクトを探す
        if (player == null)
        {
            Debug.LogWarning("Playerタグを持つオブジェクトが見つかりません！");
            return;  // Playerが見つからなければ追跡しない
        }
        navAgent = GetComponent<NavMeshAgent>();  // NavMeshAgentコンポーネントを取得
        navAgent.speed = speed;  // エネミーの移動速度を設定
    }

    void Update()
    {
        if (player == null)
            return;  // Playerが存在しない場合、処理を中断

        // プレイヤーが"Player"タグを持っているかチェック
        if (!player.CompareTag("Player"))
            return;  // "Player"タグがない場合、追跡しない

        // プレイヤーとの距離を計算
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        // プレイヤーが追跡範囲に入った場合
        if (distanceToPlayer <= chaseRange)
        {
            // プレイヤーが範囲内にいる場合、追跡を開始
            if (distanceToPlayer <= detectionRange)
            {
                if (!isChasing)
                {
                    isChasing = true;
                    timeSinceLastSeen = 0f;  // 見失った時間をリセット
                }
                navAgent.SetDestination(player.position);  // プレイヤーの位置に向かって移動
            }
        }
        else
        {
            // 追跡範囲外の場合、通常動作に戻る
            if (isChasing)
            {
                isChasing = false;
                navAgent.ResetPath();  // 追跡を停止
            }
        }

        // プレイヤーが範囲から出てから2秒経過したら見失ったとみなして通常動作に戻る
        if (isChasing)
        {
            timeSinceLastSeen += Time.deltaTime;
            if (timeSinceLastSeen >= loseSightTime)
            {
                isChasing = false;
                navAgent.ResetPath();  // 見失って通常動作に戻る
            }
        }
    }
}
