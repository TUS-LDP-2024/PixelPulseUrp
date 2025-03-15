using UnityEngine;

public class DoorMover : MonoBehaviour
{
    public enum DoorAxis
    {
        LocalX,
        LocalY,
        LocalZ
    }

    [Header("Trigger Settings")]
    [Tooltip("Assign the trigger collider (must be set as 'Is Trigger') that detects the player.")]
    public Collider triggerZone;

    [Header("Movement Settings")]
    [Tooltip("Which local axis should the door move along?")]
    public DoorAxis doorAxis = DoorAxis.LocalX;

    [Tooltip("Check to move in the positive direction of the chosen axis, uncheck for negative.")]
    public bool moveRight = true;

    [Tooltip("Distance the door moves when opening.")]
    public float moveDistance = 3f;

    [Tooltip("Speed at which the door moves.")]
    public float moveSpeed = 5f;

    private Vector3 closedPosition;
    private Vector3 openPosition;
    private bool playerDetected;

    void Start()
    {
        // Record the door's starting (closed) position
        closedPosition = transform.position;

        // Pick the local axis direction based on the enum
        Vector3 axisDirection;
        switch (doorAxis)
        {
            case DoorAxis.LocalX:
                axisDirection = transform.right;   // local X
                break;
            case DoorAxis.LocalY:
                axisDirection = transform.up;      // local Y
                break;
            case DoorAxis.LocalZ:
            default:
                axisDirection = transform.forward; // local Z
                break;
        }

        // If moveRight is false, invert the direction
        if (!moveRight)
        {
            axisDirection = -axisDirection;
        }

        // Calculate the door's open position
        openPosition = closedPosition + axisDirection * moveDistance;
    }

    void Update()
    {
        bool isPlayerInTrigger = CheckForPlayer();

        if (isPlayerInTrigger && !playerDetected)
        {
            Debug.Log("Player entered trigger zone for door: " + gameObject.name);
        }
        else if (!isPlayerInTrigger && playerDetected)
        {
            Debug.Log("Player exited trigger zone for door: " + gameObject.name);
        }
        playerDetected = isPlayerInTrigger;

        // Move the door toward the open or closed position based on detection
        Vector3 targetPosition = playerDetected ? openPosition : closedPosition;
        transform.position = Vector3.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);
    }

    // Check for player within the trigger using Physics.OverlapBox
    bool CheckForPlayer()
    {
        if (triggerZone == null)
        {
            Debug.LogWarning("Trigger zone not assigned on " + gameObject.name);
            return false;
        }

        Collider[] hits = Physics.OverlapBox(triggerZone.bounds.center,
                                            triggerZone.bounds.extents,
                                            triggerZone.transform.rotation);
        foreach (Collider hit in hits)
        {
            if (hit.CompareTag("Player"))
            {
                return true;
            }
        }
        return false;
    }

    // Draw the trigger zone in the editor for visualization
    void OnDrawGizmosSelected()
    {
        if (triggerZone != null)
        {
            Gizmos.color = Color.yellow;
            Gizmos.matrix = triggerZone.transform.localToWorldMatrix;
            BoxCollider box = triggerZone as BoxCollider;
            if (box != null)
            {
                Gizmos.DrawWireCube(box.center, box.size);
            }
        }
    }
}
