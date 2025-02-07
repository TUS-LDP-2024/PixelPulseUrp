using System;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class Zombie : MonoBehaviour // ✅ Fixed class name
{
    private Transform Target; // The player's transform
    private NavMeshAgent Agent;
    public event Action<GameObject> OnDeath;

    private void Awake()
    {
        Agent = GetComponent<NavMeshAgent>(); 
    }

    private void Start()
    {
        FindPlayer(); // Find the player when spawned
    }

    private void Update()
    {
        if (Target != null && Agent != null && Agent.enabled)
        {
            Agent.SetDestination(Target.position); // ✅ Keeps tracking player movement
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

    public void Die()
    {
        OnDeath?.Invoke(gameObject); // ✅ Safer null check
        Destroy(gameObject);
    }
}

