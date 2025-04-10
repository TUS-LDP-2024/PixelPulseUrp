using UnityEngine;

public class WaypointTrigger : MonoBehaviour
{
    public WaypointManager waypointManager; 

    void Start()
    {
        if (waypointManager == null)
        {
            Debug.LogWarning("WaypointManager not assigned! Please assign it in the Inspector.");
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && waypointManager != null)
        {
            waypointManager.AdvanceWaypoint();
        }
    }
}
