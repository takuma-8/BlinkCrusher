using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Life : MonoBehaviour
{
    public Image gameOverImage;  // Inspectorで設定
    public float displayTime = 2.0f;  // 画像表示時間
    private bool isGameOver = false;  // ゲームオーバー判定

    private void Start()
    {
        // 初期状態で画像を非表示にする
        gameOverImage.gameObject.SetActive(false);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (isGameOver) return;  // 既にゲームオーバーなら処理しない

        if (gameObject.CompareTag("Player"))
        {
            if (other.gameObject.CompareTag("Enemy2") || other.gameObject.CompareTag("Enemy1"))
            {
                StartCoroutine(TriggerGameOver());
            }
        }
    }

    private IEnumerator TriggerGameOver()
    {
        // 全ての効果音を消す
        AudioListener.pause = true;

        isGameOver = true;  // ゲームオーバーフラグを立てる
        Debug.Log("ゲームオーバー");

        // ゲームオーバーの映像を表示（映像再生用オブジェクトを有効化する場合はここで）
        gameOverImage.gameObject.SetActive(true);

        // プレイヤーの操作を無効化（PlayerController がある場合）
        PlayerController playerController = GetComponent<PlayerController>();
        if (playerController != null)
        {
            playerController.enabled = false;  // プレイヤーのスクリプトを無効化
        }

        // UIボタンなどの操作をブロック
        DisableAllUIInteractions();

        // 必要ならゲームをフリーズ（映像が影響を受けるなら削除）
        // Time.timeScale = 0;

        // 120フレーム待機
        for (int i = 0; i < 120; i++)
        {
            yield return new WaitForEndOfFrame();
        }

        // シーン遷移時は通常の時間に戻す
        Time.timeScale = 1;
        FadeManager.Instance.LoadScene("ResultScene", 1.0f);
    }



    private void DisableAllUIInteractions()
    {
        // すべてのボタンを取得して無効化
        Button[] buttons = FindObjectsOfType<Button>();
        foreach (Button btn in buttons)
        {
            btn.interactable = false;
        }
    }
}
