using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class life : MonoBehaviour
{
    void OnTriggerEnter(Collider other)
    {
        // Player�^�O�����I�u�W�F�N�g���m�F
        if (gameObject.CompareTag("Player"))
        {
            // Zorn�^�O�̃I�u�W�F�N�g�ɐG�ꂽ�ꍇ
            if (other.gameObject.CompareTag("Enemy2") || other.gameObject.CompareTag("Enemy1"))
            {
                TriggerGameOver();
            }
        }
    }

    // �Q�[���I�[�o�[���ɃV�[���J��
    private void TriggerGameOver()
    {
        Debug.Log("�Q�[���I�[�o�[");
        FadeManager.Instance.LoadScene("ResultScene", 1.0f);
    }
}
