using UnityEngine;
using UnityEngine.UI;

public class StarRating : MonoBehaviour
{
    public Image[] stars; // 空白の星（5つ）のImageコンポーネントをInspectorでセット
    public Sprite fullStarSprite; // 色付きの星のスプライト

    private void Start()
    {
        // スコアを取得
        int score = DestroyObjectInFront.GetScore();
        Debug.Log("スコア: " + score); // デバッグログでスコアを確認

        // 星のランクを反映
        SetStarRating(score);
    }

    // ランク更新
    private void SetStarRating(int score)
    {
        int starCount = GetStarCount(score); // スコアに応じた星の数を取得

        for (int i = 0; i < stars.Length; i++)
        {
            if (i < starCount)
            {
                stars[i].sprite = fullStarSprite; // 色付きの星に変更
            }
        }
    }

    private int GetStarCount(int score)
    {
        if (score >= 10000) return 5; // S
        if (score >= 8000) return 4;  // A
        if (score >= 5000) return 3;  // B
        if (score >= 3000) return 2;  // C
        return 1; // D（スコア0も含む）
    }
}
