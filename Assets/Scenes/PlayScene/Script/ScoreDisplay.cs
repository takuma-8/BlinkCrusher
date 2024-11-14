using UnityEngine;
using UnityEngine.UI;

public class ScoreDisplay : MonoBehaviour
{
    public Text scoreText;  // スコア表示用のUI Text
    private int score;      // 現在のスコア

    void Start()
    {
        // スコアを初期化
        score = DestroyObjectInFront.GetScore();
        UpdateScoreDisplay();
    }

    void Update()
    {
        // スコアが変化した場合、UIを更新
        int newScore = DestroyObjectInFront.GetScore();
        if (newScore != score)
        {
            score = newScore;
            UpdateScoreDisplay();
        }
    }

    void UpdateScoreDisplay()
    {
        // スコアを画面に表示
        scoreText.text = "Score: " + score.ToString();
    }
}
