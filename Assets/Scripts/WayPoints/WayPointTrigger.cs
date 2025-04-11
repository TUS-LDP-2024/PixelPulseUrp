using UnityEngine;

public class WaypointTrigger : MonoBehaviour
{
    public WaypointManager waypointManager; // Assigned at runtime by WaypointManager

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && waypointManager != null)
        {
            waypointManager.AdvanceWaypoint();
        }
    }
}
