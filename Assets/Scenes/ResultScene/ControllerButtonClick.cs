using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ControllerButtonClick : MonoBehaviour
{
    public Button targetButton;  // �V�[���J�ڂ̃{�^�����Z�b�g

    void Update()
    {
        AudioListener.pause = false;
        // B�{�^���iJoystickButton1�j�������ꂽ��{�^�����N���b�N
        if (Input.GetKeyDown(KeyCode.JoystickButton0))
        {
            Debug.Log("B�{�^���������ꂽ�I�{�^�����N���b�N���܂��B");
            targetButton.onClick.Invoke();
        }
    }
}
