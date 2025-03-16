using UnityEngine;
using UnityEngine.AI;

public class EnemyHealth : MonoBehaviour
{
    public float baseHealth = 100f; // Base health for round 1
    private float currentHealth;
    private float maxHealth; // Declare maxHealth as a private variable

    public delegate void DeathEvent(GameObject zombie); // Define the event delegate
    public event DeathEvent OnDeath; // Define the event
    public GameObject ammoPickup;
    public float ammoDropChance = 0.9f;

    [Header("Ragdoll Settings")]
    public float ragdollDuration = 3f; // Time to ragdoll before destruction
    private Animator animator; // Animator in the child object
    private NavMeshAgent agent;
    private Rigidbody[] ragdollRigidbodies; // Array of Rigidbodies for ragdoll
    private Collider[] ragdollColliders; // Array of Colliders for ragdoll

    private void Start()
    {
        // Initialize health based on the current round
        UpdateHealthForRound(ZombieSpawner.currentRound);

        // Get references to components
        animator = GetComponentInChildren<Animator>(); // Get Animator in child object
        agent = GetComponent<NavMeshAgent>();

        // Get all Rigidbody and Collider components in the ragdoll
        ragdollRigidbodies = GetComponentsInChildren<Rigidbody>();
        ragdollColliders = GetComponentsInChildren<Collider>();

        // Disable ragdoll physics at the start
        SetRagdollActive(false);
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

        // Disable the Animator (in the child object) and NavMeshAgent
        if (animator != null)
        {
            animator.enabled = false;
        }
        if (agent != null)
        {
            agent.enabled = false;
        }

        // Enable ragdoll physics
        SetRagdollActive(true);

        // Chance to drop ammo
        var result = Random.Range(0f, 1f);
        if (result < ammoDropChance)
        {
            var pickup = Instantiate(ammoPickup);
            pickup.transform.position = gameObject.transform.position + new Vector3(0f, -0.8f, 0f);
        }

        // Destroy the zombie after a delay
        Destroy(gameObject, ragdollDuration);
    }

    // Enable or disable ragdoll physics
    private void SetRagdollActive(bool isActive)
    {
        // Enable/disable all Rigidbodies in the ragdoll
        foreach (var rb in ragdollRigidbodies)
        {
            rb.isKinematic = !isActive; // Enable physics if active
            rb.detectCollisions = isActive; // Enable collisions if active
        }

        // Enable/disable all Colliders in the ragdoll
        foreach (var col in ragdollColliders)
        {
            col.enabled = isActive; // Enable colliders if active
        }

        // Disable the main Collider and Rigidbody (if any)
        var mainCollider = GetComponent<Collider>();
        if (mainCollider != null)
        {
            mainCollider.enabled = !isActive;
        }

        var mainRigidbody = GetComponent<Rigidbody>();
        if (mainRigidbody != null)
        {
            mainRigidbody.isKinematic = isActive;
        }
    }
}