using UnityEngine;

public class CrouchRestrictionZone : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerController playerController = other.GetComponent<PlayerController>();
            if (playerController != null)
            {
                playerController.SetCrouchRestriction(true); // ���Ⴊ�݉����𖳌���
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerController playerController = other.GetComponent<PlayerController>();
            if (playerController != null)
            {
                playerController.SetCrouchRestriction(false); // ���Ⴊ�݉������ĂїL����
            }
        }
    }
}
