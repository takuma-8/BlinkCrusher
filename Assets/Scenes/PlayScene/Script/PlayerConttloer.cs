using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerConttloer : MonoBehaviour
{
    public float speed_ = 5.0f;         // 今のスピード
    public float normalSpeed_ = 5.0f;   // 通常時のスピード
    public float blinkSpeed_ = 8.0f;    // ブリンク時のスピード
    public float distance_ = 1.0f;   // ブリンクで移動する距離

    private Rigidbody rb;
    private Vector3 targetPosition_;
    private bool isMoving_ = false;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        speed_ = normalSpeed_;
    }

    // Update is called once per frame
    void Update()
    {
        // 入力の取得
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        // 移動ベクトルの計算
        Vector3 direction = new Vector3(horizontal, 0, vertical).normalized;

        if (!isMoving_)
        {
            // プレイヤー位置の更新
            rb.MovePosition(rb.position + direction * speed_ * Time.deltaTime);
        }

        Vector3 movement = new Vector3(horizontal, 0.0f, vertical);




        // プレイヤー回転処理
        if (movement.magnitude > 0.1f)  // 小さな値だと回転しないらしい
        {
            // 移動方向にモデルを回転
            Quaternion targetRotation = Quaternion.LookRotation(movement);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 10.0f);
        }

        // ブリンク
        //if (Input.GetButtonDown("B") && !isMoving_)
        //{
        //    StartBlink();
        //}


        if (Input.GetKeyDown(KeyCode.Space))
        {
            StartBlink();
        }
        
    }

    private void FixedUpdate()
    {
        if (isMoving_)
        {
            Blink();
        }
    }

    void StartBlink()
    {
        targetPosition_ = rb.position + transform.forward * distance_;
        speed_ = blinkSpeed_;
        　
        isMoving_ = true;
    }


    void Blink()
    {
        Vector3 direction_ = (targetPosition_ - rb.position).normalized;
        rb.MovePosition(rb.position + direction_ * speed_ * Time.fixedDeltaTime);

        Debug.Log($"Current Position: {rb.position}, Target Position: {targetPosition_}");

        if (Vector3.Distance(rb.position, targetPosition_) <= 0.7f)
        {
            // 最終位置をセット
            rb.position = targetPosition_;
            transform.position = targetPosition_;

            rb.velocity = Vector3.zero;        // 移動速度をゼロにリセット
            rb.angularVelocity = Vector3.zero; // 回転速度をゼロにリセット

         
            isMoving_ = false;

            // スピードを戻す
            speed_ = normalSpeed_;

            // デバッグで確認
            Debug.Log("Blink終了");
        }
    }

}
