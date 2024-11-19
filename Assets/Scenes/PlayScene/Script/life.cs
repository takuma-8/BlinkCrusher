using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class life : MonoBehaviour
{
    public GameObject[] lifeArray = new GameObject[3]; // 最大3つのハート
    private int lifePoint = 3; // ハートの残り数
    private bool isStunned = false; // スタン状態のフラグ

    public Material normalMaterial; // 通常時のマテリアル
    public Material blinkMaterial; // 点滅時のマテリアル

    private Renderer playerRenderer; // プレイヤーのRenderer

    void Start()
    {
        playerRenderer = GetComponent<Renderer>();

        if (playerRenderer == null)
        {
            Debug.LogError("Renderer が見つかりません。スクリプトが正しいオブジェクトにアタッチされているか確認してください。");
        }
        UpdateLifeUI(); // 最初にハートをUIに反映
    }

    void OnTriggerEnter(Collider other)
    {
        // Playerタグを持つオブジェクトか確認
        if (gameObject.CompareTag("Player"))
        {
            // Zornタグのオブジェクトに触れた場合
            if (other.gameObject.CompareTag("Zorn") && !isStunned)
            {
                StartCoroutine(HandleStun()); // スタン処理を開始
            }
        }
    }

    // スタン処理
    private IEnumerator HandleStun()
    {
        isStunned = true; // スタン状態
        Debug.Log("スタン状態開始");

        // ハートを減らす
        if (lifePoint > 0)
        {
            lifePoint--;
            UpdateLifeUI(); // ハートのUIを更新
        }

        // 残機がゼロならゲームオーバー処理
        if (lifePoint <= 0)
        {
            TriggerGameOver(); // ゲームオーバー処理
            yield break; // 処理を中断
        }

        // 点滅処理を開始
        StartCoroutine(BlinkEffect());

        // 8秒間スタン状態を維持
        yield return new WaitForSeconds(5f);

        isStunned = false; // スタン解除
        if (playerRenderer != null)
        {
            playerRenderer.material = normalMaterial; // マテリアルを通常に戻す
        }
        Debug.Log("スタン解除");
    }

    // 点滅処理
    private IEnumerator BlinkEffect()
    {
        if (playerRenderer == null || blinkMaterial == null || normalMaterial == null)
        {
            Debug.LogError("BlinkEffect: Renderer or materials are not set properly.");
            yield break;
        }

        bool isBlinking = false;

        while (isStunned)
        {
            // 点滅状態を切り替え
            playerRenderer.material = isBlinking ? normalMaterial : blinkMaterial;
            isBlinking = !isBlinking;

            // 点滅間隔を設定
            yield return new WaitForSeconds(0.3f);
        }

        // スタンが終了したら通常のマテリアルに戻す
        playerRenderer.material = normalMaterial;
    }

    // UIのハート表示を更新
    private void UpdateLifeUI()
    {
        for (int i = 0; i < lifeArray.Length; i++)
        {
            if (i < lifePoint)
            {
                lifeArray[i].SetActive(true); // ハートを表示
            }
            else
            {
                lifeArray[i].SetActive(false); // ハートを非表示
            }
        }
    }

    // ゲームオーバー時にシーン遷移
    private void TriggerGameOver()
    {
        Debug.Log("ゲームオーバー");
        SceneManager.LoadScene("ResultScene"); // ゲームオーバーシーンへ遷移
    }
}
