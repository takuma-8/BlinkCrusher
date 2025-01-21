using UnityEngine;

public class CameraSwitcher : MonoBehaviour
{
    public Camera mainCamera; // �ʏ펞�̃J����
    public Camera secondaryCamera; // �؂�ւ��p�̃J����

    void Start()
    {
        // �������F���C���J������L�����A�Z�J���_���J�����𖳌���
        SetCameraState(true);
    }

    void Update()
    {
        // Xbox�R���g���[���[��Y�{�^���ijoystick button 3�j�������Ă���Ԃ����Z�J���_���J������L����
        if (Input.GetKey(KeyCode.JoystickButton3))
        {
            SetCameraState(false); // �Z�J���_���J�����ɐ؂�ւ�
        }
        else
        {
            SetCameraState(true); // ���C���J�����ɖ߂�
        }
    }

    void SetCameraState(bool isMainActive)
    {
        // �J�����̏�Ԃ�؂�ւ���
        if (mainCamera != null) mainCamera.enabled = isMainActive;
        if (secondaryCamera != null) secondaryCamera.enabled = !isMainActive;
    }
}
