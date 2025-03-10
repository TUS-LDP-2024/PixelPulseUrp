using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;
using System; // Required for TextMeshPro

public class PlayerShooting : MonoBehaviour
{
    [Header("Weapon Stats")]
    public int damage = 34;       // Default damage
    public float range = 100f;    // Default range
    public float fireRate = 1f;   // Default fire rate
    public int maxAmmo = 30;      // Maximum ammo capacity
    public float reloadTime = 2f; // Time it takes to reload
    public int maxStoredAmmo = 100; // Max stored ammo
    public int storedAmmo = 100; // Stored ammo count

    [Header("Recoil Settings")]
    public float recoilForce = 1f; // Recoil force applied to the weapon
    public float recoilRecoverySpeed = 5f; // Speed at which the weapon returns to its original position

    [Header("Camera Shake")]
    public CameraShake cameraShake; // Reference to the CameraShake script on the Main Camera

    private int currentAmmo;      // Current ammo count
    private bool isReloading = false; // Flag to track reloading state

    [Header("Shooting Effects")]
    public GameObject impactEffect;

    [Header("References")]
    public PointsManager pointsManager; // Reference to the PointsManager script
    public WeaponManager weaponManager; // Reference to the WeaponManager script
    public TextMeshProUGUI ammoDisplay; // Reference to the ammo display text

    private Transform gunBarrel;   // Transform representing the gun barrel
    private PlayerInput playerInput;
    private InputAction fireAction;
    private float nextFireTime = 0f;

    private Vector3 originalWeaponPosition; // Original position of the weapon
    private Quaternion originalWeaponRotation; // Original rotation of the weapon

    private AudioSource weaponAudioSource; // AudioSource for weapon sounds

    private void Awake()
    {
        playerInput = GetComponent<PlayerInput>();
        fireAction = playerInput.actions["Fire"];
    }

    private void Start()
    {
        // Initialize ammo
        currentAmmo = maxAmmo;
        UpdateAmmoDisplay(); // Update the ammo display at the start

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

        // Store the original position and rotation of the weapon
        if (weaponManager != null && weaponManager.currentWeaponModel != null)
        {
            originalWeaponPosition = weaponManager.currentWeaponModel.transform.localPosition;
            originalWeaponRotation = weaponManager.currentWeaponModel.transform.localRotation;

            // Get the AudioSource component from the weapon model
            weaponAudioSource = weaponManager.currentWeaponModel.GetComponent<AudioSource>();
            if (weaponAudioSource == null)
            {
                Debug.LogError("AudioSource component not found on the weapon model!");
            }
        }
    }

    private void OnEnable()
    {
        fireAction.performed += OnShoot;
    }

    private void OnDisable()
    {
        fireAction.performed -= OnShoot;
    }

    private void Update()
    {
        // Recoil recovery
        if (weaponManager != null && weaponManager.currentWeaponModel != null)
        {
            weaponManager.currentWeaponModel.transform.localPosition = Vector3.Lerp(
                weaponManager.currentWeaponModel.transform.localPosition,
                originalWeaponPosition,
                Time.deltaTime * recoilRecoverySpeed
            );

            weaponManager.currentWeaponModel.transform.localRotation = Quaternion.Lerp(
                weaponManager.currentWeaponModel.transform.localRotation,
                originalWeaponRotation,
                Time.deltaTime * recoilRecoverySpeed
            );
        }
    }

    private void OnShoot(InputAction.CallbackContext context)
    {
        if (isReloading) return; // Don't shoot while reloading

        // Check if enough time has passed since the last shot
        if (Time.time < nextFireTime) return;

        // Check if there is ammo left
        if (currentAmmo <= 0)
        {
            Reload();
            return;
        }

        // Set the next fire time based on the fire rate
        nextFireTime = Time.time + 1f / fireRate;

        // Perform the raycast
        PerformRaycast();

        // Apply recoil
        ApplyRecoil();

        // Trigger camera shake
        if (cameraShake != null)
        {
            Debug.Log("Triggering camera shake from PlayerShooting.");
            cameraShake.TriggerShake();
        }
        else
        {
            Debug.LogError("CameraShake reference is null in PlayerShooting!");
        }

        // Play shooting sound
        if (weaponAudioSource != null && weaponManager.currentWeapon != null)
        {
            weaponAudioSource.PlayOneShot(weaponManager.currentWeapon.shootSound);
        }
        else
        {
            Debug.LogError("Weapon AudioSource or shootSound is missing!");
        }

        // Consume ammo
        currentAmmo--;
        UpdateAmmoDisplay(); // Update the ammo display after shooting
    }

