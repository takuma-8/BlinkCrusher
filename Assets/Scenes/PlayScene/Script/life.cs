using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class life : MonoBehaviour
{
    public GameObject[] lifeArray = new GameObject[3]; // �ő�3�̃n�[�g
    private int lifePoint = 3; // �n�[�g�̎c�萔
    private bool isStunned = false; // �X�^����Ԃ̃t���O

    void Start()
    {
        UpdateLifeUI(); // �ŏ��Ƀn�[�g��UI�ɔ��f
    }

    void OnTriggerEnter(Collider other)
    {
        // zorn�Ƃ����^�O���t����ꂽ�I�u�W�F�N�g�ɐG�ꂽ�ꍇ
        if (other.gameObject.CompareTag("Zorn") && !isStunned)
        {
            StartCoroutine(HandleStun()); // �X�^���������J�n
        }
    }

    // �X�^������
    private IEnumerator HandleStun()
    {
        isStunned = true; // �X�^�����
        Debug.Log("�X�^����ԊJ�n");

        // �n�[�g�����炷
        if (lifePoint > 0)
        {
            lifePoint--;
            UpdateLifeUI(); // �n�[�g��UI���X�V
        }

        // Check if lifePoint is 0, and if so, trigger scene transition
        if (lifePoint <= 0)
        {
            TriggerGameOver(); // �Q�[���I�[�o�[����
        }

        // 2�b�ԃX�^����Ԃ��ێ�
        yield return new WaitForSeconds(2f);

        isStunned = false; // �X�^������
        Debug.Log("�X�^������");
    }

    // UI�̃n�[�g�\�����X�V
    private void UpdateLifeUI()
    {
        for (int i = 0; i < lifeArray.Length; i++)
        {
            if (i < lifePoint)
            {
                lifeArray[i].SetActive(true); // �n�[�g��\��
            }
            else
            {
                lifeArray[i].SetActive(false); // �n�[�g���\��
            }
        }
    }

    // �Q�[���I�[�o�[���ɃV�[���J��
    private void TriggerGameOver()
    {
        Debug.Log("�Q�[���I�[�o�[");
        // �����ŃQ�[���I�[�o�[��ʂ⎟�̃V�[���ɑJ�ڂ��鏈��������
        // �Ⴆ�΁A"GameOver"�Ƃ����V�[���ɑJ�ڂ���ꍇ
        SceneManager.LoadScene("ResultScene"); // �Q�[���I�[�o�[�V�[���֑J��
    }
}
