using UnityEngine;
using UnityEngine.InputSystem;

public class WeaponManager : MonoBehaviour
{
    [Header("Weapon Inventory")]
    public Weapon[] weapons;
    public Weapon[] playerInventory = new Weapon[2];
    private int _currentWeaponIndex = 0;
    public int currentWeaponIndex
    {
        get => _currentWeaponIndex;
        private set => _currentWeaponIndex = value;
    }

    [Header("References")]
    public PlayerShooting playerShooting;
    public PointsManager pointsManager;
    public Transform weaponParent;

    [Header("Current Weapon Model")]
    public GameObject currentWeaponModel;
    public Weapon currentWeapon { get; private set; }

    private PlayerInput playerInput;
    private InputAction reloadAction;
    private InputAction switchWeaponAction;

    public System.Action OnWeaponChanged;

    private void Awake()
    {
        playerInput = GetComponent<PlayerInput>();
        reloadAction = playerInput.actions["Reload"];
        switchWeaponAction = playerInput.actions["SwitchWeapon"];
    }

    private void OnEnable()
    {
        if (reloadAction != null)
        {
            reloadAction.performed += OnReload;
        }
        if (switchWeaponAction != null)
        {
            switchWeaponAction.performed += OnSwitchWeapon;
        }
    }

    private void OnDisable()
    {
        if (reloadAction != null)
        {
            reloadAction.performed -= OnReload;
        }
        if (switchWeaponAction != null)
        {
            switchWeaponAction.performed -= OnSwitchWeapon;
        }
    }

    private void Start()
    {
        if (playerInventory.Length > 0 && playerInventory[0] != null)
        {
            EquipWeapon(0);
        }
    }

    private void OnSwitchWeapon(InputAction.CallbackContext context)
    {
        SwitchWeapon();
    }

    private void OnReload(InputAction.CallbackContext context)
    {
        if (playerShooting != null && context.performed)
        {
            playerShooting.Reload();
        }
    }

    private void SwitchWeapon()
    {
        if (playerInventory[0] == null || playerInventory[1] == null)
        {
            return;
        }

        currentWeaponIndex = (currentWeaponIndex + 1) % playerInventory.Length;
        EquipWeapon(currentWeaponIndex);
    }

    public void EquipWeapon(int index)
    {
        if (index < 0 || index >= playerInventory.Length || playerInventory[index] == null)
        {
            return;
        }

        currentWeapon = playerInventory[index];

        if (playerShooting != null)
        {
            playerShooting.UpdateWeaponStats(
                currentWeapon.damage,
                currentWeapon.range,
                currentWeapon.fireRate,
                currentWeapon.maxAmmo,
                currentWeapon.isShotgun ? currentWeapon.shellReloadInterval : currentWeapon.reloadTime,
                currentWeapon.isShotgun,
                currentWeapon.spreadAngle,
                currentWeapon.pelletCount,
                currentWeapon.recoilForce,
                currentWeapon.recoilIntensity
            );
            playerShooting.ResetAmmo();
        }

        if (currentWeaponModel != null)
        {
            Destroy(currentWeaponModel);
        }

        currentWeaponModel = currentWeapon.InstantiateModel(weaponParent);

        // Reset weapon position/rotation if needed
        if (playerShooting != null)
        {
            playerShooting.CancelReload();
        }

        OnWeaponChanged?.Invoke();
    }

    public void ApplyWeaponUpgrade(float damageBoost, float fireRateBoost, float ammoBoost, float reloadSpeedBoost)
    {
        if (currentWeapon == null) return;

        if (damageBoost > 0)
        {
            currentWeapon.damage = Mathf.RoundToInt(currentWeapon.damage * (1 + damageBoost));
        }

        if (fireRateBoost > 0)
        {
            currentWeapon.fireRate *= (1 + fireRateBoost);
        }

        if (ammoBoost > 0)
        {
            currentWeapon.maxAmmo = Mathf.RoundToInt(currentWeapon.maxAmmo * (1 + ammoBoost));
        }

        if (reloadSpeedBoost > 0)
        {
            currentWeapon.reloadTime *= (1 - reloadSpeedBoost);
        }

        // Refresh weapon with new stats
        EquipWeapon(currentWeaponIndex);
    }

    public void BuyWeapon(Weapon newWeapon, int cost)
    {
        if (pointsManager == null)
        {
            Debug.LogWarning("PointsManager reference not set in WeaponManager");
            return;
        }

        if (pointsManager.points < cost)
        {
            Debug.Log("Not enough points to buy weapon");
            return;
        }

        pointsManager.SpendPoints(cost);

        if (playerInventory[0] == null)
        {
            playerInventory[0] = newWeapon;
            EquipWeapon(0);
        }
        else if (playerInventory[1] == null)
        {
            playerInventory[1] = newWeapon;
            EquipWeapon(1);
        }
        else
        {
            // Replace current weapon
            playerInventory[currentWeaponIndex] = newWeapon;
            EquipWeapon(currentWeaponIndex);
        }
    }

    public void ForceWeaponReset()
    {
        if (playerShooting != null)
        {
            playerShooting.CancelReload();
        }
        if (currentWeapon != null)
        {
            EquipWeapon(currentWeaponIndex);
        }
    }
}