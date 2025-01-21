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
    private Vector3 originalPosition; // �G���y���[��ԑO�̃v���C���[�ʒu

    private void Start()
    {
        playerController = GetComponent<PlayerController>();

        if (targetObjects.Count == 0 || targetColliders.Count == 0 || targetMeshRenderers.Count == 0)
        {
            Debug.LogError("�^�[�Q�b�g�I�u�W�F�N�g�̃��X�g���ݒ肳��Ă��܂���I");
        }
    }

    private void Update()
    {
        if (targetObjects.Count == 0) return;

        foreach (Transform targetObject in targetObjects)
        {
            float distance = Vector3.Distance(transform.position, targetObject.position);

            if (distance <= triggerRange && Input.GetButtonDown(fireButton))
            {
                if (isEmperor)
                {
                    ExitEmperorState();
                }
                else
                {
                    EnterEmperorState();
                    originalPosition = transform.position; // ���݈ʒu���L�^
                    TeleportToTarget(targetObject);
                    DisableTriggerObject(targetObject);
                }
            }
        }
    }

    private void EnterEmperorState()
    {
        playerController.LockPlayerActions();
        playerController.ChangePlayerTag("not_Player");
        isEmperor = true;

        for (int i = 0; i < targetColliders.Count; i++)
        {
            if (targetColliders[i] != null)
            {
                targetColliders[i].isTrigger = true;
            }
        }

        Debug.Log("�v���C���[�̓G���y���[��ԂɂȂ�A�����Ȃ��Ȃ�܂����I");
    }

    private void ExitEmperorState()
    {
        playerController.UnlockPlayerActions();
        playerController.ChangePlayerTag("Player");
        isEmperor = false;

        transform.position = originalPosition; // ���̈ʒu�ɖ߂�

        for (int i = 0; i < targetColliders.Count; i++)
        {
            if (targetColliders[i] != null)
            {
                targetColliders[i].isTrigger = false;
                targetColliders[i].enabled = true;
            }

            if (targetMeshRenderers[i] != null)
            {
                targetMeshRenderers[i].enabled = true;
            }
        }

        Debug.Log("�G���y���[��Ԃ���������A�v���C���[�͌��̈ʒu�ɖ߂�܂����I");
    }

    private void TeleportToTarget(Transform targetObject)
    {
        Vector3 targetPosition = targetObject.position;

        if (IsPositionBlocked(targetPosition))
        {
            targetPosition = GetSafePosition(targetPosition);
            Debug.Log("�v���C���[���ǂɏՓ˂������߁A�ʒu�𒲐����܂����I");
        }

        transform.position = targetPosition;
        Debug.Log("�v���C���[���ړ����܂����I �V�����ʒu: " + transform.position);
    }

    private bool IsPositionBlocked(Vector3 position)
    {
        RaycastHit hit;
        if (Physics.Raycast(position, Vector3.down, out hit, collisionCheckDistance))
        {
            return true;
        }
        return false;
    }

    private Vector3 GetSafePosition(Vector3 targetPosition)
    {
        Vector3 safePosition = targetPosition + transform.forward * collisionCheckDistance;

        if (!IsPositionBlocked(safePosition))
        {
            return safePosition;
        }

        return targetPosition;
    }

    private void DisableTriggerObject(Transform targetObject)
    {
        int index = targetObjects.IndexOf(targetObject);

        if (index >= 0 && index < targetColliders.Count)
        {
            if (targetColliders[index] != null)
            {
                targetColliders[index].enabled = false;
                targetColliders[index].isTrigger = false;
            }

            if (targetMeshRenderers[index] != null)
            {
                targetMeshRenderers[index].enabled = false;
            }
        }

        Debug.Log("�g���K�[�I�u�W�F�N�g�̋@�\�𖳌������܂����I");
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, triggerRange);
    }
}
