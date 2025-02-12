using System;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class Zombie : MonoBehaviour
{
    private Transform Target; // The player's transform
    private NavMeshAgent Agent;
    public event Action<GameObject> OnDeath;

    [Header("Combat Settings")]
    public float AttackRange = 2f; // Range within which the zombie can attack
    public float AttackCooldown = 1f; // Time between attacks
    private float _lastAttackTime; // Time when the last attack occurred

    private void Awake()
    {
        Agent = GetComponent<NavMeshAgent>();
    }

    private void Start()
    {
        FindPlayer(); // Find the player when spawned
        _lastAttackTime = -AttackCooldown; // Allow immediate attack
    }

    private void Update()
    {
        if (Target != null && Agent != null && Agent.enabled)
        {
            // Calculate distance to the player
            float distanceToPlayer = Vector3.Distance(transform.position, Target.position);

            // Chase the player if they are within detection range
            Agent.SetDestination(Target.position);

            // Attack the player if they are within attack range and the cooldown has passed
            if (distanceToPlayer <= AttackRange && Time.time >= _lastAttackTime + AttackCooldown)
            {
                Attack();
            }
        }
    }

    private void FindPlayer()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            Target = player.transform;
        }
        else
        {
            Debug.LogError("⚠ Player not found! Make sure the player has the 'Player' tag.");
        }
    }

    private void Attack()
{
    Debug.Log("Attack");

    // Damage the player
    PlayerHealth playerHealth = Target.GetComponent<PlayerHealth>();
    if (playerHealth != null)
    {
        playerHealth.TakeDamage(10); // Example: Deal 10 damage
    }

    _lastAttackTime = Time.time;
}

    public void Die()
    {
        OnDeath?.Invoke(gameObject); // Trigger the OnDeath event
        Destroy(gameObject); // Destroy the zombie
    }
}

