using UnityEngine;
using UnityEngine.UI;

public class TriggerHintUI : MonoBehaviour
{
    public Text hintText; // �q���g���b�Z�[�W�p��UI�e�L�X�g
    private PlayerTeleport playerTeleport; // �e�I�u�W�F�N�g�ɂ���PlayerTeleport�ւ̎Q��

    private bool previousStateIsEmperor;
    private bool previousStateInRange;

    private void Start()
    {
        // �e�I�u�W�F�N�g����PlayerTeleport���擾
        playerTeleport = GetComponentInParent<PlayerTeleport>();

        if (hintText == null)
        {
            Debug.LogError("HintText���ݒ肳��Ă��܂���IInspector��UI�e�L�X�g��ݒ肵�Ă��������B");
            enabled = false;
            return;
        }

        if (playerTeleport == null)
        {
            Debug.LogError("PlayerTeleport���e�I�u�W�F�N�g�Ɍ�����܂���I�X�N���v�g���m�F���Ă��������B");
            enabled = false;
            return;
        }

        hintText.gameObject.SetActive(false); // ������Ԃł͔�\��
    }

    private void Update()
    {
        if (playerTeleport == null || playerTeleport.targetObjects == null || playerTeleport.targetObjects.Count == 0)
        {
            return;
        }

        bool isInRange = false;
        bool isEmperor = playerTeleport.IsEmperor();

        // �g���K�[�͈͂��`�F�b�N
        foreach (Transform triggerObject in playerTeleport.targetObjects)
        {
            if (Vector3.Distance(playerTeleport.transform.position, triggerObject.position) <= playerTeleport.triggerRange)
            {
                isInRange = true;
                break;
            }
        }

        // ��Ԃ��ω������ꍇ�̂� UI ���X�V
        if (isEmperor != previousStateIsEmperor || isInRange != previousStateInRange)
        {
            if (isEmperor)
            {
                hintText.text = "B�{�^���ŏo��"; // �B��Ă����Ԃ̃��b�Z�[�W
                hintText.gameObject.SetActive(true);
            }
            else if (isInRange)
            {
                hintText.text = "B�{�^���ŉB���"; // �ʏ펞�̃��b�Z�[�W
                hintText.gameObject.SetActive(true);
            }
            else
            {
                hintText.gameObject.SetActive(false); // ��\��
            }

            previousStateIsEmperor = isEmperor;
            previousStateInRange = isInRange;
        }
    }
}