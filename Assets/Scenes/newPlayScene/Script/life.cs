using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Life : MonoBehaviour
{
    public Image gameOverImage;  // Inspectorで設定
    public float displayTime = 2.0f;  // 画像表示時間

    private void Start()
    {
        // 初期状態で画像を非表示にする
        gameOverImage.gameObject.SetActive(false);
    }

    private void OnTriggerEnter(Collider other)
    {
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
        Debug.Log("ゲームオーバー");

        // ゲームオーバーの画像を表示
        gameOverImage.gameObject.SetActive(true);

        // 画像を表示して一定時間待機
        yield return new WaitForSeconds(displayTime);

        // シーン遷移
        FadeManager.Instance.LoadScene("ResultScene", 1.0f);
    }
}
