using UnityEngine;

public class SecretCommand : MonoBehaviour
{
    // �B���R�}���h�̏���
    private readonly string[] secretCommand = {
        "TriggerLeft", "TriggerRight",
        "X", "Y",
        "TriggerLeft", "TriggerRight",
        "X", "Y",
        "TriggerLeft", "TriggerRight",
        "X", "X"
    };

    private int inputIndex = 0; // ���݂̓��͈ʒu��ǐ�
    private float inputCooldown = 0.2f; // ���̓N�[���_�E���i���̓��͂��������Ԋu�j
    private float lastInputTime = 0f; // �Ō�̓��͎��Ԃ��L�^
    private bool isSecretEnemySummoned = false; // ��x������������t���O

    // Secret_Enemy��Prefab���A�T�C��
    public GameObject secretEnemyPrefab;

    void Update()
    {
        if (Input.GetAxis("TriggerLeft") > 0.8f) Debug.Log("TriggerLeft pressed");
        if (Input.GetAxis("TriggerRight") > 0.8f) Debug.Log("TriggerRight pressed");
        if (Input.GetButtonDown("X")) Debug.Log("X pressed");
        if (Input.GetButtonDown("Y")) Debug.Log("Y pressed");
        if (Time.time - lastInputTime < inputCooldown)
            return; // �N�[���_�E�����͓��͂𖳎�

        // �R�}���h���̓`�F�b�N
        if (CheckInput(secretCommand[inputIndex]))
        {
            inputIndex++;
            lastInputTime = Time.time; // ���͂��L�^

            // �S�ẴR�}���h�����������ꍇ
            if (inputIndex >= secretCommand.Length && !isSecretEnemySummoned)
            {
                Debug.Log("�B���R�}���h���������܂����I");
                ActivateSecretFeature(); // �B���@�\�𔭓�
                inputIndex = 0; // ���Z�b�g
            }
        }
        else if (AnyInputDetected()) // ���̓��͂�����΃��Z�b�g
        {
            inputIndex = 0;
        }
    }

    // ���̓`�F�b�N
    private bool CheckInput(string command)
    {
        switch (command)
        {
            case "TriggerLeft":
                return Input.GetAxis("TriggerLeft") > 0.8f; // ���g���K�[����
            case "TriggerRight":
                return Input.GetAxis("TriggerRight") > 0.8f; // �E�g���K�[����
            case "Y":
                return Input.GetButtonDown("Y"); // Y�{�^������
            case "X":
                return Input.GetButtonDown("X"); // X�{�^������
            default:
                return false;
        }
    }

    // �C�ӂ̓��͂��������������m�F
    private bool AnyInputDetected()
    {
        return Input.GetAxis("TriggerLeft") > 0.1f ||
               Input.GetAxis("TriggerRight") > 0.1f ||
               Input.GetButtonDown("Y") ||
               Input.GetButtonDown("X");
    }

    // �B���R�}���h�̔������Ɏ��s�����@�\
    private void ActivateSecretFeature()
    {
        if (!isSecretEnemySummoned && secretEnemyPrefab != null)
        {
            Vector3 spawnPosition = new Vector3(43.63f, 1.03f, -3.48f);
            Instantiate(secretEnemyPrefab, spawnPosition, Quaternion.identity); // Secret_Enemy������
            Debug.Log("Secret_Enemy����������܂����I");
            isSecretEnemySummoned = true; // �t���O�𗧂Ă�
        }
    }
}
