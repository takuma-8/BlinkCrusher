using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class SceneSeni : MonoBehaviour
{
    public void ChangeScene()
    {
        SceneManager.LoadScene("PlayScene");
    }
}