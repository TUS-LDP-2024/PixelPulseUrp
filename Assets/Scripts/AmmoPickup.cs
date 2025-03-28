using UnityEngine;

public class AmmoPickup : MonoBehaviour
{
    public int ammoRestored = 20;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("PlayerCollider"))
        {
            var playerShooting = other.GetComponentInParent<PlayerShooting>();
            if (playerShooting != null)
            {
                playerShooting.AddAmmo(ammoRestored);
                Destroy(gameObject);
            }
        }
    }
}