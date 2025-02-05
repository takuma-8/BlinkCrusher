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

        // ゲームオーバーの画像を表示
        gameOverImage.gameObject.SetActive(true);

        // プレイヤーの操作を無効化（PlayerController がある場合）
        PlayerController playerController = GetComponent<PlayerController>();
        if (playerController != null)
        {
            playerController.enabled = false;  // プレイヤーのスクリプトを無効化
        }

        // UIボタンなどの操作をブロック
        DisableAllUIInteractions();

        // ゲームをフリーズ
        Time.timeScale = 0;

        // 画像を表示して一定時間待機（リアルタイム時間を基準に）
        yield return new WaitForSecondsRealtime(displayTime);

        Time.timeScale = 1; // シーン遷移時は通常の時間に戻す
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
