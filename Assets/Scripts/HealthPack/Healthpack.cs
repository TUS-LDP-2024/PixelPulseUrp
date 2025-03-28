using UnityEngine;

public class HealthPack : MonoBehaviour
{
    public bool isLarge = false;
    private int healthAmount;
    private UIManager uiManager;

    private void Start()
    {
        healthAmount = isLarge ? 50 : 25;
        uiManager = FindObjectOfType<UIManager>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerHealth playerHealth = other.GetComponent<PlayerHealth>();
            if (playerHealth != null)
            {
                // Use the public method to add health
                playerHealth.AddHealth(healthAmount);
            }

            if (uiManager != null)
            {
                uiManager.Flash();
            }

            Destroy(gameObject);
        }
    }
}