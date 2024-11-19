using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class life : MonoBehaviour
{
    public GameObject[] lifeArray = new GameObject[3]; // �ő�3�̃n�[�g
    private int lifePoint = 3; // �n�[�g�̎c�萔
    private bool isStunned = false; // �X�^����Ԃ̃t���O

    public Material normalMaterial; // �ʏ펞�̃}�e���A��
    public Material blinkMaterial; // �_�Ŏ��̃}�e���A��

    private Renderer playerRenderer; // �v���C���[��Renderer

    void Start()
    {
        playerRenderer = GetComponent<Renderer>();

        if (playerRenderer == null)
        {
            Debug.LogError("Renderer ��������܂���B�X�N���v�g���������I�u�W�F�N�g�ɃA�^�b�`����Ă��邩�m�F���Ă��������B");
        }
        UpdateLifeUI(); // �ŏ��Ƀn�[�g��UI�ɔ��f
    }

    void OnTriggerEnter(Collider other)
    {
        // Player�^�O�����I�u�W�F�N�g���m�F
        if (gameObject.CompareTag("Player"))
        {
            // Zorn�^�O�̃I�u�W�F�N�g�ɐG�ꂽ�ꍇ
            if (other.gameObject.CompareTag("Zorn") && !isStunned)
            {
                StartCoroutine(HandleStun()); // �X�^���������J�n
            }
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

        // �c�@���[���Ȃ�Q�[���I�[�o�[����
        if (lifePoint <= 0)
        {
            TriggerGameOver(); // �Q�[���I�[�o�[����
            yield break; // �����𒆒f
        }

        // �_�ŏ������J�n
        StartCoroutine(BlinkEffect());

        // 8�b�ԃX�^����Ԃ��ێ�
        yield return new WaitForSeconds(5f);

        isStunned = false; // �X�^������
        if (playerRenderer != null)
        {
            playerRenderer.material = normalMaterial; // �}�e���A����ʏ�ɖ߂�
        }
        Debug.Log("�X�^������");
    }

    // �_�ŏ���
    private IEnumerator BlinkEffect()
    {
        if (playerRenderer == null || blinkMaterial == null || normalMaterial == null)
        {
            Debug.LogError("BlinkEffect: Renderer or materials are not set properly.");
            yield break;
        }

        bool isBlinking = false;

        while (isStunned)
        {
            // �_�ŏ�Ԃ�؂�ւ�
            playerRenderer.material = isBlinking ? normalMaterial : blinkMaterial;
            isBlinking = !isBlinking;

            // �_�ŊԊu��ݒ�
            yield return new WaitForSeconds(0.3f);
        }

        // �X�^�����I��������ʏ�̃}�e���A���ɖ߂�
        playerRenderer.material = normalMaterial;
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
        SceneManager.LoadScene("ResultScene"); // �Q�[���I�[�o�[�V�[���֑J��
    }
}
