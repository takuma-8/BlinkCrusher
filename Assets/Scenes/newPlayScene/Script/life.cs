using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Life : MonoBehaviour
{
    public Image gameOverImage;  // Inspector�Őݒ�
    public float displayTime = 2.0f;  // �摜�\������
    private bool isGameOver = false;  // �Q�[���I�[�o�[����

    private void Start()
    {
        // ������Ԃŉ摜���\���ɂ���
        gameOverImage.gameObject.SetActive(false);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (isGameOver) return;  // ���ɃQ�[���I�[�o�[�Ȃ珈�����Ȃ�

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

        isGameOver = true;  // �Q�[���I�[�o�[�t���O�𗧂Ă�
        Debug.Log("�Q�[���I�[�o�[");

        // �Q�[���I�[�o�[�̉f����\���i�f���Đ��p�I�u�W�F�N�g��L��������ꍇ�͂����Łj
        gameOverImage.gameObject.SetActive(true);

        // �v���C���[�̑���𖳌����iPlayerController ������ꍇ�j
        PlayerController playerController = GetComponent<PlayerController>();
        if (playerController != null)
        {
            playerController.enabled = false;  // �v���C���[�̃X�N���v�g�𖳌���
        }

        // UI�{�^���Ȃǂ̑�����u���b�N
        DisableAllUIInteractions();

        // �K�v�Ȃ�Q�[�����t���[�Y�i�f�����e�����󂯂�Ȃ�폜�j
        // Time.timeScale = 0;

        // 120�t���[���ҋ@
        for (int i = 0; i < 120; i++)
        {
            yield return new WaitForEndOfFrame();
        }

        // �V�[���J�ڎ��͒ʏ�̎��Ԃɖ߂�
        Time.timeScale = 1;
        FadeManager.Instance.LoadScene("ResultScene", 1.0f);
    }



    private void DisableAllUIInteractions()
    {
        // ���ׂẴ{�^�����擾���Ė�����
        Button[] buttons = FindObjectsOfType<Button>();
        foreach (Button btn in buttons)
        {
            btn.interactable = false;
        }
    }
}
