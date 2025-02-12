using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerHealth : MonoBehaviour
{
    public int Health = 100; // Player's starting health

    public void TakeDamage(int damage)
    {
        Health -= damage; // Reduce health by the damage amount
        Debug.Log($"Player took {damage} damage. Health: {Health}");

        // Check if the player has died
        if (Health <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        Debug.Log("Player has died. Restarting scene...");

        // Restart the current scene
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}