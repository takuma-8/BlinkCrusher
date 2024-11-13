using UnityEngine;
using UnityEngine.UI;

public class CountdownTimer : MonoBehaviour
{
    public Text countdownText;  // Textコンポーネントをインスペクターで指定
    private float timeRemaining = 151f;  // カウントダウン時間（150秒）

    void Update()
    {
        if (timeRemaining > 0)
        {
            // deltaTimeをTime.deltaTimeとして使用
            timeRemaining -= Time.deltaTime;
            int seconds = Mathf.FloorToInt(timeRemaining);
            countdownText.text = "残り時間：" + seconds.ToString() + "秒";
        }
        else
        {
            countdownText.text = "残り時間：0秒";
            // 必要に応じてカウントダウン終了時の処理を追加
        }
    }
}
