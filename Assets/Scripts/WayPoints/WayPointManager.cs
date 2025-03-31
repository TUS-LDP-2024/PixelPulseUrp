using UnityEngine;

public class WaypointManager : MonoBehaviour
{
    public Transform[] waypointPositions;  // Set these in the Inspector
    public GameObject waypointPrefab;       // The visual marker prefab
    private int currentWaypointIndex = 0;
    private GameObject currentWaypointMarker;

    void Start()
    {
        if (waypointPositions.Length > 0)
        {
            // Instantiate the first waypoint marker
            currentWaypointMarker = Instantiate(waypointPrefab, waypointPositions[currentWaypointIndex].position, Quaternion.identity);
        }
    }

    // This method is called when the player triggers the current waypoint
    public void AdvanceWaypoint()
    {
        Destroy(currentWaypointMarker);
        currentWaypointIndex++;
        if (currentWaypointIndex < waypointPositions.Length)
        {
            currentWaypointMarker = Instantiate(waypointPrefab, waypointPositions[currentWaypointIndex].position, Quaternion.identity);
        }
        else
        {
            Debug.Log("All waypoints completed!");
            // Optionally, trigger an event that the level is complete or the next area is reached.
        }
    }
}
