using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class BlinkingText : MonoBehaviour
{
    public Text targetText; // UI��Text�R���|�[�l���g

    void Start()
    {
        StartCoroutine(BlinkText());
    }

    IEnumerator BlinkText()
    {
        while (true)
        {
            targetText.enabled = !targetText.enabled; // �\�� / ��\����؂�ւ���
            yield return new WaitForSeconds(1f); // 1�b�҂�
        }
    }
}
