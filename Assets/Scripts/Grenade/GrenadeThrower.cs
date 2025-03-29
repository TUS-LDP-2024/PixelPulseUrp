using UnityEngine;
using UnityEngine.InputSystem; // Required for the new Input System

public class GrenadeThrower : MonoBehaviour
{
    [Header("Grenade Settings")]
    public GameObject grenadePrefab;       // Reference to the grenade prefab
    public Transform grenadeSpawnPoint;    // Where the grenade spawns
    public int grenadeCount = 5;           // Starting grenade ammo

    [Header("Throw Settings")]
    public float maxThrowForce = 20f;      // Maximum force of the throw
    public float minThrowForce = 5f;       // Minimum force to start with
    public float forceChargeRate = 10f;    // How fast the force builds per second
    public float upwardModifier = 0.5f;    // Extra upward component

    [Header("Trajectory Visualization")]
    public LineRenderer trajectoryLine;
    public int numTrajectoryPoints = 30;   // How many points to draw
    public float trajectoryTimeStep = 0.1f;// Time interval between points

    [Header("Input")]
    // Reference to your "ThrowGrenade" action from the new Input System
    public InputActionReference throwGrenadeAction;

    private bool isCharging = false;
    private float currentCharge = 10f;

    private void OnEnable()
    {
        // Subscribe to started & canceled events
        throwGrenadeAction.action.started += OnThrowStarted;
        throwGrenadeAction.action.canceled += OnThrowCanceled;
        throwGrenadeAction.action.Enable();
    }

    private void OnDisable()
    {
        // Unsubscribe to avoid memory leaks
        throwGrenadeAction.action.started -= OnThrowStarted;
        throwGrenadeAction.action.canceled -= OnThrowCanceled;
        throwGrenadeAction.action.Disable();
    }

    private void OnThrowStarted(InputAction.CallbackContext ctx)
    {
        // If no grenades left, do nothing
        if (grenadeCount <= 0) return;

        // Begin charging throw
        isCharging = true;
        currentCharge = minThrowForce;

        if (trajectoryLine != null)
        {
            trajectoryLine.enabled = true;
        }
    }

    private void OnThrowCanceled(InputAction.CallbackContext ctx)
    {
        // If we were charging, release the throw
        if (isCharging)
        {
            ThrowGrenade();
            isCharging = false;

            if (trajectoryLine != null)
            {
                trajectoryLine.enabled = false;
            }
        }
    }

    private void Update()
    {
        // If we are charging, increase the force and update the trajectory
        if (isCharging)
        {
            currentCharge += forceChargeRate * Time.deltaTime;
            currentCharge = Mathf.Clamp(currentCharge, minThrowForce, maxThrowForce);

            UpdateTrajectory();
        }
    }

    private void UpdateTrajectory()
    {
        if (trajectoryLine == null || grenadeSpawnPoint == null) return;

        Vector3[] points = new Vector3[numTrajectoryPoints];

        // Calculate the initial velocity
        Vector3 initialVelocity = (transform.forward + transform.up * upwardModifier).normalized * currentCharge;
        Vector3 startPosition = grenadeSpawnPoint.position;

        for (int i = 0; i < numTrajectoryPoints; i++)
        {
            float t = i * trajectoryTimeStep;
            // standard projectile motion: p = p0 + v0*t + 0.5*g*t^2
            points[i] = startPosition + initialVelocity * t + 0.5f * Physics.gravity * t * t;
        }

        trajectoryLine.positionCount = numTrajectoryPoints;
        trajectoryLine.SetPositions(points);
    }

    private void ThrowGrenade()
    {
        if (grenadePrefab == null || grenadeSpawnPoint == null) return;

        // Calculate final throw velocity
        Vector3 throwVelocity = (transform.forward + transform.up * upwardModifier).normalized * currentCharge;

        // Instantiate the grenade
        GameObject grenade = Instantiate(grenadePrefab, grenadeSpawnPoint.position, Quaternion.identity);
        Rigidbody rb = grenade.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.velocity = throwVelocity;
        }

        Collider playerCollider = GetComponent<Collider>(); 
        Collider grenadeCollider = grenade.GetComponent<Collider>();

        if (playerCollider && grenadeCollider)
        {
            Physics.IgnoreCollision(playerCollider, grenadeCollider);
        }


        // Decrement the grenade count
        grenadeCount--;
    }
}
