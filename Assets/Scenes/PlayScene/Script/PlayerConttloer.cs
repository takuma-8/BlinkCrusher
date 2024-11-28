using UnityEngine;
using UnityEngine.UI;
public class PlayerController : MonoBehaviour
{
    public Image cooldownCircle; // 円形ゲージ用のUI
    public Image colorCircle;    // クールタイム中の補助画像
    public Image Blinkimage;     // 常時表示される画像
    public RectTransform[] BlinkUIPositions; // ブリンクUIのRectTransform配列

    private bool isCooldown = false; // クールタイム中の状態を管理
    private float cooldownTimer = 0f; // 現在のクールタイム進行
    private float coolTime = 30f; // クールタイムの秒数
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

        if (colorCircle != null)
        {
            colorCircle.enabled = false; // 画像を非表示に設定
            Blinkimage.enabled = false;
        }
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

        CoolTime(); // クールタイムの処理を追加
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
    void CoolTime()
    {
        // BlinkPoint が最大値ではない場合、クールダウンを進行
        if (BlinkPoint < BlinkArray.Length)
        {
            isCooldown = true; // クールダウン中
            cooldownTimer += Time.deltaTime; // タイマーを進行
            cooldownCircle.fillAmount = cooldownTimer / coolTime; // ゲージを更新

            if (!colorCircle.enabled)
            {
                colorCircle.enabled = true; // クールダウン中の画像を表示
            }

            if (!Blinkimage.enabled)
            {
                Blinkimage.enabled = true; // Blinkimage を表示
            }

            // BlinkArray の位置にサークルを移動
            if (BlinkArray.Length > BlinkPoint)
            {
                // BlinkArray の位置に cooldownCircle を移動
                cooldownCircle.rectTransform.position = BlinkArray[BlinkPoint].transform.position;

                // colorCircle と Blinkimage の位置も同様に移動
                colorCircle.rectTransform.position = BlinkArray[BlinkPoint].transform.position;
                Blinkimage.rectTransform.position = BlinkArray[BlinkPoint].transform.position;
            }

            // 一定の透明度に設定（例えば、50%透明に設定）
            SetBlinkImageTransparency(0.5f); // 透明度を50%に設定

            if (cooldownTimer >= coolTime) // クールタイム完了時
            {
                BlinkPoint++; // ポイントを回復
                cooldownTimer = 0f; // タイマーをリセット
                UpdateBlinkUI(); // UIを更新

                // 最大値に達した場合、クールダウン終了
                if (BlinkPoint == BlinkArray.Length)
                {
                    cooldownCircle.fillAmount = 0f; // ゲージをリセット
                    colorCircle.enabled = false; // 画像を非表示
                    Blinkimage.enabled = false; // Blinkimage を非表示
                    isCooldown = false;
                }
            }
        }
        else
        {
            // BlinkPoint が最大の場合、クールダウンを無効化
            isCooldown = false;
            colorCircle.enabled = false; // 画像を非表示
            Blinkimage.enabled = true;  // Blinkimage は表示されたまま
            cooldownCircle.fillAmount = 0f; // ゲージをリセット
        }
    }






    void StopBlink()
    {
        isMoving_ = false;
        speed_ = normalSpeed_;
        rb.isKinematic = false;
        BlinkPoint--;
        UpdateBlinkUI();

        if (!isCooldown) // クールタイムが始まっていない場合
        {
            isCooldown = true; // クールタイムを開始
            cooldownTimer = 0f; // タイマーをリセット
            colorCircle.enabled = true;
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

    void SetBlinkImageTransparency(float alpha)
    {
        Color currentColor = Blinkimage.color;
        currentColor.a = alpha;  // 透明度を指定された値に設定
        Blinkimage.color = currentColor;
    }

}