using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ControllerButtonClick : MonoBehaviour
{
    public Button targetButton;  // シーン遷移のボタンをセット

    void Update()
    {
        AudioListener.pause = false;
        // Bボタン（JoystickButton1）が押されたらボタンをクリック
        if (Input.GetKeyDown(KeyCode.JoystickButton0))
        {
            Debug.Log("Bボタンが押された！ボタンをクリックします。");
            targetButton.onClick.Invoke();
        }
    }
}
