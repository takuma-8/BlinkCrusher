using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class BlinkingText : MonoBehaviour
{
    public Text targetText; // UIのTextコンポーネント

    void Start()
    {
        StartCoroutine(BlinkText());
    }

    IEnumerator BlinkText()
    {
        while (true)
        {
            targetText.enabled = !targetText.enabled; // 表示 / 非表示を切り替える
            yield return new WaitForSeconds(1f); // 1秒待つ
        }
    }
}
