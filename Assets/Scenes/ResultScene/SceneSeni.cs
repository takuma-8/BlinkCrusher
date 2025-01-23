using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.SocialPlatforms.Impl;
using UnityEngine.UI;

public class SceneSeni : MonoBehaviour
{
    public string nextSceneName; // �J�ڂ���V�[�������C���X�y�N�^�[�Őݒ�

    void Start()
    {
        // �{�^�����N���b�N���ꂽ��V�[����ύX���鏈����o�^
        GetComponent<Button>().onClick.AddListener(ChangeScene);
    }

    public void ChangeScene()
    {
        // �X�R�A�����Z�b�g
        DestroyObjectInFront.ResetScore();
        SceneManager.LoadScene("TitleScene");
    }
}
