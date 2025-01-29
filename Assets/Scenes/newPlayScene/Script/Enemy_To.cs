using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_To: MonoBehaviour
{
    // Start is called before the first frame update
    private Animator animation;

    void Start()
    {
        animation = gameObject.GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        // Enterキーが押されたかどうかをチェックします
        if (Input.GetKeyDown(KeyCode.Return))
        {
            // Animator内の"boolwalk"パラメータをtrueに設定し、歩行アニメーションを再生する
            animation.SetBool("blRot" +
                "", true);
        }
    }
}