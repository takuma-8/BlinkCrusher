using UnityEngine;
using UnityEngine.UI;

public class CountdownTimer : MonoBehaviour
{
    public Text countdownText;  // Text�R���|�[�l���g���C���X�y�N�^�[�Ŏw��
    private float timeRemaining = 151f;  // �J�E���g�_�E�����ԁi150�b�j

    void Update()
    {
        if (timeRemaining > 0)
        {
            // deltaTime��Time.deltaTime�Ƃ��Ďg�p
            timeRemaining -= Time.deltaTime;
            int seconds = Mathf.FloorToInt(timeRemaining);
            countdownText.text = "�c�莞�ԁF" + seconds.ToString() + "�b";
        }
        else
        {
            countdownText.text = "�c�莞�ԁF0�b";
            // �K�v�ɉ����ăJ�E���g�_�E���I�����̏�����ǉ�
        }
    }
}
