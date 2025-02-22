using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerShooting : MonoBehaviour
{
    [Header("Raycast Settings")]
    public float range = 100f;
    public LayerMask enemyLayer;

    [Header("Shooting Effects")]
    public GameObject impactEffect;
    public Transform gunBarrel;

    [Header("Points Settings")]
    public PointsManager pointsManager; // Reference to the PointsManager script

    private PlayerInput playerInput;
    private InputAction fireAction;  // Store reference to the action

    private void Awake()
    {
        // Get the PlayerInput component
        playerInput = GetComponent<PlayerInput>();
    }

    private void OnEnable()
    {
        // Ensure the action map "Player" and action "Fire" are properly found
        fireAction = playerInput.actions.FindActionMap("Player").FindAction("Fire");

        if (fireAction != null)
        {
            Debug.Log("Fire action found and assigned successfully.");
            fireAction.performed += ctx => Shoot();
        }
        else
        {
            Debug.LogError("Fire action not found in Player action map!");
        }
    }

    private void OnDisable()
    {
        // Check if fireAction is not null before unsubscribing
        if (fireAction != null)
        {
            fireAction.performed -= ctx => Shoot();
        }
        else
        {
            Debug.LogError("Fire action is null, cannot unsubscribe.");
        }
    }

    private void Shoot()
    {
        // Log that the ray is being fired
        Debug.Log("Ray Fired!");

        RaycastHit hit;

        // Draw the ray to visualize it in the scene
        Debug.DrawRay(gunBarrel.position, gunBarrel.forward * range, Color.blue, 1f);

        // Perform raycast from the gun barrel forward
        if (Physics.Raycast(gunBarrel.position, gunBarrel.forward, out hit, range, enemyLayer))
        {
            Debug.Log("Hit: " + hit.collider.name);

            // Create visual effect for debugging (raycast visualization)
            Debug.DrawLine(gunBarrel.position, hit.point, Color.red, 1f);

            // Look for the EnemyHealth component in the parent hierarchy
            EnemyHealth enemyHealth = hit.collider.GetComponentInParent<EnemyHealth>();

            // If the enemy health component is found, apply damage
            if (enemyHealth != null)
            {
                if (hit.collider.CompareTag("Head"))
                {
                    Debug.Log("Headshot!");
                    enemyHealth.TakeDamage(67);  // Headshot damage

                    // Award 50 points for a headshot
                    if (pointsManager != null)
                    {
                        pointsManager.AddPoints(50);
                    }
                }
                else if (hit.collider.CompareTag("Body"))
                {
                    Debug.Log("Body shot!");
                    enemyHealth.TakeDamage(34);  // Body shot damage

                    // Award 10 points for a body shot
                    if (pointsManager != null)
                    {
                        pointsManager.AddPoints(10);
                    }
                }
            }

            // Instantiate impact effect (e.g., blood splatter) on hit location
            Instantiate(impactEffect, hit.point, Quaternion.LookRotation(hit.normal));
        }
        else
        {
            // No hit, just show the ray visualization
            Debug.DrawRay(gunBarrel.position, gunBarrel.forward * range, Color.green, 1f);
        }
    }
}