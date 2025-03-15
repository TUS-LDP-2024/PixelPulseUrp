using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;
using System.Collections;

public class PlayerShotgunShooting : MonoBehaviour
{
    [Header("Shotgun Stats")]
    public int damagePerRay = 60;
    public int raysPerShot = 13;
    public float range = 100f;
    public float fireRate = 1f;
    public int maxAmmo = 8;
    public float reloadTime = 2f;
    public int storedAmmo = 32;

    // Spread settings:
    // The ellipse will have a horizontal radius = 0.3 * Screen.width and vertical radius = 0.3 * Screen.height,
    // yielding an oval covering roughly 60% of the screen.
    private float radiusX { get { return 0.3f * Screen.width; } }
    private float radiusY { get { return 0.3f * Screen.height; } }

    [Header("Recoil Settings")]
    public float recoilForce = 1f;
    private float recoilRecoverySpeed;
    private bool isRecoiling = false;

    [Header("Shooting Effects")]
    public GameObject impactEffect;
    public AudioClip shootSound;
    public AudioClip reloadSound;

    [Header("References")]
    public Camera playerCamera; // Typically the main camera.
    public Transform gunBarrel; // The origin point for the rays (e.g. the end of the weapon model).
    public CameraShake cameraShake; // Reference to the camera shake script.
    public TextMeshProUGUI ammoDisplay; // UI element for ammo.

    // It will use the universal HitMarkerManager to display hit markers on enemy impact.
    public HitMarkerManager hitMarkerManager;

    private PlayerInput playerInput;
    private InputAction fireAction;
    private float nextFireTime = 0f;
    private int currentAmmo;
    private bool isReloading = false;

    // For recoil recovery, we store the original local position/rotation of the gun barrel.
    private Vector3 originalWeaponPosition;
    private Quaternion originalWeaponRotation;
    private AudioSource weaponAudioSource;

    private void Awake()
    {
        playerInput = GetComponent<PlayerInput>();
        fireAction = playerInput.actions["Fire"];
    }

    private void Start()
    {
        currentAmmo = maxAmmo;
        UpdateAmmoDisplay();

        if (playerCamera == null)
            playerCamera = Camera.main;

        if (gunBarrel == null)
        {
            Debug.LogError("GunBarrel not assigned in PlayerShotgunShooting!");
            return;
        }

        originalWeaponPosition = gunBarrel.localPosition;
        originalWeaponRotation = gunBarrel.localRotation;

        weaponAudioSource = gunBarrel.GetComponent<AudioSource>();
        if (weaponAudioSource == null)
            Debug.LogWarning("AudioSource not found on gunBarrel. Please assign one.");
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
        // Smooth recoil recovery.
        if (gunBarrel != null && isRecoiling)
        {
            gunBarrel.localPosition = Vector3.Lerp(gunBarrel.localPosition, originalWeaponPosition, Time.deltaTime * recoilRecoverySpeed);
            gunBarrel.localRotation = Quaternion.Lerp(gunBarrel.localRotation, originalWeaponRotation, Time.deltaTime * recoilRecoverySpeed);
            if (Vector3.Distance(gunBarrel.localPosition, originalWeaponPosition) < 0.01f &&
                Quaternion.Angle(gunBarrel.localRotation, originalWeaponRotation) < 0.01f)
            {
                isRecoiling = false;
            }
        }
    }

    private void OnShoot(InputAction.CallbackContext context)
    {
        if (isReloading || isRecoiling)
        {
            Debug.Log("Cannot shoot: either reloading or recoiling.");
            return;
        }

        if (Time.time < nextFireTime)
        {
            Debug.Log("Shot on cooldown.");
            return;
        }

        if (currentAmmo <= 0)
        {
            Reload();
            return;
        }

        nextFireTime = Time.time + 1f / fireRate;
        currentAmmo--;
        UpdateAmmoDisplay();

        // Cast multiple rays to simulate buckshot.
        for (int i = 0; i < raysPerShot; i++)
        {
            Ray ray = GenerateShotgunRay();
            if (Physics.Raycast(gunBarrel.position, ray.direction, out RaycastHit hit, range))
            {
                Debug.Log("Shotgun ray hit: " + hit.collider.name);

                // Check if the hit object is an enemy.
                EnemyHealth enemyHealth = hit.collider.GetComponentInParent<EnemyHealth>();
                if (enemyHealth != null)
                {
                    enemyHealth.TakeDamage(damagePerRay);

                    // Use the hit marker manager to display a hitmarker at the impact point.
                    if (hitMarkerManager != null)
                        hitMarkerManager.ShowHitMarker(hit.point);
                    else if (HitMarkerManager.Instance != null)
                        HitMarkerManager.Instance.ShowHitMarker(hit.point);
                }

                // Spawn an impact effect if one is assigned.
                if (impactEffect != null)
                    Instantiate(impactEffect, hit.point, Quaternion.LookRotation(hit.normal));
            }
        }

        // Apply recoil to the gun model.
        ApplyRecoil();

        // Trigger a camera shake effect.
        if (cameraShake != null)
            cameraShake.TriggerShake();

        // Play the shooting sound.
        if (weaponAudioSource != null && shootSound != null)
            weaponAudioSource.PlayOneShot(shootSound);
    }

    // Generates a ray whose direction is based on a random point within an elliptical spread.
    private Ray GenerateShotgunRay()
    {
        // Get the center of the screen.
        Vector2 screenCenter = new Vector2(Screen.width / 2f, Screen.height / 2f);

        // Use polar coordinates to sample uniformly inside an ellipse.
        float angle = Random.Range(0, 2 * Mathf.PI);
        // The radius factor is randomized so that points are uniformly distributed.
        float r = Mathf.Sqrt(Random.value);
        float xOffset = r * Mathf.Cos(angle) * radiusX;
        float yOffset = r * Mathf.Sin(angle) * radiusY;

        Vector2 randomScreenPoint = screenCenter + new Vector2(xOffset, yOffset);
        Ray cameraRay = playerCamera.ScreenPointToRay(randomScreenPoint);

        // Use the direction from the camera's ray but override the origin with the gun barrel.
        return new Ray(gunBarrel.position, cameraRay.direction);
    }

    private void ApplyRecoil()
    {
        if (gunBarrel != null)
        {
            isRecoiling = true;
            recoilRecoverySpeed = fireRate * 2f;
            // Move the barrel slightly backwards.
            gunBarrel.localPosition -= Vector3.forward * recoilForce;
            // Apply a slight upward tilt.
            gunBarrel.localRotation *= Quaternion.Euler(-recoilForce * 10f, 0f, 0f);
        }
    }

    public void Reload()
    {
        if (isReloading || currentAmmo == maxAmmo || storedAmmo == 0)
            return;

        isReloading = true;
        if (weaponAudioSource != null && reloadSound != null)
            weaponAudioSource.PlayOneShot(reloadSound);
        Invoke(nameof(FinishReload), reloadTime);
    }

    private void FinishReload()
    {
        int ammoNeeded = maxAmmo - currentAmmo;
        int ammoToReload = Mathf.Min(ammoNeeded, storedAmmo);
        currentAmmo += ammoToReload;
        storedAmmo -= ammoToReload;
        isReloading = false;
        UpdateAmmoDisplay();
    }

    private void UpdateAmmoDisplay()
    {
        if (ammoDisplay != null)
            ammoDisplay.text = $"{currentAmmo} / {storedAmmo}";
    }
}
