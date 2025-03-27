using UnityEngine;
using UnityEngine.AI;

public class EnemyHealth : MonoBehaviour
{
    public float baseHealth = 100f;
    private float currentHealth;
    private float maxHealth;

    public delegate void DeathEvent(GameObject zombie);
    public event DeathEvent OnDeath;
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
        float healthMultiplier = 1 + (round - 1) * 0.2f;
        maxHealth = baseHealth * healthMultiplier;
        currentHealth = maxHealth;
    }

    public void TakeDamage(float damage)
    {
        currentHealth -= damage;
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

        if (Random.value < ammoDropChance)
        {
            Instantiate(ammoPickup, transform.position + Vector3.down * 0.8f, Quaternion.identity);
        }

        Destroy(gameObject, ragdollDuration);
    }

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

        var mainCollider = GetComponent<Collider>();
        if (mainCollider != null) mainCollider.enabled = !isActive;

        var mainRigidbody = GetComponent<Rigidbody>();
        if (mainRigidbody != null) mainRigidbody.isKinematic = isActive;
    }
}