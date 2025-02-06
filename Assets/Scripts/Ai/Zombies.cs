using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class Zombies : MonoBehaviour
{
    public Transform Target; // The target the zombie will follow
    public float UpdateSpeed = 0.1f; // How frequently to recalculate path based on Target's position

    private NavMeshAgent Agent;

    private void Awake()
    {
        Agent = GetComponent<NavMeshAgent>(); // Get the NavMeshAgent component
    }

    public void Start()
    {
        StartCoroutine(FollowTarget()); // Start the coroutine to follow the target
    }

    private IEnumerator FollowTarget()
    {
        WaitForSeconds wait = new WaitForSeconds(UpdateSpeed); // Create a wait time based on UpdateSpeed
        
        while (true) // Continuously follow the target
        {
            // Set the destination of the NavMeshAgent to the target's position
            Agent.SetDestination(Target.position); 

            yield return wait; // Wait for the specified time before the next calculation
        }
    }
}
