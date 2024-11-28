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
    public float lifetime = 10f; // 表示される時間（秒）
    private bool isTemporary; // Enemy2かどうかを判定

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
        isTemporary = true;
        StartCoroutine(DeactivateAfterLifetime(lifetime));
    }

    private IEnumerator DeactivateAfterLifetime(float time)
    {
        yield return new WaitForSeconds(time);
        gameObject.SetActive(false); // 一時的に非アクティブ化
        Debug.Log($"{gameObject.name} has disappeared after {time} seconds.");
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
            // 点滅状態を切り替え
            enemyRenderer.material = isBlinking ? normalMaterial : blinkMaterial;
            isBlinking = !isBlinking;

            // 点滅間隔を設定
            yield return new WaitForSeconds(0.3f);

            elapsedTime += 0.3f;
        }

        // 気絶が終わったら通常のマテリアルに戻す
        enemyRenderer.material = normalMaterial;
    }
}
