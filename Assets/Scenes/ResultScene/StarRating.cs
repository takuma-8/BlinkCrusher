using UnityEngine;
using UnityEngine.UI;

public class StarRating : MonoBehaviour
{
    public Image[] stars; // �󔒂̐��i5�j��Image�R���|�[�l���g��Inspector�ŃZ�b�g
    public Sprite fullStarSprite; // �F�t���̐��̃X�v���C�g

    private void Start()
    {
        // �X�R�A���擾
        int score = DestroyObjectInFront.GetScore();
        Debug.Log("�X�R�A: " + score); // �f�o�b�O���O�ŃX�R�A���m�F

        // ���̃����N�𔽉f
        SetStarRating(score);
    }

    // �����N�X�V
    private void SetStarRating(int score)
    {
        int starCount = GetStarCount(score); // �X�R�A�ɉ��������̐����擾

        for (int i = 0; i < stars.Length; i++)
        {
            if (i < starCount)
            {
                stars[i].sprite = fullStarSprite; // �F�t���̐��ɕύX
            }
        }
    }

    private int GetStarCount(int score)
    {
        if (score >= 10000) return 5; // S
        if (score >= 8000) return 4;  // A
        if (score >= 5000) return 3;  // B
        if (score >= 3000) return 2;  // C
        return 1; // D�i�X�R�A0���܂ށj
    }
}
