using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class PlayerHealth : MonoBehaviour
{
    public float maxHealth = 100f;
    public float regenRate = 0f; // Health per second regeneration rate
    private float currentHealth;

    // Reference to the primary UI Image for the blood overlay effect.
    public Image bloodOverlayImage;
    // Additional canvas image for the blood overlay effect (capped max alpha).
    public Image bloodOverlayImage2;

    // Reference to the UIManager for flash effects.
    private UIManager uiManager;

    // Delay between damage instances in seconds.
    private bool canTakeDamage = true;
    private float damageCooldown = 1.5f;

    private void Start()
    {
        currentHealth = maxHealth; // Initialize health
        // Find the UIManager in the scene.
        uiManager = FindObjectOfType<UIManager>();
    }

    private void Update()
    {
        // Regenerate health if below 15% of maxHealth.
        if (currentHealth < maxHealth * 0.15f)
        {
            currentHealth += regenRate * Time.deltaTime;
            // Clamp health to 15% maximum when regenerating.
            if (currentHealth > maxHealth * 0.15f)
            {
                currentHealth = maxHealth * 0.15f;
            }
        }

        UpdateBloodOverlay();
    }

    public void TakeDamage(float damage)
    {
        // Only allow damage if cooldown has elapsed.
        if (!canTakeDamage)
        {
            return;
        }

        currentHealth -= damage;
        Debug.Log($"Player took {damage} damage! Current Health: {currentHealth}");

        // Trigger the damage flash effect via UIManager.
        if (uiManager != null)
        {
            uiManager.DamageFlash();
        }

        if (currentHealth <= 0)
        {
            Die();
        }

        // Start damage cooldown.
        canTakeDamage = false;
        StartCoroutine(DamageCooldown());
    }

    // Coroutine to reset damage reception after cooldown.
    private IEnumerator DamageCooldown()
    {
        yield return new WaitForSeconds(damageCooldown);
        canTakeDamage = true;
    }

    // Add health but never exceed maxHealth.
    public void AddHealth(float amount)
    {
        currentHealth += amount;
        if (currentHealth > maxHealth)
        {
            currentHealth = maxHealth;
        }
        Debug.Log($"Player healed by {amount}! Current Health: {currentHealth}");
    }

    private void Die()
    {
        Debug.Log("Player has died! Loading main menu...");
        SceneManager.LoadScene("EndGameMenu");
    }

    private void UpdateBloodOverlay()
    {
        // Calculate thresholds in absolute values.
        float halfHealth = maxHealth * 0.5f;
        float fifteenPercentHealth = maxHealth * 0.15f;
        float baseAlpha = 0f;

        // Only show the overlay when health is below 50%.
        if (currentHealth < halfHealth)
        {
            // Mathf.InverseLerp returns 0 when currentHealth equals halfHealth and 1 when it equals fifteenPercentHealth.
            baseAlpha = Mathf.InverseLerp(halfHealth, fifteenPercentHealth, currentHealth);
            baseAlpha = Mathf.Clamp01(baseAlpha);
        }
        else
        {
            baseAlpha = 0f;
        }

        // For the primary image, use a PingPong pulse (with a 1.5× multiplier).
        float alpha1 = baseAlpha;
        if (currentHealth <= fifteenPercentHealth)
        {
            float ping = Mathf.PingPong(Time.time * 1.5f, 1f);
            float smoothPing = Mathf.SmoothStep(0f, 1f, ping);
            // Center around zero and scale by amplitude.
            float pulse = (smoothPing - 0.5f) * 0.8f;
            // Lower the minimum pulse value to -0.7 for a lower minimum alpha.
            pulse = Mathf.Clamp(pulse, -0.7f, 0.2f);
            alpha1 = Mathf.Clamp01(baseAlpha + pulse);
        }

        // For the secondary image, use a slower PingPong pulse.
        float alpha2 = baseAlpha;
        if (currentHealth <= fifteenPercentHealth)
        {
            float ping = Mathf.PingPong(Time.time * 1f, 1f);
            float smoothPing = Mathf.SmoothStep(0.3f, 1f, ping);
            float pulse = (smoothPing - 0.5f) * 0.8f;
            pulse = Mathf.Clamp(pulse, -0.5f, 0.2f);
            alpha2 = Mathf.Clamp01(baseAlpha + pulse);
        }

        // Apply the computed alpha to the primary blood overlay image.
        if (bloodOverlayImage != null)
        {
            Color color = bloodOverlayImage.color;
            color.a = alpha1;
            bloodOverlayImage.color = color;
        }

        // For the additional blood overlay, cap the maximum alpha at 25/255.
        if (bloodOverlayImage2 != null)
        {
            Color color2 = bloodOverlayImage2.color;
            color2.a = alpha2 * (25f / 255f);
            bloodOverlayImage2.color = color2;
        }
    }
    public void IncreaseMaxHealth(float amount)
    {
        maxHealth += amount;
        currentHealth += amount; // Also heal the player
        Debug.Log($"Max health increased to {maxHealth}");
    }

    public void IncreaseRegenRate(float amount)
    {
        regenRate += amount;
        Debug.Log($"Regen rate increased to {regenRate}/sec");
    }
}
