using UnityEngine;
using UnityEngine.InputSystem;

public class WeaponManager : MonoBehaviour
{
    [Header("Weapon Inventory")]
    public Weapon[] weapons;
    public Weapon[] playerInventory = new Weapon[2];
    private int currentWeaponIndex = 0;

    [Header("References")]
    public PlayerShooting playerShooting;
    public PointsManager pointsManager;
    public Transform weaponParent;

    [Header("Current Weapon Model")]
    public GameObject currentWeaponModel;
    public Weapon currentWeapon;

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
        if (playerShooting != null)
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
    public void ApplyWeaponUpgrade(float damageBoost, float fireRateBoost, float ammoBoost, float reloadSpeedBoost)
    {
        if (currentWeapon == null) return;

        if (damageBoost > 0)
            currentWeapon.damage = Mathf.RoundToInt(currentWeapon.damage * (1 + damageBoost));

        if (fireRateBoost > 0)
            currentWeapon.fireRate *= (1 + fireRateBoost);

        if (ammoBoost > 0)
            currentWeapon.maxAmmo = Mathf.RoundToInt(currentWeapon.maxAmmo * (1 + ammoBoost));

        if (reloadSpeedBoost > 0)
            currentWeapon.reloadTime *= (1 - reloadSpeedBoost);

        EquipWeapon(currentWeaponIndex);
    }

    private void EquipWeapon(int index)
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
                currentWeapon.reloadTime,
                currentWeapon.isShotgun,
                currentWeapon.spreadAngle,
                currentWeapon.pelletCount,
                currentWeapon.recoilForce,
                currentWeapon.recoilIntensity
            );
        }

        if (currentWeaponModel != null)
        {
            Destroy(currentWeaponModel);
        }
        currentWeaponModel = currentWeapon.InstantiateModel(weaponParent);

        OnWeaponChanged?.Invoke();
    }

    public void BuyWeapon(Weapon newWeapon, int cost)
    {
        if (pointsManager == null)
        {
            return;
        }

        if (pointsManager.points < cost)
        {
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
            playerInventory[currentWeaponIndex] = newWeapon;
            EquipWeapon(currentWeaponIndex);
        }
    }
}