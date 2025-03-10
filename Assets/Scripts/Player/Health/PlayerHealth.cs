using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PlayerHealth : MonoBehaviour
{
    public float maxHealth = 100f;
    public float regenRate = 0f; // Health per second regeneration rate
    private float currentHealth;

    // Reference to the UI Image for the blood overlay effect.
    public Image bloodOverlayImage;

    private void Start()
    {
        currentHealth = maxHealth / 10; // Initialize health
    }

    private void Update()
    {
        // Regenerate health if below 50% of maxHealth
        if (currentHealth < maxHealth * 0.5f)
        {
            currentHealth += regenRate * Time.deltaTime;
            // Clamp health to 50% maximum when regenerating
            if (currentHealth > maxHealth * 0.5f)
            {
                currentHealth = maxHealth * 0.5f;
            }
        }

        UpdateBloodOverlay();
    }

    public void TakeDamage(float damage)
    {
        currentHealth -= damage;
        Debug.Log($"Player took {damage} damage! Current Health: {currentHealth}");

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        Debug.Log("Player has died! Loading main menu...");
        // Load the end-game or main menu scene
        SceneManager.LoadScene("EndGameMenu");
    }

    private void UpdateBloodOverlay()
    {
        // Calculate thresholds in absolute values.
        float halfHealth = maxHealth * 0.5f;
        float fifteenPercentHealth = maxHealth * 0.15f;
        float alpha = 0f;

        // Only show the overlay when health is below 50%
        if (currentHealth < halfHealth)
        {
            // Mathf.InverseLerp returns 0 when currentHealth == halfHealth and 1 when currentHealth == fifteenPercentHealth.
            alpha = Mathf.InverseLerp(halfHealth, fifteenPercentHealth, currentHealth);
            alpha = Mathf.Clamp01(alpha);

            // If health is at or below 15%, add a pulsing effect.
            if (currentHealth <= fifteenPercentHealth)
            {
                // pulse oscillates between -0.1 and 0.1; adjust frequency and amplitude as needed.
                float pulse = 0.8f * Mathf.Sin(Time.time * 2.2f);
                pulse = Mathf.Clamp(pulse, -0.5f, 0.2f); // Clamp the pulse effect within a safe range
                alpha = Mathf.Clamp01(alpha + pulse);
            }
        }

        else
        {
            alpha = 0f;
        }

        // Apply the computed alpha to the blood overlay image.
        if (bloodOverlayImage != null)
        {
            Color color = bloodOverlayImage.color;
            color.a = alpha;
            bloodOverlayImage.color = color;
        }
    }
}

