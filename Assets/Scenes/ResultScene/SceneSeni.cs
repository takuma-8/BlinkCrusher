using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.SocialPlatforms.Impl;
using UnityEngine.UI;

public class SceneSeni : MonoBehaviour
{
    public string nextSceneName; // �J�ڂ���V�[�������C���X�y�N�^�[�Őݒ�

    void Update()
    {
        // Fire2�{�^���������ꂽ��V�[����ύX
        if (Input.GetButtonDown("Fire1"))
        {
            ChangeScene();
        }
    }

    public void ChangeScene()
    {
        // �X�R�A�����Z�b�g
        DestroyObjectInFront.ResetScore();
        SceneManager.LoadScene("TitleScene");
    }
}