    private void ApplyRecoil()
    {
        if (weaponManager != null && weaponManager.currentWeaponModel != null)
        {
            // Apply recoil to the weapon
            weaponManager.currentWeaponModel.transform.localPosition -= Vector3.forward * recoilForce;
            weaponManager.currentWeaponModel.transform.localRotation = Quaternion.Euler(
                weaponManager.currentWeaponModel.transform.localRotation.eulerAngles + new Vector3(-recoilForce * 10, 0, 0)
            );
        }
    }

    private void PerformRaycast()
    {
        if (gunBarrel == null)
        {
            Debug.LogError("GunBarrel is not assigned!");
            return;
        }

        // Perform a raycast that gets all hits along the bullet's path.
        // Using QueryTriggerInteraction.Collide so triggers are included.
        RaycastHit[] hits = Physics.RaycastAll(gunBarrel.position, gunBarrel.forward, range, ~0, QueryTriggerInteraction.Collide);

        // Sort the hits by distance (closest first)
        System.Array.Sort(hits, (h1, h2) => h1.distance.CompareTo(h2.distance));

        bool validHitFound = false;
        foreach (var hit in hits)
        {
            // Check if this hit should be ignored:
            // - It's a BoxCollider
            // - It's a trigger
            // - And its GameObject is either on the "GroundLayer" or tagged "Floor"
            if (hit.collider is BoxCollider && hit.collider.isTrigger)
            {
                if (hit.collider.CompareTag("Floor") || hit.collider.gameObject.layer == LayerMask.NameToLayer("GroundLayer"))
                {
                    Debug.Log("Ignored BoxCollider trigger: " + hit.collider.name);
                    continue; // Skip this hit and look for the next one
                }
            }

            // Use this hit since it doesn't match the ignore criteria.
            HandleHit(hit);
            validHitFound = true;
            break;
        }

        if (!validHitFound)
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
    public void UpdateWeaponStats(int newDamage, float newRange, float newFireRate, int newMaxAmmo, float newReloadTime)
    {
        damage = newDamage;
        range = newRange;
        fireRate = newFireRate;
        maxAmmo = newMaxAmmo;
        reloadTime = newReloadTime;

        // Reset ammo when switching weapons
        currentAmmo = maxAmmo;
        UpdateAmmoDisplay(); // Update the ammo display when switching weapons
    }

    // Reload the weapon
    public void Reload()
    {
        if (isReloading || currentAmmo == maxAmmo || storedAmmo == 0) return;

        Debug.Log("Reloading...");
        isReloading = true;

        // Play reloading sound
        if (weaponAudioSource != null && weaponManager.currentWeapon != null)
        {
            weaponAudioSource.PlayOneShot(weaponManager.currentWeapon.reloadSound);
        }
        else
        {
            Debug.LogError("Weapon AudioSource or reloadSound is missing!");
        }

        // Wait for the reload time
        Invoke(nameof(FinishReload), reloadTime);
    }

    private void FinishReload()
    {
        storedAmmo += currentAmmo;
        int ammoToReload = Math.Min(storedAmmo, maxAmmo);
        storedAmmo -= ammoToReload;
        currentAmmo = ammoToReload;
        isReloading = false;
        UpdateAmmoDisplay(); // Update the ammo display after reloading
        Debug.Log("Reload complete!");
    }

    // Update the ammo display text
    public void UpdateAmmoDisplay()
    {
        if (ammoDisplay != null)
        {
            ammoDisplay.text = $"{currentAmmo} / {storedAmmo}";
        }
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

        // Update the AudioSource reference when switching weapons
        weaponAudioSource = weaponManager.currentWeaponModel.GetComponent<AudioSource>();
        if (weaponAudioSource == null)
        {
            Debug.LogError("AudioSource component not found on the weapon model!");
        }
    }
}