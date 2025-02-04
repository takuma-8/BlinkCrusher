using TMPro;
using UnityEngine;

public class RankDisplay : MonoBehaviour
{
    public TMP_Text rankText;  // �����N�\���p�� TextMeshPro

    private void Start()
    {
        
        // PlayerPrefs ����X�R�A���擾���ă����N��\��
        int score = DestroyObjectInFront.GetScore();
        Debug.Log("�X�R�A: " + score); // �X�R�A���������擾����Ă��邩�m�F

        // �����N�X�V
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
