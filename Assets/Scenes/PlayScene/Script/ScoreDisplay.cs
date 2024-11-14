using UnityEngine;
using UnityEngine.UI;

public class ScoreDisplay : MonoBehaviour
{
    public Text scoreText;  // �X�R�A�\���p��UI Text
    private int score;      // ���݂̃X�R�A

    void Start()
    {
        // �X�R�A��������
        score = DestroyObjectInFront.GetScore();
        UpdateScoreDisplay();
    }

    void Update()
    {
        // �X�R�A���ω������ꍇ�AUI���X�V
        int newScore = DestroyObjectInFront.GetScore();
        if (newScore != score)
        {
            score = newScore;
            UpdateScoreDisplay();
        }
    }

    void UpdateScoreDisplay()
    {
        // �X�R�A����ʂɕ\��
        scoreText.text = "Score: " + score.ToString();
    }
}
