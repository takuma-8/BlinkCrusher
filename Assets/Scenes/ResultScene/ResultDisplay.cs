using UnityEngine;
using UnityEngine.UI;

public class ResultDisplay : MonoBehaviour
{
    public Text scoreText;  // スコア表示用のUI Text

    void Start()
    {
            scoreText.text = "Score: " + DestroyObjectInFront.GetScore().ToString();    
    }
}
