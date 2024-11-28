using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public GameObject wallCheck; // wallcheck 子オブジェクト
    public string wallTag = "Wall"; // 壁判定に使用するタグ

    public GameObject[] BlinkArray = new GameObject[3];
    private int BlinkPoint = 3;

    public float speed_ = 5.0f;
    public float normalSpeed_ = 5.0f;
    public float blinkSpeed_ = 8.0f;
    public float distance_ = 1.0f;

    private Rigidbody rb;
    private Vector3 targetPosition_;
    [HideInInspector] public bool isMoving_ = false;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        speed_ = normalSpeed_;
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
        // ブリンクの移動を開始
        targetPosition_ = rb.position + transform.forward * distance_;
        speed_ = blinkSpeed_;
        isMoving_ = true; // ブリンク中の移動フラグを立てる
        rb.isKinematic = true; // Rigidbody の物理演算を無効化
    }

    void Blink()
    {
        // wallcheck が壁に接触しているか判定
        Collider[] hitColliders = Physics.OverlapSphere(wallCheck.transform.position, 0.1f);
        foreach (var hitCollider in hitColliders)
        {
            if (hitCollider.CompareTag(wallTag))
            {
                // 壁にぶつかった場合、その場で停止
                Debug.Log("Wall detected, stopping blink!");
                StopBlink();
                return;
            }
        }

        // ブリンク移動
        transform.position = Vector3.MoveTowards(transform.position, targetPosition_, blinkSpeed_ * Time.fixedDeltaTime);

        // 目標位置に到達した場合、ブリンクを停止
        if (Vector3.Distance(transform.position, targetPosition_) <= 0.1f)
        {
            StopBlink();
        }
    }

    void StopBlink()
    {
        isMoving_ = false; // ブリンク終了
        speed_ = normalSpeed_; // 通常速度に戻す
        rb.isKinematic = false; // Rigidbody を元に戻す
        BlinkPoint--; // ブリンクポイントを減らす
        UpdateBlinkUI();
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
        // ブリンク中にエネミーに当たった場合
        if ((other.CompareTag("Enemy1") || other.CompareTag("Enemy2")) && isMoving_)
        {
            Debug.Log("Enemy hit during Blink!");
            EnemyController enemyController = other.GetComponent<EnemyController>();
            if (enemyController != null)
            {
                enemyController.Stun(); // エネミーをスタンさせる処理
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

