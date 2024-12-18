using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Movement Parameters")]
    public float moveSpeed = 5.0f; // �ʏ�̈ړ����x
    public float crouchSpeed = 3.0f; // ���Ⴊ�ݎ��̈ړ����x
    public float maxDistanceToWall = 0.5f; // �ǂƂ̋�����臒l
    public string wallTag = "Wall";
    public bool isCrouching { get; private set; } // �O������ǂݎ��\�����A�����ł̂ݕύX�\
    private bool canStandUp = true;  // ���Ⴊ�݉����\��

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

        // �v���C���[�̈ړ����x�ݒ�
        float currentSpeed = isCrouching ? crouchSpeed : moveSpeed;

        // �v���C���[�̈ړ�
        Vector3 direction = transform.forward * vertical + transform.right * horizontal;
        direction = direction.normalized;

        // �ǃ`�F�b�N
        if (!IsWallInFront(direction))
        {
            rb.MovePosition(rb.position + direction * currentSpeed * Time.deltaTime);
        }

        // ���Ⴊ�ݓ��� (��: C�L�[�ł��Ⴊ��/����)
        if (Input.GetKeyDown(KeyCode.C) || Input.GetButtonDown("Fire2"))
        {
            if (!isCrouching)
            {
                Crouch(); // ���Ⴊ��
            }
            else if (canStandUp) // ���Ⴊ�݉�����������Ă���ꍇ�̂ݎ��s
            {
                StandUp(); // ���Ⴊ�݉���
            }
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

    private void Crouch()
    {
        isCrouching = true;
        transform.localScale = new Vector3(1f, 0.5f, 1f); // �v���C���[�̍����𔼕��ɂ���
        transform.position = new Vector3(transform.position.x, 0.5f, transform.position.z); // y���W��0.5�ɐݒ�
        Debug.Log("���Ⴊ��");
    }

    private void StandUp()
    {
        isCrouching = false;
        transform.localScale = new Vector3(1f, 1f, 1f); // �v���C���[�̍��������ɖ߂�
        transform.position = new Vector3(transform.position.x, 1f, transform.position.z); // y���W��1�ɖ߂�
        Debug.Log("�����オ����");
    }

    // ���Ⴊ�݉����s�G���A�ɓ������ꍇ
    public void SetCrouchRestriction(bool restriction)
    {
        canStandUp = !restriction;
        Debug.Log(restriction ? "���Ⴊ�݉����s�G���A�ɓ�����" : "���Ⴊ�݉����s�G���A����o��");
    }
}