using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class PlayerHealth : MonoBehaviour
{
    [Header("Health Settings")]
    public float maxHealth = 100f;
    [SerializeField] private float currentHealth;
    [SerializeField] private float regenRate = 0f;
    [SerializeField] private float damageReduction = 0f;

    [Header("Visual Effects")]
    public Image bloodOverlayImage;
    public Image bloodOverlayImage2;

    [Header("Gameplay Settings")]
    public float regenDelay = 5f;
    private float timeSinceLastDamage;
    private bool isRegenerating;
    private bool canTakeDamage = true;
    private float damageCooldown = 1.5f;

    private void Awake()
    {
        currentHealth = maxHealth;
        timeSinceLastDamage = 0f;
    }

    private void Update()
    {
        timeSinceLastDamage += Time.deltaTime;

        if (CanRegenerate())
        {
            if (!isRegenerating)
            {
                Debug.Log("Regeneration started");
                isRegenerating = true;
            }
            currentHealth = Mathf.Min(currentHealth + (regenRate * Time.deltaTime), maxHealth);
        }
        else if (isRegenerating)
        {
            isRegenerating = false;
        }

        UpdateBloodOverlay();
    }

    private bool CanRegenerate()
    {
        return regenRate > 0 &&
               currentHealth < maxHealth &&
               timeSinceLastDamage >= regenDelay;
    }

    public void TakeDamage(float damage)
    {
        if (!canTakeDamage) return;

        timeSinceLastDamage = 0f;
        isRegenerating = false;

        float reducedDamage = damage * (1f - damageReduction);
        currentHealth -= reducedDamage;

        if (currentHealth <= 0)
        {
            Die();
        }
        else
        {
            canTakeDamage = false;
            StartCoroutine(DamageCooldown());
        }
    }

    private IEnumerator DamageCooldown()
    {
        yield return new WaitForSeconds(damageCooldown);
        canTakeDamage = true;
    }

    public void IncreaseMaxHealth(float amount)
    {
        maxHealth += amount;
        currentHealth += amount;
        Debug.Log($"Max health increased to {maxHealth}");
    }

    public void IncreaseRegenRate(float amount)
    {
        regenRate += amount;
        Debug.Log($"Regen rate increased to {regenRate} HP/s");
    }

    public void IncreaseDamageReduction(float amount)
    {
        damageReduction = Mathf.Clamp(damageReduction + amount, 0f, 0.8f);
        Debug.Log($"Damage reduction now: {damageReduction * 100}%");
    }

    private void Die()
    {
        Debug.Log("Player has died! Loading main menu...");
        SceneManager.LoadScene("EndGameMenu");
    }

    private void UpdateBloodOverlay()
    {
        float healthPercent = currentHealth / maxHealth;
        float baseAlpha = 0f;

        // Only show blood when below 50% health
        if (healthPercent < 0.5f)
        {
            baseAlpha = Mathf.InverseLerp(0.5f, 0.15f, healthPercent);
            baseAlpha = Mathf.Clamp01(baseAlpha);
        }

        // Primary overlay - pulsating effect when very low health
        float alpha1 = baseAlpha;
        if (healthPercent <= 0.15f)
        {
            float ping = Mathf.PingPong(Time.time * 1.5f, 1f);
            float smoothPing = Mathf.SmoothStep(0f, 1f, ping);
            float pulse = (smoothPing - 0.5f) * 0.8f;
            pulse = Mathf.Clamp(pulse, -0.7f, 0.2f);
            alpha1 = Mathf.Clamp01(baseAlpha + pulse);
        }

        // Secondary overlay - more subtle effect
        float alpha2 = baseAlpha;
        if (healthPercent <= 0.15f)
        {
            float ping = Mathf.PingPong(Time.time * 1f, 1f);
            float smoothPing = Mathf.SmoothStep(0.3f, 1f, ping);
            float pulse = (smoothPing - 0.5f) * 0.8f;
            pulse = Mathf.Clamp(pulse, -0.5f, 0.2f);
            alpha2 = Mathf.Clamp01(baseAlpha + pulse);
        }

        // Apply to UI
        if (bloodOverlayImage != null)
        {
            Color color = bloodOverlayImage.color;
            color.a = alpha1;
            bloodOverlayImage.color = color;
        }

        if (bloodOverlayImage2 != null)
        {
            Color color2 = bloodOverlayImage2.color;
            color2.a = alpha2 * (25f / 255f); // Capped at ~10% opacity
            bloodOverlayImage2.color = color2;
        }
    }

    public void AddHealth(float amount)
    {
        currentHealth = Mathf.Min(currentHealth + amount, maxHealth);
        Debug.Log($"Added {amount} health. Current: {currentHealth}/{maxHealth}");
    }

    // For debugging
    public float GetCurrentHealth() => currentHealth;
    public float GetHealthPercent() => currentHealth / maxHealth;
}