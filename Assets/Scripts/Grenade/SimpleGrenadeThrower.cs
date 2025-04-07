using UnityEngine;
using UnityEngine.InputSystem; // Required for the new Input System

public class SimpleGrenadeThrower : MonoBehaviour
{
    [Header("Grenade Settings")]
    public GameObject grenadePrefab;        // Grenade prefab reference
    public Transform grenadeSpawnPoint;     // Spawn point for grenade
    public int grenadeCount = 5;            // Starting grenade count

    [Header("Throw Settings")]
    public float throwForce = 15f;          // Fixed throw force
    public float upwardModifier = 0.5f;     // Adds arc to the throw

    [Header("Input")]
    public InputActionReference throwGrenadeAction; // Input system reference

    private void OnEnable()
    {
        throwGrenadeAction.action.started += OnThrowGrenade;
        throwGrenadeAction.action.Enable();
    }

    private void OnDisable()
    {
        throwGrenadeAction.action.started -= OnThrowGrenade;
        throwGrenadeAction.action.Disable();
    }

    private void OnThrowGrenade(InputAction.CallbackContext ctx)
    {
        if (grenadeCount <= 0 || grenadePrefab == null || grenadeSpawnPoint == null) return;

        // Spawn and apply force
        GameObject grenade = Instantiate(grenadePrefab, grenadeSpawnPoint.position, Quaternion.identity);
        Rigidbody rb = grenade.GetComponent<Rigidbody>();

        if (rb != null)
        {
            Vector3 throwDirection = (transform.forward + transform.up * upwardModifier).normalized;
            rb.velocity = throwDirection * throwForce;
        }

        // Ignore collision between player and grenade
        Collider playerCollider = GetComponent<Collider>();
        Collider grenadeCollider = grenade.GetComponent<Collider>();
        if (playerCollider && grenadeCollider)
        {
            Physics.IgnoreCollision(playerCollider, grenadeCollider);
        }

        grenadeCount--;
    }
}
