using UnityEngine;

public class HideUnderDesk : MonoBehaviour
{
    public bool isCrouching = false;
    private bool canCrouch = false;
    private Transform currentDesk;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (canCrouch)
            {
                if (isCrouching)
                {
                    // ���̉��ŃX�y�[�X���������ꍇ�̓��ʂȏ���
                    HandleUnderDeskAction();
                }
                else
                {
                    StartCrouch();
                }
            }
            else if (isCrouching)
            {
                StopCrouch();
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("HidableDesk"))
        {
            canCrouch = true;
            currentDesk = other.transform;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("HidableDesk"))
        {
            canCrouch = false;
            currentDesk = null;
            if (isCrouching)
            {
                StopCrouch();
            }
        }
    }

    private void StartCrouch()
    {
        isCrouching = true;
        // ���Ⴊ�݃��[�V������ʒu�����Ȃ�
        Debug.Log("���Ⴊ��");
        // �v���C���[�̈ʒu�𒲐����Ċ��̉��ɉB���悤�ɂ���
        transform.position = new Vector3(transform.position.x, transform.position.y - 0.5f, transform.position.z);
    }

    private void StopCrouch()
    {
        isCrouching = false;
        // �ʏ�̓����ɖ߂�
        Debug.Log("���Ⴊ�݉���");
        transform.position = new Vector3(transform.position.x, transform.position.y + 0.5f, transform.position.z);
    }

    private void HandleUnderDeskAction()
    {
        Debug.Log("���̉��ŃX�y�[�X�����������̏���");
        // ���ʂȏ����������Ŏ���
    }
}
