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
                playerController.SetCrouchRestriction(true); // ‚µ‚á‚ª‚İ‰ğœ‚ğ–³Œø‰»
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
                playerController.SetCrouchRestriction(false); // ‚µ‚á‚ª‚İ‰ğœ‚ğÄ‚Ñ—LŒø‰»
            }
        }
    }
}
