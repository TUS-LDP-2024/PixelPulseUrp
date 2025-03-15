using UnityEngine;
using UnityEngine.InputSystem;
using System;
using TMPro;

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

    [Header("Shotgun Settings")]
    public bool isShotgun = false; // Is the current weapon a shotgun?
    public float spreadAngle = 10f; // Spread angle for shotgun pellets (in degrees)
    public int pelletCount = 8; // Number of pellets fired per shot

    [Header("Recoil Settings")]
    public float recoilForce = 1f; // Recoil force applied to the weapon
    private float recoilRecoverySpeed; // Speed at which the weapon returns to its original position

    [Header("Camera Shake")]
    public CameraShake cameraShake; // Reference to the CameraShake script on the Main Camera

    [Header("Blend Shape Settings")]
    public SkinnedMeshRenderer pistolMeshRenderer; // Reference to the pistol's SkinnedMeshRenderer
    public int magEjectBlendShapeIndex = 0; // Index of the MagEject blend shape
    public int roundCycleBlendShapeIndex = 1; // Index of the RoundCycle blend shape
    public float blendShapeSpeed = 5f; // Speed of blend shape animation

    private int currentAmmo;      // Current ammo count
    private bool isReloading = false; // Flag to track reloading state
    private bool isRecoiling = false; // Flag to track recoil state

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

    private Vector3 originalWeaponPosition; // Original local position of the weapon
    private Quaternion originalWeaponRotation; // Original local rotation of the weapon

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

        // Store the original local position and rotation of the weapon
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
        if (weaponManager != null && weaponManager.currentWeaponModel != null && isRecoiling)
        {
            // Smoothly reset the weapon's local position
            weaponManager.currentWeaponModel.transform.localPosition = Vector3.Lerp(
                weaponManager.currentWeaponModel.transform.localPosition,
                originalWeaponPosition,
                Time.deltaTime * recoilRecoverySpeed
            );

            // Smoothly reset the weapon's local rotation
            weaponManager.currentWeaponModel.transform.localRotation = Quaternion.Lerp(
                weaponManager.currentWeaponModel.transform.localRotation,
                originalWeaponRotation,
                Time.deltaTime * recoilRecoverySpeed
            );

            // Check if the weapon has returned to its original position and rotation
            if (Vector3.Distance(weaponManager.currentWeaponModel.transform.localPosition, originalWeaponPosition) < 0.01f &&
                Quaternion.Angle(weaponManager.currentWeaponModel.transform.localRotation, originalWeaponRotation) < 0.01f)
            {
                // Reset the recoil state
                isRecoiling = false;
                Debug.Log("Recoil reset complete. isRecoiling = " + isRecoiling);
            }
        }
    }

    private void OnShoot(InputAction.CallbackContext context)
    {
        if (isReloading || isRecoiling)
        {
            Debug.Log("Cannot shoot: isReloading = " + isReloading + ", isRecoiling = " + isRecoiling);
            return; // Don't shoot while reloading or recoiling
        }

        // Check if enough time has passed since the last shot
        if (Time.time < nextFireTime)
        {
            Debug.Log("Cannot shoot: Fire rate cooldown.");
            return;
        }

        // Check if there is ammo left
        if (currentAmmo <= 0)
        {
            Debug.Log("Cannot shoot: Out of ammo.");
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
        Debug.Log("Shot fired. Current ammo: " + currentAmmo);
    }

    private void ApplyRecoil()
    {
        if (weaponManager != null && weaponManager.currentWeaponModel != null && !isRecoiling)
        {
            // Set the recoil state
            isRecoiling = true;

            // Calculate recoil recovery speed based on fire rate
            recoilRecoverySpeed = fireRate * 2f; // Adjust the multiplier as needed

            // Apply recoil to the weapon in local space
            // Move the weapon backward along its local Z-axis
            weaponManager.currentWeaponModel.transform.localPosition -= Vector3.forward * recoilForce;

            // Apply rotation recoil (tilt the weapon upward)
            weaponManager.currentWeaponModel.transform.localRotation *= Quaternion.Euler(-recoilForce * 10, 0, 0);
        }
    }

    private void PerformRaycast()
    {
        if (gunBarrel == null)
        {
            Debug.LogError("GunBarrel is not assigned!");
            return;
        }

        if (weaponManager == null || weaponManager.currentWeapon == null)
        {
            Debug.LogError("WeaponManager or currentWeapon is missing!");
            return;
        }

        // Check if the current weapon is a shotgun
        if (weaponManager.currentWeapon.isShotgun)
        {
            // Shotgun behavior: Fire multiple pellets in a cone
            for (int i = 0; i < weaponManager.currentWeapon.pelletCount; i++)
            {
                // Calculate a random direction within the spread angle
                Vector3 pelletDirection = GetRandomDirectionWithinSpread(gunBarrel.forward, weaponManager.currentWeapon.spreadAngle);

                // Perform a raycast for each pellet
                RaycastHit[] hits = Physics.RaycastAll(gunBarrel.position, pelletDirection, range, ~0, QueryTriggerInteraction.Collide);
                System.Array.Sort(hits, (h1, h2) => h1.distance.CompareTo(h2.distance));

                bool validHitFound = false;
                foreach (var hit in hits)
                {
                    if (hit.collider is BoxCollider && hit.collider.isTrigger)
                    {
                        if (hit.collider.CompareTag("Floor") || hit.collider.gameObject.layer == LayerMask.NameToLayer("GroundLayer"))
                        {
                            Debug.Log("Ignored BoxCollider trigger: " + hit.collider.name);
                            continue;
                        }
                    }

                    HandleHit(hit);
                    validHitFound = true;
                    break;
                }

                if (!validHitFound)
                {
                    Debug.DrawRay(gunBarrel.position, pelletDirection * range, Color.green, 1f);
                }
            }
        }
        else
        {
            // Default behavior: Single raycast
            RaycastHit[] hits = Physics.RaycastAll(gunBarrel.position, gunBarrel.forward, range, ~0, QueryTriggerInteraction.Collide);
            System.Array.Sort(hits, (h1, h2) => h1.distance.CompareTo(h2.distance));

            bool validHitFound = false;
            foreach (var hit in hits)
            {
                if (hit.collider is BoxCollider && hit.collider.isTrigger)
                {
                    if (hit.collider.CompareTag("Floor") || hit.collider.gameObject.layer == LayerMask.NameToLayer("GroundLayer"))
                    {
                        Debug.Log("Ignored BoxCollider trigger: " + hit.collider.name);
                        continue;
                    }
                }

                HandleHit(hit);
                validHitFound = true;
                break;
            }

            if (!validHitFound)
            {
                Debug.DrawRay(gunBarrel.position, gunBarrel.forward * range, Color.green, 1f);
            }
        }
    }

    // Helper method to calculate a random direction within the spread angle
    private Vector3 GetRandomDirectionWithinSpread(Vector3 direction, float spreadAngle)
    {
        // Calculate random angles within the spread
        float randomAngleX = UnityEngine.Random.Range(-spreadAngle, spreadAngle); // Explicitly use UnityEngine.Random
        float randomAngleY = UnityEngine.Random.Range(-spreadAngle, spreadAngle); // Explicitly use UnityEngine.Random

        // Create a rotation based on the random angles
        Quaternion spreadRotation = Quaternion.Euler(randomAngleX, randomAngleY, 0);

        // Apply the rotation to the original direction
        return spreadRotation * direction;
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

    public void UpdateWeaponStats(int newDamage, float newRange, float newFireRate, int newMaxAmmo, float newReloadTime, bool isShotgun, float spreadAngle, int pelletCount)
    {
        damage = newDamage;
        range = newRange;
        fireRate = newFireRate;
        maxAmmo = newMaxAmmo;
        reloadTime = newReloadTime;

        // Shotgun-specific stats
        this.isShotgun = isShotgun;
        this.spreadAngle = spreadAngle;
        this.pelletCount = pelletCount;

        // Reset ammo when switching weapons
        currentAmmo = maxAmmo;
        UpdateAmmoDisplay(); // Update the ammo display when switching weapons
    }

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

        // Play the reload animation
        if (weaponManager.currentWeapon != null && weaponManager.currentWeaponModel != null)
        {
            weaponManager.currentWeapon.PlayReloadAnimation(weaponManager.currentWeaponModel, this); // Pass the weapon instance and 'this' to start the coroutine
        }
        else
        {
            Debug.LogError("Current weapon or weapon model is null!");
        }

        // Wait for the reload time
        Invoke(nameof(FinishReload), reloadTime);
    }

    private void FinishReload()
    {
        storedAmmo += currentAmmo;
        int ammoToReload = Math.Min(storedAmmo, maxAmmo); // Now works because of the 'using System;' directive
        storedAmmo -= ammoToReload;
        currentAmmo = ammoToReload;
        isReloading = false;
        UpdateAmmoDisplay(); // Update the ammo display after reloading
        Debug.Log("Reload complete!");
    }

    public void UpdateAmmoDisplay()
    {
        if (ammoDisplay != null)
        {
            ammoDisplay.text = $"{currentAmmo} / {storedAmmo}";
        }
    }

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

        // Store the original local position and rotation of the weapon
        originalWeaponPosition = weaponManager.currentWeaponModel.transform.localPosition;
        originalWeaponRotation = weaponManager.currentWeaponModel.transform.localRotation;
        Debug.Log("Original weapon position and rotation updated.");
    }
}