using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;
using UnityEngine.SceneManagement;

public class Life : MonoBehaviour
{
    public VideoPlayer videoPlayer;  // Inspector で設定（VideoPlayer コンポーネント）
    public RawImage raiRawImage;  // RawImageに変更
    public float videoDelayFrames = 120;  // 動画再生後の待機フレーム
    private bool isGameOver = false;

    private void Start()
    {
        videoPlayer.gameObject.SetActive(false);  // 最初は非表示
        if (raiRawImage != null)
        {
            raiRawImage.gameObject.SetActive(false);  // 最初は非表示に設定
        }
        AudioListener.pause = false;
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
        isGameOver = true;
        Debug.Log("ゲームオーバー");

        // 動画を表示＆再生
        videoPlayer.gameObject.SetActive(true);
        videoPlayer.Play();

        // RawImageも表示
        if (raiRawImage != null)
        {
            raiRawImage.gameObject.SetActive(true);
        }

        // プレイヤー操作を無効化
        PlayerController playerController = GetComponent<PlayerController>();
        if (playerController != null)
        {
            playerController.enabled = false;
        }

        // UIボタンを無効化
        DisableAllUIInteractions();

        // ゲームをフリーズ（時間を止める）
        Time.timeScale = 0;

        // 120フレーム待機
        for (int i = 0; i < videoDelayFrames; i++)
        {
            yield return new WaitForEndOfFrame();
        }

        // シーン遷移前に時間を元に戻す
        Time.timeScale = 1;

        // シーン遷移
        FadeManager.Instance.LoadScene("ResultScene", 1.0f);
    }


    private void DisableAllUIInteractions()
    {
        Button[] buttons = FindObjectsOfType<Button>();
        foreach (Button btn in buttons)
        {
            btn.interactable = false;
        }
    }
}
