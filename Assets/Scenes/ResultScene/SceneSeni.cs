using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.SocialPlatforms.Impl;
using UnityEngine.UI;

public class SceneSeni : MonoBehaviour
{
    public string nextSceneName; // 遷移するシーン名をインスペクターで設定

    void Start()
    {
        // ボタンがクリックされたらシーンを変更する処理を登録
        GetComponent<Button>().onClick.AddListener(ChangeScene);
    }

    public void ChangeScene()
    {
        // スコアをリセット
        DestroyObjectInFront.ResetScore();
        SceneManager.LoadScene("TitleScene");
    }
}
