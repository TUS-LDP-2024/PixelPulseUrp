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
    [Tooltip("Assign the trigger collider (must be set as 'Is Trigger') that detects the player or enemy.")]
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
    private bool triggerDetected;

    void Start()
    {
        // Record the door's starting (closed) position
        closedPosition = transform.position;

        // Pick the local axis direction based on the enum
        Vector3 axisDirection;
        switch (doorAxis)
        {
            case DoorAxis.LocalX:
                axisDirection = transform.right;
                break;
            case DoorAxis.LocalY:
                axisDirection = transform.up;
                break;
            case DoorAxis.LocalZ:
            default:
                axisDirection = transform.forward;
                break;
        }

        // Invert the direction if needed
        if (!moveRight)
        {
            axisDirection = -axisDirection;
        }

        // Calculate the door's open position
        openPosition = closedPosition + axisDirection * moveDistance;
    }

    void Update()
    {
        bool isTriggered = CheckForTrigger();

        if (isTriggered && !triggerDetected)
        {
            Debug.Log("Trigger entered for door: " + gameObject.name);
        }
        else if (!isTriggered && triggerDetected)
        {
            Debug.Log("Trigger exited for door: " + gameObject.name);
        }
        triggerDetected = isTriggered;

        // Move the door toward the open or closed position based on detection
        Vector3 targetPosition = triggerDetected ? openPosition : closedPosition;
        transform.position = Vector3.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);
    }

    // Check for objects tagged "Player" or "Enemy" within the trigger zone.
    bool CheckForTrigger()
    {
        if (triggerZone == null)
        {
            Debug.LogWarning("Trigger zone not assigned on " + gameObject.name);
            return false;
        }

        Collider[] hits = Physics.OverlapBox(triggerZone.bounds.center, triggerZone.bounds.extents, triggerZone.transform.rotation);
        foreach (Collider hit in hits)
        {
            if (hit.CompareTag("Player") || hit.CompareTag("Enemy"))
            {
                Debug.Log("Detected trigger object: " + hit.name);
                return true;
            }
        }
        return false;
    }

    // Draw the trigger zone in the editor for visualization.
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
