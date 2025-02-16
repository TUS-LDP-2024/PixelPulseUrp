using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class DoorController : MonoBehaviour
{
    [Header("Door Settings")]
    public int doorCost = 100;      // Points required to open this door
    public float openAngle = 90f;   // Total angle to rotate when opened (in degrees)
    public float openSpeed = 90f;   // Rotation speed in degrees per second
    public Transform pivot;         // The pivot point from which the door rotates. If null, rotates around its own transform.
    public bool ReverseOpening = false;  // Check this to reverse the opening direction

    [Header("UI Settings")]
    public Text doorCostUIText;     // UI element to display the door cost when the player is nearby

    private bool isOpen = false;
    private bool isPlayerNearby = false;
    private PointsManager playerPoints;  // Reference to the player's PointsManager

    void Update()
    {
        // If the door is already open, disable further interaction and clear UI.
        if (isOpen)
        {
            if (doorCostUIText != null)
            {
                doorCostUIText.text = "";
            }
            return;
        }

        // Update the door cost UI when the player is nearby.
        if (isPlayerNearby && doorCostUIText != null)
        {
            doorCostUIText.text = "Door Cost: " + doorCost.ToString();
        }

        // Check for interaction when the player presses "E"
        if (isPlayerNearby && Input.GetKeyDown(KeyCode.E))
        {
            if (playerPoints != null)
            {
                Debug.Log("Attempting door open: Player points = " + playerPoints.points + ", Door cost = " + doorCost);
                if (playerPoints.points >= doorCost)
                {
                    if (playerPoints.SpendPoints(doorCost))
                    {
                        Debug.Log("Door opened successfully.");
                        OpenDoor();
                        // Clear the door cost UI after opening the door.
                        if (doorCostUIText != null)
                        {
                            doorCostUIText.text = "";
                        }
                    }
                }
                else
                {
                    Debug.Log("Not enough points to open door! Player points = " + playerPoints.points + ", required = " + doorCost);
                }
            }
        }
    }

    private void OpenDoor()
    {
        if (!isOpen)
        {
            StartCoroutine(OpenDoorRoutine());
        }
    }

    private IEnumerator OpenDoorRoutine()
    {
        isOpen = true; // Disable further interactions once the door begins opening.
        float rotatedAngle = 0f;
        float targetAngle = Mathf.Abs(openAngle); // Use absolute value for comparison
        int direction = ReverseOpening ? -1 : 1;    // Determine rotation direction based on checkbox

        while (rotatedAngle < targetAngle)
        {
            float angleStep = openSpeed * Time.deltaTime;
            if (rotatedAngle + angleStep > targetAngle)
            {
                angleStep = targetAngle - rotatedAngle;
            }
            angleStep *= direction; // Apply the direction factor

            if (pivot != null)
            {
                // Rotate around the specified pivot point.
                transform.RotateAround(pivot.position, pivot.up, angleStep);
            }
            else
            {
                // Fallback: rotate around the door's own transform.
                transform.Rotate(0, angleStep, 0);
            }
            rotatedAngle += Mathf.Abs(angleStep);
            yield return null;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        // Do nothing if the door is already open.
        if (isOpen)
            return;

        if (other.CompareTag("Player"))
        {
            // Attempt to retrieve the player's PointsManager.
            playerPoints = other.GetComponent<PointsManager>() ?? other.GetComponentInChildren<PointsManager>();
            isPlayerNearby = true;
            if (doorCostUIText != null)
            {
                doorCostUIText.text = "Door Cost: " + doorCost.ToString();
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerNearby = false;
            playerPoints = null;
            if (doorCostUIText != null)
            {
                doorCostUIText.text = "";
            }
        }
    }
}
