using UnityEngine;

public class HealthPack : MonoBehaviour
{
    // Toggle to choose between 25 HP (default) and 50 HP.
    public bool isLarge = false;
    private int healthAmount;

    // Optionally, cache a reference to your UIManager (which should be present in your scene).
    private UIManager uiManager;

    private void Start()
    {
        healthAmount = isLarge ? 50 : 25;
        // Find the UIManager in the scene.
        uiManager = FindObjectOfType<UIManager>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // Add health to the player (ensuring it doesn't exceed maxHealth).
            PlayerHealth playerHealth = other.GetComponent<PlayerHealth>();
            if (playerHealth != null)
            {
                playerHealth.AddHealth(healthAmount);
            }

            // Trigger the flash effect via the UIManager.
            if (uiManager != null)
            {
                uiManager.Flash();
            }

            // Remove the health pack from the scene.
            Destroy(gameObject);
        }
    }
}
        