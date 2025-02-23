using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    public float baseHealth = 100f; // Base health for round 1
    private float currentHealth;
    private float maxHealth; // Declare maxHealth as a private variable

    public delegate void DeathEvent(GameObject zombie); // Define the event delegate
    public event DeathEvent OnDeath; // Define the event

    private void Start()
    {
        // Initialize health based on the current round
        UpdateHealthForRound(ZombieSpawner.currentRound);
    }

    // Update the zombie's health based on the current round
    public void UpdateHealthForRound(int round)
    {
        // Increase health by 20% for each round (adjust the formula as needed)
        float healthMultiplier = 1 + (round - 1) * 0.2f; // Round 1: 1x, Round 2: 1.2x, Round 3: 1.4x, etc.
        maxHealth = baseHealth * healthMultiplier;

        // Reset current health to the new max health
        currentHealth = maxHealth;

        Debug.Log($"Zombie health updated for round {round}: Max Health = {maxHealth}");
    }

    public void TakeDamage(float damage)
    {
        currentHealth -= damage;
        Debug.Log($"Enemy took {damage} damage! Current Health: {currentHealth}");

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        Debug.Log("Enemy eliminated!");

        // Trigger the OnDeath event before destroying the GameObject
        OnDeath?.Invoke(gameObject);

        Destroy(gameObject); // Remove enemy from the scene
    }
}