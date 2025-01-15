using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.SocialPlatforms.Impl;

public class SceneSeni : MonoBehaviour
{
    public void ChangeScene()
    {
        // スコアをリセット
        DestroyObjectInFront.ResetScore();
        SceneManager.LoadScene("newPlayScene");
    }
}
