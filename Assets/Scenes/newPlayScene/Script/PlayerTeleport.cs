using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;

public class PlayerTeleport : MonoBehaviour
{
    public List<Transform> targetObjects; // �����̃g���K�[�I�u�W�F�N�g��Transform
    public string fireButton = "Fire2"; // �R���g���[���[��Fire2�{�^��
    public float triggerRange = 5f; // �g���K�[�͈͂̔��a
    public List<Vector3> cameraDirections; // ���b�J�[���Ƃ̃J��������
    public Image locker; // ���b�J�[���̉��o�pCanvas

    private PlayerController playerController;
    private CameraController cameraController; // CameraController ���Q��
    private bool isEmperor = false;
    public Camera playerCamera;
    public Camera emperorCamera;

    private void Start()
    {
        playerController = GetComponent<PlayerController>();
        cameraController = GetComponent<CameraController>(); // CameraController ���擾

        if (playerController == null)
            Debug.LogError("PlayerController��������܂���I");

        if (locker != null)
            locker.enabled = false; // ������Ԃł͔�\��
    }

    private void EnterEmperorState(Transform targetObject)
    {
        playerController.LockPlayerActions();
        playerController.ChangePlayerTag("not_Player");
        isEmperor = true;

        // �J��������𖳌���
        cameraController.DisableCameraControl();

        // ���C���J���������Z�b�g
        if (playerCamera != null)
        {
            playerCamera.transform.rotation = Quaternion.Euler(0, 0, 0);
            playerCamera.gameObject.SetActive(false);
        }

        // �G���y���[�J���������Z�b�g
        if (emperorCamera != null)
        {
            if (!emperorCamera.gameObject.activeInHierarchy)
            {
                emperorCamera.gameObject.SetActive(true);
            }

            // �v���C���[�̌������J�����̌����ɍ��킹��
            Vector3 cameraDirection = cameraDirections[targetObjects.IndexOf(targetObject)];
            cameraDirection.x = 0;
            cameraDirection.z = 0;

            // �v���C���[�̉�]���ɐݒ�
            Vector3 playerRotation = new Vector3(0, cameraDirection.y, 0);
            transform.rotation = Quaternion.Euler(playerRotation);

            // ���ɃJ�����̉�]��ݒ�
            emperorCamera.transform.rotation = Quaternion.Euler(cameraDirection);
        }

        // ���b�J�[�����o
        UpdateLockerVisibility();

        Debug.Log("�G���y���[��Ԃɓ���܂����B");
    }

    private void ExitEmperorState()
    {
        playerController.UnlockPlayerActions();
        playerController.ChangePlayerTag("Player");
        isEmperor = false;

        // �G���y���[�J�����̉�]�����C���J�����ɓK�p
        if (emperorCamera != null && playerCamera != null)
        {
            Quaternion emperorRotation = emperorCamera.transform.rotation;

            // �v���C���[��Y����]�̂݃G���y���[�J�����̉�]�ɍ��킹��
            Vector3 cameraEulerAngles = emperorRotation.eulerAngles;
            Vector3 playerRotation = new Vector3(0, cameraEulerAngles.y, 0);
            transform.rotation = Quaternion.Euler(playerRotation); // �v���C���[�̉�]��Y�������X�V

            // ���C���J�����̉�]���X�V
            playerCamera.transform.rotation = emperorRotation;
            Debug.Log($"�G���y���[�J�����̉�]�����C���J�����ɓK�p���܂���: {emperorRotation.eulerAngles}");
        }
        else
        {
            Debug.LogWarning("�G���y���[�J�����܂��̓��C���J�������ݒ肳��Ă��܂���B");
        }

        // ���C���J������L����
        if (playerCamera != null)
        {
            playerCamera.gameObject.SetActive(true);
        }

        // �G���y���[�J�����𖳌���
        if (emperorCamera != null)
        {
            emperorCamera.gameObject.SetActive(false);
        }

        // �J�����R���g���[���𕜌�
        cameraController.EnableCameraControl();

        // �v���C���[�������Ă�������Ɉ���O�ɏo��
        Vector3 forwardDirection = transform.forward; // �v���C���[�̌����Ɋ�Â�
        transform.position += forwardDirection; // ����O�ɐi��

        // ���b�J�[�����o������
        UpdateLockerVisibility();

        Debug.Log("�G���y���[��Ԃ���������܂����B");
    }

    private void Update()
    {
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
                    EnterEmperorState(targetObject);
                    TeleportToTarget(targetObject);
                }
            }
        }

        // �G���y���[��Ԃɉ��������b�J�[�\���̍X�V (�O�̂���)
        UpdateLockerVisibility();
    }

    private void TeleportToTarget(Transform targetObject)
    {
        transform.position = targetObject.position;
        Debug.Log("�w�肳�ꂽ���b�J�[�Ƀe���|�[�g���܂����I");
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

    private void UpdateLockerVisibility()
    {
        if (locker != null)
        {
            locker.enabled = isEmperor;
        }
    }
}
