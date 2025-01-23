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
    private CameraCollision cameraCollision;

    public Camera playerCamera; // �v���C���[�̃J�����iInspector����w��j
    public Camera emperorCamera; // �G���y���[��ԗp�J�����iInspector����w��j

    private void Start()
    {
        // PlayerController���擾
        playerController = GetComponent<PlayerController>();
        if (playerController == null)
        {
            Debug.LogError("PlayerController��������܂���I���̃X�N���v�g���A�^�b�`����GameObject��PlayerController�R���|�[�l���g��ǉ����Ă��������B");
        }

        // CameraCollision�𖳌���
        cameraCollision = FindObjectOfType<CameraCollision>();
        if (cameraCollision == null)
        {
            Debug.LogError("CameraCollision��������܂���I�V�[�����ɑ��݂��邩�m�F���Ă��������B");
        }
        else
        {
            cameraCollision.enabled = false; // ������ԂŖ�����
        }

        // �v���C���[�J�����ƃG���y���[�J�������w�肳��Ă��Ȃ��ꍇ�̓G���[���b�Z�[�W���o��
        if (playerCamera == null)
        {
            Debug.LogError("playerCamera���ݒ肳��Ă��܂���IInspector�Őݒ肵�Ă��������B");
        }
        if (emperorCamera == null)
        {
            Debug.LogError("emperorCamera���ݒ肳��Ă��܂���IInspector�Őݒ肵�Ă��������B");
        }
    }
    private bool IsPositionBlocked(Vector3 position)
    {
        // �v���C���[�̃L�����N�^�[�̃T�C�Y���l���������C�L���X�g
        RaycastHit hit;
        float playerHeight = 2f;  // �v���C���[�̍����i�K�؂Ȓl��ݒ�j
        if (Physics.Raycast(position + Vector3.up * playerHeight * 0.5f, Vector3.down, out hit, collisionCheckDistance + playerHeight))
        {
            return true; // �Փ˂�����Έʒu���u���b�N����Ă���
        }
        return false;
    }

    private void EnterEmperorState()
    {
        if (playerController == null)
        {
            Debug.LogError("playerController��null�ł��BPlayerController���A�^�b�`����Ă��邩�m�F���Ă��������B");
            return;
        }

        originalPosition = transform.position; // �G���y���[��ԑO�̈ʒu���L�^
        playerController.LockPlayerActions();
        playerController.ChangePlayerTag("not_Player");
        isEmperor = true;

        // �J�����̐؂�ւ�
        if (playerCamera != null) playerCamera.gameObject.SetActive(false); // Main Camera�𖳌��ɂ���
        if (emperorCamera != null) emperorCamera.gameObject.SetActive(true); // �G���y���[�p�J������L���ɂ���

        if (cameraCollision != null)
        {
            cameraCollision.enabled = true; // �G���y���[��ԂŗL����
            cameraCollision.cameraTransform = emperorCamera.transform; // �G���y���[�p�J������ݒ�
        }

        Debug.Log("�v���C���[�̓G���y���[��ԂɂȂ�A�����Ȃ��Ȃ�܂����I");
    }

    private void ExitEmperorState()
    {
        if (playerController == null)
        {
            Debug.LogError("playerController��null�ł��BPlayerController���A�^�b�`����Ă��邩�m�F���Ă��������B");
            return;
        }

        playerController.UnlockPlayerActions();
        playerController.ChangePlayerTag("Player");
        isEmperor = false;

        // �J�����̐؂�ւ�
        if (playerCamera != null) playerCamera.gameObject.SetActive(true); // Main Camera���ĕ\��
        if (emperorCamera != null) emperorCamera.gameObject.SetActive(false); // �G���y���[�p�J�����𖳌��ɂ���

        if (cameraCollision != null)
        {
            cameraCollision.enabled = false; // �ʏ��ԂŖ�����
        }

        // �v���C���[���G���y���[��ԑO�̈ʒu�ɖ߂�
        transform.position = originalPosition;

        // �g���K�[�I�u�W�F�N�g�̏�Ԃ����ɖ߂�
        RestoreTriggerObjectState();

        Debug.Log("�G���y���[��Ԃ���������A�v���C���[�͌��̈ʒu�ɖ߂�܂����I");
    }

    private void RestoreTriggerObjectState()
    {
        // ���ׂẴg���K�[�I�u�W�F�N�g�̏�Ԃ����ɖ߂�
        for (int i = 0; i < targetObjects.Count; i++)
        {
            if (targetColliders[i] != null)
            {
                targetColliders[i].isTrigger = true; // isTrigger�����ɖ߂�
            }

            if (targetMeshRenderers[i] != null)
            {
                targetMeshRenderers[i].enabled = true; // MeshRenderer�����ɖ߂�
            }
        }
    }

    private void Update()
    {
        // targetObjects��null�܂��͋�łȂ������m�F
        if (targetObjects == null || targetObjects.Count == 0) return;

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
                    TeleportToTarget(targetObject);
                    DisableTriggerObject(targetObject);
                }
            }
        }
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

    public bool IsEmperor()
    {
        return isEmperor;
    }

}