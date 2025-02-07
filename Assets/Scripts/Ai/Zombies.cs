using System.Collections;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class Zombies : MonoBehaviour
{
    private Transform Target; // The player's transform
    private NavMeshAgent Agent;

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
        if (Target != null)
        {
            Agent.SetDestination(Target.position); // Update destination every frame
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
            Debug.LogError("Player not found! Make sure the player has the 'Player' tag.");
        }
    }
}

