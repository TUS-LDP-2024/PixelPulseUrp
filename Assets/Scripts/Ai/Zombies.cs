using System.Collections;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class Zombies : MonoBehaviour
{
    public float UpdateSpeed = 0.1f; // Frequency of recalculating path
    private Transform Target; // The player's transform
    private NavMeshAgent Agent;

    private void Awake()
    {
        Agent = GetComponent<NavMeshAgent>(); 
    }

    private void Start()
    {
        FindPlayer(); // Find the player on spawn

        if (Target != null)
        {
            StartCoroutine(FollowTarget());
        }
        else
        {
            Debug.LogError("Player not found! Make sure the player has the 'Player' tag.");
        }
    }

    private void FindPlayer()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            Target = player.transform;
        }
    }

    private IEnumerator FollowTarget()
    {
        WaitForSeconds wait = new WaitForSeconds(UpdateSpeed);
        
        while (Target != null && Agent.enabled)
        {
            Agent.SetDestination(Target.position);
            yield return wait;
        }
    }
}
