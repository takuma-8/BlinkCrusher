using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;
using UnityEngine.SceneManagement;

public class Life : MonoBehaviour
{
    public VideoPlayer videoPlayer;  // Inspector �Őݒ�iVideoPlayer �R���|�[�l���g�j
    public RawImage raiRawImage;  // RawImage�ɕύX
    public float videoDelayFrames = 120;  // ����Đ���̑ҋ@�t���[��
    private bool isGameOver = false;

    private void Start()
    {
        videoPlayer.gameObject.SetActive(false);  // �ŏ��͔�\��
        if (raiRawImage != null)
        {
            raiRawImage.gameObject.SetActive(false);  // �ŏ��͔�\���ɐݒ�
        }
        AudioListener.pause = false;
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
        isGameOver = true;
        Debug.Log("�Q�[���I�[�o�[");

        // �����\�����Đ�
        videoPlayer.gameObject.SetActive(true);
        videoPlayer.Play();

        // RawImage���\��
        if (raiRawImage != null)
        {
            raiRawImage.gameObject.SetActive(true);
        }

        // �v���C���[����𖳌���
        PlayerController playerController = GetComponent<PlayerController>();
        if (playerController != null)
        {
            playerController.enabled = false;
        }

        // UI�{�^���𖳌���
        DisableAllUIInteractions();

        // �Q�[�����t���[�Y�i���Ԃ��~�߂�j
        Time.timeScale = 0;

        // 120�t���[���ҋ@
        for (int i = 0; i < videoDelayFrames; i++)
        {
            yield return new WaitForEndOfFrame();
        }

        // �V�[���J�ڑO�Ɏ��Ԃ����ɖ߂�
        Time.timeScale = 1;

        // �V�[���J��
        FadeManager.Instance.LoadScene("ResultScene", 1.0f);
    }


    private void DisableAllUIInteractions()
    {
        Button[] buttons = FindObjectsOfType<Button>();
        foreach (Button btn in buttons)
        {
            btn.interactable = false;
        }
    }
}
