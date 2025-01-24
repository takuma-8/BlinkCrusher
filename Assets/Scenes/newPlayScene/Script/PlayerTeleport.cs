using UnityEngine;
using System.Collections.Generic;

public class PlayerTeleport : MonoBehaviour
{
    public List<Transform> targetObjects; // �����̃g���K�[�I�u�W�F�N�g��Transform
    public string fireButton = "Fire2"; // �R���g���[���[��Fire2�{�^��
    public float triggerRange = 5f; // �g���K�[�͈͂̔��a
    public List<Vector3> cameraDirections; // ���b�J�[���Ƃ̃J��������
    public Canvas emperorCanvas; // ���b�J�[���̉��o�pCanvas

    private PlayerController playerController;
    private CameraController cameraController; // CameraController ���Q��
    private bool isEmperor = false;
    private Vector3 originalPosition;
    private Quaternion originalCameraRotation;
    public Camera playerCamera;
    public Camera emperorCamera;

    private void Start()
    {
        playerController = GetComponent<PlayerController>();
        cameraController = GetComponent<CameraController>(); // CameraController ���擾
        if (playerController == null)
            Debug.LogError("PlayerController��������܂���I");

        if (emperorCanvas != null)
            emperorCanvas.enabled = false; // ������Ԃł͔�\��
    }

    private void EnterEmperorState(Transform targetObject)
    {
        originalPosition = transform.position;
        originalCameraRotation = playerCamera.transform.rotation;

        playerController.LockPlayerActions();
        playerController.ChangePlayerTag("not_Player");
        isEmperor = true;

        // �J��������𖳌���
        cameraController.DisableCameraControl(); // CameraController �̃��\�b�h���Ăяo��

        // �J�����؂�ւ�
        if (playerCamera != null) playerCamera.gameObject.SetActive(false);
        if (emperorCamera != null)
        {
            emperorCamera.gameObject.SetActive(true);
            int index = targetObjects.IndexOf(targetObject);
            if (index >= 0 && index < cameraDirections.Count)
            {
                // �J����������ݒ肵�A����̎����Œ�
                Vector3 cameraDirection = cameraDirections[index];
                cameraDirection.x = 0; // �s�b�`�i�㉺�����̉�]�j���Œ�
                cameraDirection.z = 0; // ���[���i���X���j���Œ�
                emperorCamera.transform.rotation = Quaternion.Euler(cameraDirection);
            }
        }

        // ���b�J�[�����o
        if (emperorCanvas != null)
            emperorCanvas.enabled = true;

        Debug.Log("�G���y���[��Ԃɓ���܂����B");
    }

    private void ExitEmperorState()
    {
        playerController.UnlockPlayerActions();
        playerController.ChangePlayerTag("Player");
        isEmperor = false;

        // �J��������𕜌�
        cameraController.EnableCameraControl(); // CameraController �̃��\�b�h���Ăяo��

        // �J�����؂�ւ�
        if (playerCamera != null) playerCamera.gameObject.SetActive(true);
        if (emperorCamera != null)
            emperorCamera.gameObject.SetActive(false);

        // ���̈ʒu�ƃJ���������ɖ߂�
        transform.position = originalPosition;
        playerCamera.transform.rotation = originalCameraRotation;

        // ���b�J�[�����o����
        if (emperorCanvas != null)
            emperorCanvas.enabled = false;

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
}
