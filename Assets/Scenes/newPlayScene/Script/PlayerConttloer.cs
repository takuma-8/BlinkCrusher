using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Movement Parameters")]
    public float moveSpeed = 5.0f; // �ʏ�̈ړ����x
    public float maxDistanceToWall = 0.5f; // �ǂƂ̋�����臒l
    public string wallTag = "Wall";

    private Rigidbody rb;
    private bool isActionLocked = false; // ���샍�b�N�t���O

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        if (isActionLocked) return; // ���샍�b�N���͉������Ȃ�

        // Xbox�R���g���[���[�̉E�X�e�B�b�N�ňړ�
        float horizontal = Input.GetAxis("Horizontal"); // �E�X�e�B�b�N�̍��E
        float vertical = Input.GetAxis("Vertical"); // �E�X�e�B�b�N�̏㉺

        // �v���C���[�̈ړ�
        Vector3 direction = transform.forward * vertical + transform.right * horizontal;
        direction = direction.normalized;

        // �ǃ`�F�b�N
        if (!IsWallInFront(direction))
        {
            rb.MovePosition(rb.position + direction * moveSpeed * Time.deltaTime);
        }
    }

    /// <summary>
    /// �ǂ��ڂ̑O�ɂ��邩�ǂ������`�F�b�N
    /// </summary>
    /// <param name="direction">�i�s����</param>
    /// <returns>�ǂ�����ꍇ��true</returns>
    bool IsWallInFront(Vector3 direction)
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, direction, out hit, maxDistanceToWall))
        {
            if (hit.collider.CompareTag(wallTag))
            {
                Debug.Log("Wall detected!");
                return true;
            }
        }
        return false;
    }

    public void LockPlayerActions()
    {
        isActionLocked = true;
        rb.isKinematic = true; // ���������S�ɒ�~
    }

    public void UnlockPlayerActions()
    {
        isActionLocked = false;
        rb.isKinematic = false; // ������ĊJ
    }

    // �v���C���[�̃^�O��ύX
    public void ChangePlayerTag(string newTag)
    {
        gameObject.tag = newTag;
    }
}
