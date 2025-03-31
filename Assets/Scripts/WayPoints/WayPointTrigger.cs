using UnityEngine;

public class WaypointTrigger : MonoBehaviour
{
    private WaypointManager waypointManager;

    void Start()
    {
        // Assume the manager is in the scene with a tag "WaypointManager"
        waypointManager = GameObject.FindWithTag("WaypointManager").GetComponent<WaypointManager>();
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            waypointManager.AdvanceWaypoint();
        }
    }
}
