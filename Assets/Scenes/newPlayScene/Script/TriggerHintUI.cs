using UnityEngine;
using UnityEngine.UI;

public class TriggerHintUI : MonoBehaviour
{
    public Image showHintSprite; // �uB�{�^���ŏo��v�̉摜
    public Image hideHintSprite; // �uB�{�^���ŉB���v�̉摜

    private PlayerTeleport playerTeleport; // �e�I�u�W�F�N�g�ɂ���PlayerTeleport�ւ̎Q��
    private bool previousStateIsEmperor;
    private bool previousStateInRange;

    private void Start()
    {
        // �e�I�u�W�F�N�g����PlayerTeleport���擾
        playerTeleport = GetComponentInParent<PlayerTeleport>();

        if (playerTeleport == null)
        {
            Debug.LogError("PlayerTeleport���e�I�u�W�F�N�g�Ɍ�����܂���I�X�N���v�g���m�F���Ă��������B");
            enabled = false;
            return;
        }

        // ������Ԃ�UI���\���ɂ���
        showHintSprite.enabled = false;
        hideHintSprite.enabled = false;
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
                showHintSprite.enabled = true;
                hideHintSprite.enabled = false;
            }
            else if (isInRange)
            {
                showHintSprite.enabled = false;
                hideHintSprite.enabled = true;
            }
            else
            {
                showHintSprite.enabled = false;
                hideHintSprite.enabled = false;
            }

            previousStateIsEmperor = isEmperor;
            previousStateInRange = isInRange;
        }
    }
}
