using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    public float maxHealth = 100f;
    private float currentHealth;

    public delegate void DeathEvent(GameObject zombie); // Define the event delegate
    public event DeathEvent OnDeath; // Define the event

    private void Start()
    {
        currentHealth = maxHealth; // Set enemy health to 100 at start
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