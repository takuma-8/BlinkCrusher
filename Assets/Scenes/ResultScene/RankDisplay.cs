using TMPro;
using UnityEngine;

public class RankDisplay : MonoBehaviour
{
    public TMP_Text rankText;  // ランク表示用の TextMeshPro

    private void Start()
    {
        
        // PlayerPrefs からスコアを取得してランクを表示
        int score = DestroyObjectInFront.GetScore();
        Debug.Log("スコア: " + score); // スコアが正しく取得されているか確認

        // ランク更新
        UpdateRank(score);
    }

    public void UpdateRank(int score)
    {
        string rank = CalculateRank(score);
        rankText.text = rank;
    }

    private string CalculateRank(int score)
    {
        if (score >= 10000) return "S";
        if (score >= 8000) return "A";
        if (score >= 5000) return "B";
        if (score >= 3000) return "C";

        return "D";
    }
}
