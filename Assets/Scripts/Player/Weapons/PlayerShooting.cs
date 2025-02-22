using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerShooting : MonoBehaviour
{
    [Header("Weapon Stats")]
    public int damage = 34;       // Default damage
    public float range = 100f;    // Default range
    public float fireRate = 1f;   // Default fire rate

    [Header("Shooting Effects")]
    public GameObject impactEffect;

    [Header("References")]
    public PointsManager pointsManager; // Reference to the PointsManager script
    public WeaponManager weaponManager; // Reference to the WeaponManager script

    private Transform gunBarrel;   // Transform representing the gun barrel
    private PlayerInput playerInput;
    private InputAction fireAction;
    private float nextFireTime = 0f;

    private void Awake()
    {
        playerInput = GetComponent<PlayerInput>();
        fireAction = playerInput.actions["Fire"];
    }

    private void Start()
    {
        // Subscribe to the WeaponManager's weapon change event
        if (weaponManager != null)
        {
            weaponManager.OnWeaponChanged += UpdateGunBarrel;
        }
        else
        {
            Debug.LogError("WeaponManager reference is missing!");
        }

        // Initialize the gun barrel for the starting weapon
        UpdateGunBarrel();
    }

    private void OnEnable()
    {
        fireAction.performed += OnShoot;
    }

    private void OnDisable()
    {
        fireAction.performed -= OnShoot;
    }

    private void OnShoot(InputAction.CallbackContext context)
    {
        // Check if enough time has passed since the last shot
        if (Time.time < nextFireTime) return;

        // Set the next fire time based on the fire rate
        nextFireTime = Time.time + 1f / fireRate;

        // Perform the raycast
        PerformRaycast();
    }

    private void PerformRaycast()
    {
        if (gunBarrel == null)
        {
            Debug.LogError("GunBarrel is not assigned!");
            return;
        }

        RaycastHit hit;
        Debug.DrawRay(gunBarrel.position, gunBarrel.forward * range, Color.blue, 1f);

        if (Physics.Raycast(gunBarrel.position, gunBarrel.forward, out hit, range))
        {
            HandleHit(hit);
        }
        else
        {
            Debug.DrawRay(gunBarrel.position, gunBarrel.forward * range, Color.green, 1f);
        }
    }

    private void HandleHit(RaycastHit hit)
    {
        Debug.Log("Hit: " + hit.collider.name);

        // Check if the hit object is a zombie
        EnemyHealth enemyHealth = hit.collider.GetComponentInParent<EnemyHealth>();
        if (enemyHealth != null)
        {
            // Determine if it's a headshot or body shot
            bool isHeadshot = hit.collider.CompareTag("Head");
            int damageToApply = isHeadshot ? damage * 2 : damage;

            // Apply damage to the zombie
            enemyHealth.TakeDamage(damageToApply);

            // Award points based on the shot type
            if (pointsManager != null)
            {
                int pointsToAdd = isHeadshot ? 50 : 10;
                pointsManager.AddPoints(pointsToAdd);
            }

            Debug.Log(isHeadshot ? "Headshot!" : "Body shot!");
        }

        // Instantiate impact effect at the hit location
        if (impactEffect != null)
        {
            Instantiate(impactEffect, hit.point, Quaternion.LookRotation(hit.normal));
        }
    }

    // Update the weapon stats when a new weapon is equipped
    public void UpdateWeaponStats(int newDamage, float newRange, float newFireRate)
    {
        damage = newDamage;
        range = newRange;
        fireRate = newFireRate;
    }

    // Update the gun barrel transform when the weapon changes
    private void UpdateGunBarrel()
    {
        if (weaponManager == null || weaponManager.currentWeaponModel == null)
        {
            Debug.LogError("WeaponManager or currentWeaponModel is missing!");
            return;
        }

        // Find the gun barrel transform in the current weapon model
        gunBarrel = weaponManager.currentWeaponModel.transform.Find("GunBarrel");
        if (gunBarrel == null)
        {
            Debug.LogError("GunBarrel not found in the current weapon model!");
        }
        else
        {
            Debug.Log("GunBarrel updated: " + gunBarrel.name);
        }
    }
}