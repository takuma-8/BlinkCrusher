using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class life : MonoBehaviour
{
    void OnTriggerEnter(Collider other)
    {
        // Playerタグを持つオブジェクトか確認
        if (gameObject.CompareTag("Player"))
        {
            // Zornタグのオブジェクトに触れた場合
            if (other.gameObject.CompareTag("Enemy2") || other.gameObject.CompareTag("Enemy1"))
            {
                TriggerGameOver();
            }
        }
    }

    // ゲームオーバー時にシーン遷移
    private void TriggerGameOver()
    {
        Debug.Log("ゲームオーバー");
        FadeManager.Instance.LoadScene("ResultScene", 1.0f);
    }
}
