using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;

public class WeaponPurchase : MonoBehaviour
{
    [Header("Weapon Settings")]
    public Weapon weapon; // The weapon to purchase
    public int cost = 100; // Cost of the weapon

    [Header("References")]
    public WeaponManager weaponManager; // Reference to the WeaponManager script

    [Header("Cooldown Settings")]
    public float cooldownTime = 1f; // Time (in seconds) before another purchase can be made

    [Header("UI Settings")]
    public GameObject purchaseMessage; // UI element to display purchase messages

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
            // Check if the player already has the weapon
            if (PlayerHasWeapon(weapon))
            {
                ShowMessage("You already have this weapon!");
            }
            else
            {
                // Attempt to buy the weapon
                weaponManager.BuyWeapon(weapon, cost);
            }

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

    private bool PlayerHasWeapon(Weapon weaponToCheck)
    {
        // Check if the weapon is already in the player's inventory
        foreach (Weapon weapon in weaponManager.playerInventory)
        {
            if (weapon != null && weapon.weaponName == weaponToCheck.weaponName)
            {
                return true; // Player already has this weapon
            }
        }
        return false; // Player does not have this weapon
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

    private void ShowMessage(string message)
    {
        if (purchaseMessage != null)
        {
            // Display the message (e.g., using a UI text element)
            purchaseMessage.SetActive(true);
            purchaseMessage.GetComponent<TextMeshProUGUI>().text = message;

            // Hide the message after a short delay
            Invoke(nameof(HideMessage), 2f);
        }
    }

    private void HideMessage()
    {
        if (purchaseMessage != null)
        {
            purchaseMessage.SetActive(false);
        }
    }
}