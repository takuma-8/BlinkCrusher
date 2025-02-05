using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Life : MonoBehaviour
{
    public Image gameOverImage;  // Inspector�Őݒ�
    public float displayTime = 2.0f;  // �摜�\������

    private void Start()
    {
        // ������Ԃŉ摜���\���ɂ���
        gameOverImage.gameObject.SetActive(false);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (gameObject.CompareTag("Player"))
        {
            if (other.gameObject.CompareTag("Enemy2") || other.gameObject.CompareTag("Enemy1"))
            {
                StartCoroutine(TriggerGameOver());
            }
        }
    }

    private IEnumerator TriggerGameOver()
    {
        // �S�Ă̌��ʉ�������
        AudioListener.pause = true;

        Debug.Log("�Q�[���I�[�o�[");

        // �Q�[���I�[�o�[�̉摜��\��
        gameOverImage.gameObject.SetActive(true);

        // �v���C���[�̃^�O��ύX
        gameObject.tag = "not_Player";

        // �Q�[�����t���[�Y
        Time.timeScale = 0;

        // �摜��\�����Ĉ�莞�ԑҋ@�i���A���^�C�����Ԃ���Ɂj
        yield return new WaitForSecondsRealtime(displayTime);
        Time.timeScale = 1;
        // �V�[���J�ځi�t���[�Y���ێ������܂܁j
        FadeManager.Instance.LoadScene("ResultScene", 1.0f);
    }
}
