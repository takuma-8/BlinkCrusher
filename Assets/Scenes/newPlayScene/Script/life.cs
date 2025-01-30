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
        Debug.Log("�Q�[���I�[�o�[");

        // �Q�[���I�[�o�[�̉摜��\��
        gameOverImage.gameObject.SetActive(true);

        // �摜��\�����Ĉ�莞�ԑҋ@
        yield return new WaitForSeconds(displayTime);

        // �V�[���J��
        FadeManager.Instance.LoadScene("ResultScene", 1.0f);
    }
}
