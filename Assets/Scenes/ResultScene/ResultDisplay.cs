using UnityEngine;
using UnityEngine.UI;

public class ResultDisplay : MonoBehaviour
{
    public Text scoreText;  // �X�R�A�\���p��UI Text

    void Start()
    {
            scoreText.text = "�X�R�A: " + DestroyObjectInFront.GetScore().ToString();    
    }
}
