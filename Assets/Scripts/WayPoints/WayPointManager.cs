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
            SpawnWaypointMarker(currentWaypointIndex);
        }
    }

    public void AdvanceWaypoint()
    {
        Destroy(currentWaypointMarker);
        currentWaypointIndex++;

        if (currentWaypointIndex < waypointPositions.Length)
        {
            SpawnWaypointMarker(currentWaypointIndex);
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

    private void SpawnWaypointMarker(int index)
    {
        currentWaypointMarker = Instantiate(
            waypointPrefab,
            waypointPositions[index].position,
            Quaternion.identity
        );

        WaypointTrigger trigger = currentWaypointMarker.GetComponent<WaypointTrigger>();
        if (trigger != null)
        {
            trigger.waypointManager = this;
        }
        else
        {
            Debug.LogWarning("Spawned waypoint prefab is missing a WaypointTrigger component!");
        }
    }
}
