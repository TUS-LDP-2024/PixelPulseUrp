using UnityEngine;
using UnityEngine.InputSystem;

public class WeaponManager : MonoBehaviour
{
    [Header("Weapon Inventory")]
    public Weapon[] weapons; // Array to store all available weapons
    public Weapon[] playerInventory = new Weapon[2]; // Player's inventory (max 2 weapons)
    private int currentWeaponIndex = 0; // Index of the currently equipped weapon

    [Header("References")]
    public PlayerShooting playerShooting; // Reference to the PlayerShooting script
    public PointsManager pointsManager;   // Reference to the PointsManager script
    public Transform weaponParent;       // Parent transform for weapon models

    [Header("Current Weapon Model")]
    public GameObject currentWeaponModel; // Currently instantiated weapon model
    public Weapon currentWeapon; // Reference to the current weapon's data

    private PlayerInput playerInput;
    private InputAction reloadAction;

    // Event to notify when the weapon changes
    public System.Action OnWeaponChanged;

    private void Awake()
    {
        playerInput = GetComponent<PlayerInput>();
        reloadAction = playerInput.actions["Reload"];
    }

    private void OnEnable()
    {
        if (reloadAction != null)
        {
            reloadAction.performed += OnReload; // Subscribe to the reload action
        }
    }

    private void OnDisable()
    {
        if (reloadAction != null)
        {
            reloadAction.performed -= OnReload; // Unsubscribe from the reload action
        }
    }

    private void Start()
    {
        // Initialize the first weapon in the inventory (if any)
        if (playerInventory.Length > 0 && playerInventory[0] != null)
        {
            EquipWeapon(0);
        }
        else
        {
            Debug.LogWarning("No weapon in inventory slot 0.");
        }
    }

    // Handle reload input
    private void OnReload(InputAction.CallbackContext context)
    {
        Debug.Log("Reload input detected!");
        if (playerShooting != null)
        {
            playerShooting.Reload(); // Call the public Reload method
        }
        else
        {
            Debug.LogError("PlayerShooting reference is missing!");
        }
    }

    // Switch to the next weapon in the inventory
    private void SwitchWeapon()
    {
        // Only switch if there are 2 weapons in the inventory
        if (playerInventory[0] == null || playerInventory[1] == null)
        {
            Debug.Log("Cannot switch weapons: Inventory is not full.");
            return;
        }

        // Increment the weapon index and wrap around if necessary
        currentWeaponIndex = (currentWeaponIndex + 1) % playerInventory.Length;

        // Equip the weapon at the new index
        EquipWeapon(currentWeaponIndex);
    }

    private void EquipWeapon(int index)
    {
        if (index < 0 || index >= playerInventory.Length || playerInventory[index] == null)
        {
            Debug.LogError("Invalid weapon index or no weapon in slot!");
            return;
        }

        // Set the current weapon
        currentWeapon = playerInventory[index];
        Debug.Log($"Equipping {currentWeapon.weaponName}");

        // Update the PlayerShooting script with the new weapon's stats
        if (playerShooting != null)
        {
            playerShooting.UpdateWeaponStats(
                currentWeapon.damage,
                currentWeapon.range,
                currentWeapon.fireRate,
                currentWeapon.maxAmmo,
                currentWeapon.reloadTime,
                currentWeapon.isShotgun,
                currentWeapon.spreadAngle,
                currentWeapon.pelletCount,
                currentWeapon.recoilForce, // Pass recoil force
                currentWeapon.recoilIntensity // Pass recoil intensity
            );
        }

        // Instantiate the weapon model
        if (currentWeaponModel != null)
        {
            Destroy(currentWeaponModel); // Destroy the current weapon model
        }
        currentWeaponModel = currentWeapon.InstantiateModel(weaponParent);

        if (currentWeaponModel == null)
        {
            Debug.LogError("Failed to instantiate weapon model!");
        }
        else
        {
            Debug.Log($"Weapon model instantiated: {currentWeaponModel.name}");
        }

        // Notify subscribers that the weapon has changed
        OnWeaponChanged?.Invoke();
    }

    // Buy a weapon from the wall and add it to the inventory
    public void BuyWeapon(Weapon newWeapon, int cost)
    {
        if (pointsManager == null)
        {
            Debug.LogError("PointsManager reference is missing!");
            return;
        }

        // Check if the player has enough points
        if (pointsManager.points < cost)
        {
            Debug.Log("Not enough points to buy this weapon!");
            return;
        }

        // Deduct the cost from the player's points
        pointsManager.SpendPoints(cost);

        // Add the new weapon to the inventory
        if (playerInventory[0] == null)
        {
            // Add to the first slot if empty
            playerInventory[0] = newWeapon;
            EquipWeapon(0);
        }
        else if (playerInventory[1] == null)
        {
            // Add to the second slot if empty
            playerInventory[1] = newWeapon;
            EquipWeapon(1);
        }
        else
        {
            // Replace the currently equipped weapon
            playerInventory[currentWeaponIndex] = newWeapon;
            EquipWeapon(currentWeaponIndex);
        }

        Debug.Log($"Purchased {newWeapon.weaponName} for {cost} points");
    }
}