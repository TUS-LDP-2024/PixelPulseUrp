using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerHealth : MonoBehaviour
{
    public float maxHealth = 100f;
    private float currentHealth;

    private void Start()
    {
        currentHealth = maxHealth; // Initialize health
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

        // Load the main menu scene
        LoadMainMenu();
    }

    private void LoadMainMenu()
    {
        // Replace "MainMenu" with the exact name of your main menu scene
        SceneManager.LoadScene("EndGameMenu");
    }
}