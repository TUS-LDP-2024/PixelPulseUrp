using UnityEngine;
using UnityEngine.AI;

public class EnemyHealth : MonoBehaviour
{
    [Header("Base Health")]
    public float baseHealth = 100f;
    private float currentHealth;
    private float maxHealth;

    public delegate void DeathEvent(GameObject zombie);
    public event DeathEvent OnDeath;

    [Header("Ammo Drop")]
    public GameObject ammoPickup;
    public float ammoDropChance = 0.9f;

    [Header("Ragdoll Settings")]
    public float ragdollDuration = 3f;
    private Animator animator;
    private NavMeshAgent agent;
    private Rigidbody[] ragdollRigidbodies;
    private Collider[] ragdollColliders;

    private void Start()
    {
        // If you have a RoundManager, adjust enemy health per round.
        if (RoundManager.Instance != null)
        {
            UpdateHealthForRound(RoundManager.Instance.currentRound);
        }
        else
        {
            maxHealth = baseHealth;
            currentHealth = maxHealth;
        }

        animator = GetComponentInChildren<Animator>();
        agent = GetComponent<NavMeshAgent>();

        ragdollRigidbodies = GetComponentsInChildren<Rigidbody>();
        ragdollColliders = GetComponentsInChildren<Collider>();

        SetRagdollActive(false);
    }

    public void UpdateHealthForRound(int round)
    {
        // Example scaling: +20% health per round after round 1
        float healthMultiplier = 1 + (round - 1) * 0.2f;
        maxHealth = baseHealth * healthMultiplier;
        currentHealth = maxHealth;
    }

    /// <summary>
    /// Called by guns or any direct-damage source. Uses the passed-in damage value.
    /// </summary>
    public void TakeDamage(float damage)
    {
        currentHealth -= damage;
        if (currentHealth <= 0)
        {
            Die();
        }
    }

   
    /// Called by grenades
    /// biased toward the high end (90–200).
    
    public void TakeRandomDamage()
    {
        // Uniform random [0..1], then take sqrt to bias it toward 1.
        float randomValue = Random.value;          // uniform in [0,1]
        float biasedValue = Mathf.Sqrt(randomValue);
        float randomDamage = Mathf.Lerp(90f, 200f, biasedValue);

        currentHealth -= randomDamage;
        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        OnDeath?.Invoke(gameObject);

        if (animator != null) animator.enabled = false;
        if (agent != null) agent.enabled = false;

        SetRagdollActive(true);

        // Chance to drop ammo
        if (Random.value < ammoDropChance && ammoPickup != null)
        {
            Instantiate(ammoPickup, transform.position + Vector3.down * 0.8f, Quaternion.identity);
        }

        // Destroy this GameObject after letting ragdoll persist
        Destroy(gameObject, ragdollDuration);
    }

    /// <summary>
    /// Enables or disables ragdoll colliders and rigidbodies.
    /// </summary>
    private void SetRagdollActive(bool isActive)
    {
        foreach (var rb in ragdollRigidbodies)
        {
            rb.isKinematic = !isActive;
            rb.detectCollisions = isActive;
        }

        foreach (var col in ragdollColliders)
        {
            col.enabled = isActive;
        }

        // Disable the main collider & rigidbody so they don't conflict with ragdoll
        var mainCollider = GetComponent<Collider>();
        if (mainCollider != null) mainCollider.enabled = !isActive;

        var mainRigidbody = GetComponent<Rigidbody>();
        if (mainRigidbody != null) mainRigidbody.isKinematic = isActive;
    }
}
