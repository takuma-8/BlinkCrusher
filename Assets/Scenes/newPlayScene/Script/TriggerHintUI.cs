using UnityEngine;
using UnityEngine.UI;

public class TriggerHintUI : MonoBehaviour
{
    public Image hintImage; // �q���g�p��UI�摜
    public Sprite showHintSprite; // �uB�{�^���ŏo��v�̉摜
    public Sprite hideHintSprite; // �uB�{�^���ŉB���v�̉摜

    private PlayerTeleport playerTeleport; // �e�I�u�W�F�N�g�ɂ���PlayerTeleport�ւ̎Q��
    private bool previousStateIsEmperor;
    private bool previousStateInRange;

    private void Start()
    {
        // �e�I�u�W�F�N�g����PlayerTeleport���擾
        playerTeleport = GetComponentInParent<PlayerTeleport>();

        if (hintImage == null)
        {
            Debug.LogError("HintImage���ݒ肳��Ă��܂���IInspector��UI Image��ݒ肵�Ă��������B");
            enabled = false;
            return;
        }

        if (playerTeleport == null)
        {
            Debug.LogError("PlayerTeleport���e�I�u�W�F�N�g�Ɍ�����܂���I�X�N���v�g���m�F���Ă��������B");
            enabled = false;
            return;
        }

        hintImage.enabled = false; // ������Ԃł͔�\��
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
                hintImage.sprite = showHintSprite; // �uB�{�^���ŏo��v�̉摜�ɕύX
                hintImage.enabled = true; // �摜��\��
            }
            else if (isInRange)
            {
                hintImage.sprite = hideHintSprite; // �uB�{�^���ŉB���v�̉摜�ɕύX
                hintImage.enabled = true; // �摜��\��
            }
            else
            {
                hintImage.enabled = false; // �摜���\��
            }

            previousStateIsEmperor = isEmperor;
            previousStateInRange = isInRange;
        }
    }
}
