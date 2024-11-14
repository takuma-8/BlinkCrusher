using System.Collections;
using UnityEngine;

public class life : MonoBehaviour
{
    public GameObject[] lifeArray = new GameObject[3]; // 最大3つのハート
    private int lifePoint = 3; // ハートの残り数
    private bool isStunned = false; // スタン状態のフラグ

    void Start()
    {
        UpdateLifeUI(); // 最初にハートをUIに反映
    }

    void OnTriggerEnter(Collider other)
    {
        // zornというタグが付けられたオブジェクトに触れた場合
        if (other.gameObject.CompareTag("Zorn") && !isStunned)
        {
            StartCoroutine(HandleStun()); // スタン処理を開始
        }
    }

    // スタン処理
    private IEnumerator HandleStun()
    {
        isStunned = true; // スタン状態
        Debug.Log("スタン状態開始");

        // ハートを減らす
        if (lifePoint > 0)
        {
            lifePoint--;
            UpdateLifeUI(); // ハートのUIを更新
        }

        // 2秒間スタン状態を維持
        yield return new WaitForSeconds(2f);

        isStunned = false; // スタン解除
        Debug.Log("スタン解除");
    }

    // UIのハート表示を更新
    private void UpdateLifeUI()
    {
        for (int i = 0; i < lifeArray.Length; i++)
        {
            if (i < lifePoint)
            {
                lifeArray[i].SetActive(true); // ハートを表示
            }
            else
            {
                lifeArray[i].SetActive(false); // ハートを非表示
            }
        }
    }
}

