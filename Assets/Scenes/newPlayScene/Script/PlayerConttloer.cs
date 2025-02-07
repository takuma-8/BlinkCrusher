using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Movement Parameters")]
    public float moveSpeed = 5.0f;
    public float maxDistanceToWall = 0.5f;
    public string wallTag = "Wall";

    private Rigidbody rb;
    private bool isActionLocked = false;

    private PlayerSoundManager soundManager;

    void Start()
    {
        Texture2D texture = Resources.Load<Texture2D>("YourTexture");
        Renderer renderer = GetComponent<Renderer>();
        renderer.material.mainTexture = texture;

        rb = GetComponent<Rigidbody>();
        soundManager = GetComponent<PlayerSoundManager>();

        if (soundManager == null)
        {
            Debug.LogError("PlayerSoundManager がアタッチされていません！");
        }
    }

    void FixedUpdate()
    {
        if (isActionLocked) return;

        // Xbox コントローラーの右スティックで移動
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        Vector3 direction = transform.forward * vertical + transform.right * horizontal;

        if (direction.sqrMagnitude > 0.01f) // 小さな動きを無視
        {
            direction.Normalize();

            if (!IsWallInFront(direction))
            {
                rb.MovePosition(rb.position + direction * moveSpeed * Time.fixedDeltaTime);

                if (soundManager != null)
                {
                    soundManager.PlayFootStep();
                }
            }
            else if (soundManager != null)
            {
                soundManager.StopFootStep();
            }
        }
        else if (soundManager != null)
        {
            soundManager.StopFootStep();
        }
    }

    bool IsWallInFront(Vector3 direction)
    {
        RaycastHit hit;
        return Physics.Raycast(transform.position, direction, out hit, maxDistanceToWall) && hit.collider.CompareTag(wallTag);
    }

    public void LockPlayerActions()
    {
        isActionLocked = true;
        rb.isKinematic = true;
    }

    public void UnlockPlayerActions()
    {
        isActionLocked = false;
        rb.isKinematic = false;
    }

    public void ChangePlayerTag(string newTag)
    {
        gameObject.tag = newTag;
    }
}
