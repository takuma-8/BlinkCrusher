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
                    // 机の下でスペースを押した場合の特別な処理
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
        // しゃがみモーションや位置調整など
        Debug.Log("しゃがんだ");
        // プレイヤーの位置を調整して机の下に隠れるようにする
        transform.position = new Vector3(transform.position.x, transform.position.y - 0.5f, transform.position.z);
    }

    private void StopCrouch()
    {
        isCrouching = false;
        // 通常の動きに戻す
        Debug.Log("しゃがみ解除");
        transform.position = new Vector3(transform.position.x, transform.position.y + 0.5f, transform.position.z);
    }

    private void HandleUnderDeskAction()
    {
        Debug.Log("机の下でスペースを押した時の処理");
        // 特別な処理をここで実装
    }
}
