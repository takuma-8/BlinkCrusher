using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.SocialPlatforms.Impl;
using UnityEngine.UI;

public class SceneSeni : MonoBehaviour
{
    public string nextSceneName; // 遷移するシーン名をインスペクターで設定

    void Update()
    {
        // Fire2ボタンが押されたらシーンを変更
        if (Input.GetButtonDown("Fire1"))
        {
            ChangeScene();
        }
    }

    public void ChangeScene()
    {
        // スコアをリセット
        DestroyObjectInFront.ResetScore();
        SceneManager.LoadScene("TitleScene");
    }
}
