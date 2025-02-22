using UnityEngine;
using UnityEngine.InputSystem;

public class WeaponPurchase : MonoBehaviour
{
    [Header("Weapon Settings")]
    public Weapon weapon; // The weapon to purchase
    public int cost = 100; // Cost of the weapon

    [Header("References")]
    public WeaponManager weaponManager; // Reference to the WeaponManager script

    [Header("Cooldown Settings")]
    public float cooldownTime = 1f; // Time (in seconds) before another purchase can be made

    private PlayerInput playerInput;
    private InputAction interactAction;
    private bool isOnCooldown = false; // Tracks whether the purchase is on cooldown

    private void Awake()
    {
        playerInput = GameObject.FindWithTag("Player").GetComponent<PlayerInput>();
        interactAction = playerInput.actions["Interact"];
    }

    private void OnEnable()
    {
        interactAction.performed += OnInteract;
    }

    private void OnDisable()
    {
        interactAction.performed -= OnInteract;
    }

    private void OnInteract(InputAction.CallbackContext context)
    {
        if (!isOnCooldown && IsPlayerInRange())
        {
            // Attempt to buy the weapon
            weaponManager.BuyWeapon(weapon, cost);

            // Start cooldown
            StartCooldown();
        }
    }

    private bool IsPlayerInRange()
    {
        // Check if the player is within range (e.g., using a trigger collider)
        Collider[] colliders = Physics.OverlapSphere(transform.position, 2f);
        foreach (Collider collider in colliders)
        {
            if (collider.CompareTag("Player"))
            {
                return true;
            }
        }
        return false;
    }

    private void StartCooldown()
    {
        // Set cooldown flag and start the cooldown timer
        isOnCooldown = true;
        Invoke(nameof(ResetCooldown), cooldownTime);
    }

    private void ResetCooldown()
    {
        // Reset the cooldown flag
        isOnCooldown = false;
    }
}