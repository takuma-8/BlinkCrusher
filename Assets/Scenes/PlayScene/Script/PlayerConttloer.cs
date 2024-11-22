using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public GameObject[] BlinkArray = new GameObject[3];
    private int BlinkPoint = 3;
    public List<GameObject> WallList;

    public float speed_ = 5.0f;
    public float normalSpeed_ = 5.0f;
    public float blinkSpeed_ = 8.0f;
    public float distance_ = 1.0f;

    private Rigidbody rb;
    private Vector3 targetPosition_;
    [HideInInspector] public bool isMoving_ = false;

    private BoxCollider blinkCollider; // ブリンク用の一時コライダー
    private string originalTag; // 元のタグを保存

    private Vector3 originalPosition; // 元の位置を保持
    private float groundCheckDistance = 1.5f; // 地面との距離を確認する範囲

   

    void Update()
    {
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        Vector3 direction = new Vector3(horizontal, 0, vertical).normalized;

        if (!isMoving_)
        {
            rb.MovePosition(rb.position + direction * speed_ * Time.deltaTime);
        }

        Vector3 movement = new Vector3(horizontal, 0.0f, vertical);

        if (movement.magnitude > 0.1f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(movement);
            rb.rotation = Quaternion.Slerp(rb.rotation, targetRotation, Time.deltaTime * 8.0f);
        }

        if ((Input.GetKeyDown(KeyCode.Space) || Input.GetButtonDown("B")) && BlinkPoint > 0 && !isMoving_)
        {
            // BlinkPoint--;
            StartBlink();
            UpdateBlinkUI();
        }
    }

  

    void Start()
    {
        // Rigidbody の設定
        rb = GetComponent<Rigidbody>();
        rb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;  // 衝突判定モードを ContinuousDynamic に設定

        // プレイヤーの初期位置を調整（もし地面にめり込んでいる場合）
        Vector3 initialPosition = transform.position;
        RaycastHit hit;
        if (Physics.Raycast(initialPosition, Vector3.down, out hit, 2f)) // 2 ユニット以内に地面があれば
        {
            // 地面に少し上に移動させる
            transform.position = new Vector3(initialPosition.x, hit.point.y + 0.5f, initialPosition.z);
        }

        // 他の初期化処理
        speed_ = normalSpeed_;
        originalTag = gameObject.tag; // 元のタグを保存
        UpdateBlinkUI();
    }

    void Blink()
    {
        // 現在位置から目標位置までの方向を計算
        Vector3 direction_ = (targetPosition_ - rb.position).normalized;

        // Raycastで障害物を検知
        RaycastHit hit;
        if (Physics.Raycast(rb.position, direction_, out hit, blinkSpeed_ * Time.fixedDeltaTime))
        {
            if (hit.collider.CompareTag("Wall"))
            {
                // 壁に当たった場合、その地点でブリンクを終了
                rb.MovePosition(hit.point - direction_ * 0.1f); // 壁にぶつかったら少し手前で停止
                Debug.Log("Blink stopped by Wall");
                EndBlink();
                return;
            }
        }

        // 目標位置に向けて物理的に移動
        Vector3 newPosition = Vector3.MoveTowards(rb.position, targetPosition_, blinkSpeed_ * Time.fixedDeltaTime);
        rb.MovePosition(newPosition);

        // 目標位置に十分近づいたら移動を停止
        if (Vector3.Distance(rb.position, targetPosition_) <= 0.7f)
        {
            rb.MovePosition(targetPosition_);
            EndBlink();
        }
    }

    void StartBlink()
    {
        // ブリンク先に障害物がないかチェック
        RaycastHit hit;
        if (Physics.Raycast(rb.position, transform.forward, out hit, distance_))
        {
            if (hit.collider.CompareTag("Wall"))
            {
                Debug.Log("Obstacle detected, Blink cancelled!");
                BlinkPoint++; // ブリンクキャンセル時にポイントを回復
                return;
            }
        }

        // ブリンク開始時に速度を変更
        speed_ = blinkSpeed_;  // 通常速度ではなく、ブリンク速度に設定
        targetPosition_ = rb.position + transform.forward * distance_;
        isMoving_ = true;

        rb.isKinematic = true;
        gameObject.tag = "Blink_player";
        AddBlinkCollider();
    }

    void EndBlink()
    {
        // ブリンク終了時の処理
        isMoving_ = false;
        speed_ = normalSpeed_;  // ブリンク終了後に元の速度に戻す
        rb.isKinematic = false;

        // 慣性を止める
        rb.velocity = Vector3.zero;

        gameObject.tag = originalTag;
        RemoveBlinkCollider();
    }
    void FixedUpdate()
    {
        if (isMoving_)
        {
            Blink();  // ブリンクの処理
        }
        else
        {
            // 通常の移動処理
            HandleNormalMovement();
            AdjustPositionAfterBlink();  // ブリンク終了後に地面修正
        }
    }

    void AdjustPositionAfterBlink()
    {
        // ブリンク終了後にプレイヤーが地面にいない場合に修正
        RaycastHit hit;
        if (Physics.Raycast(transform.position, Vector3.down, out hit, 2f)) // 2ユニット以内に地面があれば
        {
            // 地面が見つかれば位置を調整
            Vector3 correctedPosition = new Vector3(transform.position.x, hit.point.y + 0.5f, transform.position.z);
            rb.MovePosition(correctedPosition);
        }
        else
        {
            // 地面がない場合は空中にいると仮定して、元の位置を少し上に調整
            Vector3 correctedPosition = new Vector3(transform.position.x, originalPosition.y + 0.5f, transform.position.z);
            rb.MovePosition(correctedPosition);
        }
    }

    void HandleNormalMovement()
    {
        // 通常の移動処理
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        Vector3 direction = new Vector3(horizontal, 0, vertical).normalized;

        if (direction.magnitude > 0.1f)
        {
            // 移動方向に従って移動
            Vector3 move = direction * speed_ * Time.fixedDeltaTime;
            rb.MovePosition(rb.position + move);

            // 移動方向に向けて回転
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            rb.rotation = Quaternion.Slerp(rb.rotation, targetRotation, Time.fixedDeltaTime * 8.0f);
        }
    }


    private void AddBlinkCollider()
    {
        blinkCollider = gameObject.AddComponent<BoxCollider>();
        blinkCollider.isTrigger = true;
        blinkCollider.center = new Vector3(0, 0, 0.75f);
        blinkCollider.size = new Vector3(1.5f, 1.5f, 1.5f);
    }

    private void RemoveBlinkCollider()
    {
        if (blinkCollider != null)
        {
            Destroy(blinkCollider);
        }
    }

    private void UpdateBlinkUI()
    {
        for (int i = 0; i < BlinkArray.Length; i++)
        {
            BlinkArray[i].SetActive(i < BlinkPoint);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy1") && isMoving_)
        {
            Debug.Log("Enemy hit during Blink!");
            EnemyController enemyController = other.GetComponent<EnemyController>();
            if (enemyController != null)
            {
                enemyController.Stun();
            }
        }
    }
}




