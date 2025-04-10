using UnityEngine;

public class WaypointManager : MonoBehaviour
{
    public Transform[] waypointPositions;
    public GameObject waypointPrefab;
    private int currentWaypointIndex = 0;
    private GameObject currentWaypointMarker;

    void Start()
    {
        if (waypointPositions.Length > 0)
        {
            currentWaypointMarker = Instantiate(waypointPrefab, waypointPositions[currentWaypointIndex].position, Quaternion.identity);
        }
    }

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
        }

        if (TutorialManager.Instance != null && TutorialManager.Instance.IsTutorialActive)
        {
            TutorialManager.Instance.OnTutorialWaypointReached(currentWaypointIndex);
        }
    }
}
