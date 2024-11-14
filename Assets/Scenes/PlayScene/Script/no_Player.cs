using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float speed = 5.0f;
    private CharacterController controller;

    void Start()
    {
        controller = GetComponent<CharacterController>();
    }

    void Update()
    {
        // プレイヤーの移動方向を入力から取得
        float moveX = Input.GetAxis("Horizontal");
        float moveZ = Input.GetAxis("Vertical");

        // 移動ベクトルを作成
        Vector3 move = new Vector3(moveX, 0, moveZ);

        // CharacterControllerで移動
        controller.Move(move * speed * Time.deltaTime);

        // 移動している場合に向きを調整
        if (move != Vector3.zero)
        {
            // 移動方向に向かせる
            transform.rotation = Quaternion.LookRotation(move);
        }
    }
}
