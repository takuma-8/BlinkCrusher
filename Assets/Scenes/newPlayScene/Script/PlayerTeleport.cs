using UnityEngine;
using System.Collections.Generic;

public class PlayerTeleport : MonoBehaviour
{
    public List<Transform> targetObjects; // �����̃g���K�[�I�u�W�F�N�g��Transform
    public List<Collider> targetColliders; // �����̃g���K�[�I�u�W�F�N�g��Collider
    public List<MeshRenderer> targetMeshRenderers; // �����̃g���K�[�I�u�W�F�N�g��MeshRenderer
    public string fireButton = "Fire2"; // �R���g���[���[��Fire2�{�^��
    public float triggerRange = 5f; // �g���K�[�͈͂̔��a�i�f�t�H���g5�j
    public float collisionCheckDistance = 1f; // �Փ˃`�F�b�N�̋����i�f�t�H���g1�j

    private PlayerController playerController; // �v���C���[��Controller
    private bool isEmperor = false; // �v���C���[���G���y���[��Ԃ��ǂ������Ǘ�

    private void Start()
    {
        // PlayerController���擾
        playerController = GetComponent<PlayerController>();

        // ���X�g����łȂ������m�F
        if (targetObjects.Count == 0 || targetColliders.Count == 0 || targetMeshRenderers.Count == 0)
        {
            Debug.LogError("�^�[�Q�b�g�I�u�W�F�N�g�̃��X�g���ݒ肳��Ă��܂���I");
        }
    }

    private void Update()
    {
        if (targetObjects.Count == 0) return;

        // �v���C���[�ƃ^�[�Q�b�g�̋������v�Z
        foreach (Transform targetObject in targetObjects)
        {
            float distance = Vector3.Distance(transform.position, targetObject.position);

            // �������͈͓�����Fire2�{�^���������ꂽ�ꍇ
            if (distance <= triggerRange && Input.GetButtonDown(fireButton))
            {
                if (isEmperor)
                {
                    // �G���y���[��Ԃ�����
                    ExitEmperorState();
                }
                else
                {
                    // �G���y���[��Ԃɂ���
                    EnterEmperorState();
                    TeleportToTarget(targetObject);
                    DisableTriggerObject(targetObject);
                }
            }
        }
    }

    private void EnterEmperorState()
    {
        // �v���C���[�𓮂��Ȃ����A�^�O��ύX����
        playerController.LockPlayerActions();
        playerController.ChangePlayerTag("not_Player"); // �^�O��not_Player�ɕύX
        isEmperor = true; // �G���y���[��Ԃɐݒ�

        // �g���K�[�I�u�W�F�N�g��isTrigger��true�ɂ���
        for (int i = 0; i < targetColliders.Count; i++)
        {
            if (targetColliders[i] != null)
            {
                targetColliders[i].isTrigger = true; // Collider��isTrigger��true�ɐݒ�
            }
        }

        Debug.Log("�v���C���[�̓G���y���[��ԂɂȂ�A�����Ȃ��Ȃ�܂����I");
    }

    private void ExitEmperorState()
    {
        // �v���C���[�̓������A�����b�N���A�^�O�����ɖ߂�
        playerController.UnlockPlayerActions();
        playerController.ChangePlayerTag("Player"); // �^�O��Player�ɖ߂�
        isEmperor = false; // �G���y���[��Ԃ�����

        // �g���K�[�I�u�W�F�N�g��Collider��MeshRenderer��L����
        for (int i = 0; i < targetColliders.Count; i++)
        {
            if (targetColliders[i] != null)
            {
                targetColliders[i].isTrigger = false; // Collider��isTrigger��false�ɖ߂�
                targetColliders[i].enabled = true; // Collider���ēx�L����
            }

            if (targetMeshRenderers[i] != null)
            {
                targetMeshRenderers[i].enabled = true; // MeshRenderer��L����
            }
        }

        Debug.Log("�G���y���[��Ԃ���������A�v���C���[�͓�����悤�ɂȂ�܂����I");
    }

    private void TeleportToTarget(Transform targetObject)
    {
        Vector3 targetPosition = targetObject.position;

        // �Փ˃`�F�b�N�F�^�[�Q�b�g�ʒu���ǂ̒��ɂȂ����`�F�b�N
        if (IsPositionBlocked(targetPosition))
        {
            // �Փ˂��Ă���ꍇ�A�����O�Ɉړ�������i�܂��͑��̓K�؂Ȓ������@�j
            targetPosition = GetSafePosition(targetPosition);
            Debug.Log("�v���C���[���ǂɏՓ˂������߁A�ʒu�𒲐����܂����I");
        }

        // �v���C���[���^�[�Q�b�g�̈ʒu�Ɉړ�
        transform.position = targetPosition;
        Debug.Log("�v���C���[���ړ����܂����I �V�����ʒu: " + transform.position);
    }

    private bool IsPositionBlocked(Vector3 position)
    {
        // �Փ˃`�F�b�N�F�w�肵���ʒu���班�����Ɍ�����Raycast���s��
        RaycastHit hit;
        if (Physics.Raycast(position, Vector3.down, out hit, collisionCheckDistance))
        {
            // �Փ˂����ꍇ��true��Ԃ�
            return true;
        }
        return false;
    }

    private Vector3 GetSafePosition(Vector3 targetPosition)
    {
        // �ʒu�𒲐����邽�߂ɑO���ɏ����ړ�������
        Vector3 safePosition = targetPosition + transform.forward * collisionCheckDistance;

        // ������x�ʒu�����S���m�F
        if (!IsPositionBlocked(safePosition))
        {
            return safePosition;
        }

        // �ēx�`�F�b�N���Ĉ��S�Ȉʒu�������郍�W�b�N
        return targetPosition; // ��������ł���肪����Ό��̈ʒu��Ԃ�
    }

    private void DisableTriggerObject(Transform targetObject)
    {
        // �Ώۂ�Collider��MeshRenderer�𖳌���
        int index = targetObjects.IndexOf(targetObject);

        if (index >= 0 && index < targetColliders.Count)
        {
            // Collider���̂𖳌�������
            if (targetColliders[index] != null)
            {
                targetColliders[index].enabled = false; // Collider�𖳌���
                targetColliders[index].isTrigger = false; // isTrigger�������ɂ���
            }

            // MeshRenderer���\���ɂ���
            if (targetMeshRenderers[index] != null)
            {
                targetMeshRenderers[index].enabled = false; // MeshRenderer���\���ɂ���
            }
        }

        Debug.Log("�g���K�[�I�u�W�F�N�g�̋@�\�𖳌������܂����I");
    }

    private void OnDrawGizmosSelected()
    {
        // �g���K�[�͈͂�Scene�r���[�Ŏ��o���i�͈͂�������悤�Ɂj
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, triggerRange);
    }
}