/*
using System.Collections;
using UnityEngine;

public class PlayerConttloer : MonoBehaviour
{
    public GameObject[] BlinkArray = new GameObject[3];
    private int BlinkPoint = 3;

    public float speed_ = 5.0f;
    public float normalSpeed_ = 5.0f;
    public float blinkSpeed_ = 8.0f;
    public float distance_ = 1.0f;

    private Rigidbody rb;
    private Vector3 targetPosition_;
    [HideInInspector] public bool isMoving_ = false;

    private BoxCollider blinkCollider; // ブリンク用の一時コライダー
    private string originalTag; // 元のタグを保存

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        speed_ = normalSpeed_;
        originalTag = gameObject.tag; // 元のタグを保存
        UpdateBlinkUI();
    }

    void Update()
    {
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        Vector3 direction = new Vector3(horizontal, 0, vertical).normalized;

        if (!isMoving_)
        {
            rb.MovePosition(rb.position + direction * speed_ * Time.deltaTime);
        }

        Vector3 movement = new Vector3(horizontal, 0.0f, vertical);

        if (movement.magnitude > 0.1f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(movement);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 8.0f);
        }

        if ((Input.GetKeyDown(KeyCode.Space) || Input.GetButtonDown("B")) && BlinkPoint > 0 && !isMoving_)
        {
            BlinkPoint--;
            StartBlink();
            UpdateBlinkUI();
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
        // ブリンク先に障害物がないかチェック
        RaycastHit hit;
        if (Physics.Raycast(transform.position, transform.forward, out hit, distance_))
        {
            if (hit.collider.CompareTag("Wall"))
            {
                // 壁がある場合、ブリンクをキャンセル
                Debug.Log("Obstacle detected, Blink cancelled!");
                BlinkPoint++;
                return;
            }
        }

        targetPosition_ = rb.position + transform.forward * distance_;
        speed_ = blinkSpeed_;
        isMoving_ = true;

        rb.isKinematic = true;
        gameObject.tag = "Blink_player";
        AddBlinkCollider();
    }

    void Blink()
    {
        // ブリンク先までの方向を計算
        Vector3 direction_ = (targetPosition_ - transform.position).normalized;

        // MovePositionで物理的に移動
        Vector3 newPosition = Vector3.MoveTowards(transform.position, targetPosition_, blinkSpeed_ * Time.fixedDeltaTime);
        rb.MovePosition(newPosition);

        // 目標位置に十分近づいたら移動を停止
        if (Vector3.Distance(transform.position, targetPosition_) <= 0.7f)
        {
            rb.MovePosition(targetPosition_);
            EndBlink();
        }
    }

    /*
    void Blink()
    {
        // ブリンク先までの方向を計算
        Vector3 direction_ = (targetPosition_ - transform.position).normalized;

        // ブリンク移動を直接transform.positionで行う
        transform.position = Vector3.MoveTowards(transform.position, targetPosition_, blinkSpeed_ * Time.fixedDeltaTime);

        // 目標位置に十分近づいたら移動を停止
        if (Vector3.Distance(transform.position, targetPosition_) <= 0.7f)
        {
            transform.position = targetPosition_;

            isMoving_ = false;
            speed_ = normalSpeed_;

            // Rigidbody の isKinematic を元に戻す
            rb.isKinematic = false;

            // タグを元に戻す
            gameObject.tag = originalTag;

            // コライダーを削除
            RemoveBlinkCollider();

            
        }
        
    }




    private void AddBlinkCollider()
    {
        // コライダーを生成
        blinkCollider = gameObject.AddComponent<BoxCollider>();
        blinkCollider.isTrigger = true;

        // コライダーのサイズと位置を設定
        blinkCollider.center = new Vector3(0, 0, 0.75f);
        blinkCollider.size = new Vector3(1.5f, 1.5f, 1.5f);
    }

    private void RemoveBlinkCollider()
    {
        // コライダーを削除
        if (blinkCollider != null)
        {
            Destroy(blinkCollider);
        }
    }

    private void UpdateBlinkUI()
    {
        for (int i = 0; i < BlinkArray.Length; i++)
        {
            BlinkArray[i].SetActive(i < BlinkPoint);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy1") && isMoving_)
        {
            Debug.Log("Enemy hit during Blink!");
            EnemyController enemyController = other.GetComponent<EnemyController>();
            if (enemyController != null)
            {
                enemyController.Stun();
            }
        }
    }
}
*/

