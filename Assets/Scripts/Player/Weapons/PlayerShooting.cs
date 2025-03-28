using UnityEngine;
using UnityEngine.InputSystem;
using System;
using TMPro;
using System.Collections;

public class PlayerShooting : MonoBehaviour
{
    [Header("Weapon Stats")]
    public int damage = 34;
    public float range = 100f;
    public float fireRate = 1f;
    public int maxAmmo = 30;
    public float reloadTime = 2f;
    public int maxStoredAmmo = 100;
    public int storedAmmo = 100;

    [Header("Shotgun Settings")]
    public bool isShotgun = false;
    public float spreadAngle = 10f;
    public int pelletCount = 8;

    [Header("Recoil Settings")]
    public float recoilForce = 1f;
    public float recoilIntensity = 1f;
    private float recoilRecoverySpeed;

    [Header("Camera Shake")]
    public CameraShake cameraShake;

    [Header("Blend Shape Settings")]
    public SkinnedMeshRenderer pistolMeshRenderer;
    public int magEjectBlendShapeIndex = 0;
    public int roundCycleBlendShapeIndex = 1;
    public float blendShapeSpeed = 5f;

    private int _currentAmmo;
    public int currentAmmo
    {
        get => _currentAmmo;
        set
        {
            _currentAmmo = Mathf.Clamp(value, 0, maxAmmo);
            UpdateAmmoDisplay();
        }
    }
    private bool isReloading = false;
    private bool isRecoiling = false;

    [Header("Shooting Effects")]
    public GameObject impactEffect;

    [Header("References")]
    public PointsManager pointsManager;
    public WeaponManager weaponManager;
    public TextMeshProUGUI ammoDisplay;

    private Transform gunBarrel;
    private PlayerInput playerInput;
    private InputAction fireAction;
    private float nextFireTime = 0f;

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

        if (weaponManager != null)
        {
            weaponManager.OnWeaponChanged += UpdateGunBarrel;
        }

        UpdateGunBarrel();

        if (weaponManager != null && weaponManager.currentWeaponModel != null)
        {
            originalWeaponPosition = weaponManager.currentWeaponModel.transform.localPosition;
            originalWeaponRotation = weaponManager.currentWeaponModel.transform.localRotation;
            weaponAudioSource = weaponManager.currentWeaponModel.GetComponent<AudioSource>();
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
        if (weaponManager != null && weaponManager.currentWeaponModel != null && isRecoiling)
        {
            weaponManager.currentWeaponModel.transform.localRotation = Quaternion.Lerp(
                weaponManager.currentWeaponModel.transform.localRotation,
                originalWeaponRotation,
                Time.deltaTime * recoilRecoverySpeed
            );

            if (Quaternion.Angle(weaponManager.currentWeaponModel.transform.localRotation, originalWeaponRotation) < 0.01f)
            {
                isRecoiling = false;
            }
        }
    }

    private void OnShoot(InputAction.CallbackContext context)
    {
        if (isReloading || isRecoiling)
        {
            return;
        }

        if (Time.time < nextFireTime)
        {
            return;
        }

        if (currentAmmo <= 0)
        {
            Reload();
            return;
        }

        nextFireTime = Time.time + 1f / fireRate;
        PerformRaycast();
        ApplyRecoil();

        if (cameraShake != null)
        {
            cameraShake.TriggerShake();
        }

        if (weaponAudioSource != null && weaponManager.currentWeapon != null)
        {
            weaponAudioSource.PlayOneShot(weaponManager.currentWeapon.shootSound);
        }

        currentAmmo--;
    }

    public void AddAmmo(int amount)
    {
        storedAmmo = Mathf.Min(maxStoredAmmo, storedAmmo + amount);
        UpdateAmmoDisplay();
    }

    private void ApplyRecoil()
    {
        if (weaponManager != null && weaponManager.currentWeaponModel != null && !isRecoiling)
        {
            isRecoiling = true;
            recoilRecoverySpeed = fireRate * 2f;
            float calculatedRecoil = recoilForce * recoilIntensity * (1f / fireRate);

            if (isShotgun)
            {
                weaponManager.currentWeaponModel.transform.localRotation *= Quaternion.Euler(0, -calculatedRecoil * 15, 0);
            }
            else
            {
                weaponManager.currentWeaponModel.transform.localRotation *= Quaternion.Euler(0, 0, calculatedRecoil * 15);
            }
        }
    }

    private void PerformRaycast()
    {
        if (gunBarrel == null || weaponManager == null || weaponManager.currentWeapon == null)
        {
            return;
        }

        if (weaponManager.currentWeapon.isShotgun)
        {
            for (int i = 0; i < weaponManager.currentWeapon.pelletCount; i++)
            {
                Vector3 pelletDirection = GetRandomDirectionWithinSpread(gunBarrel.forward, weaponManager.currentWeapon.spreadAngle);
                RaycastHit[] hits = Physics.RaycastAll(gunBarrel.position, pelletDirection, range, ~0, QueryTriggerInteraction.Collide);
                System.Array.Sort(hits, (h1, h2) => h1.distance.CompareTo(h2.distance));

                bool validHitFound = false;
                foreach (var hit in hits)
                {
                    if (hit.collider is BoxCollider && hit.collider.isTrigger)
                    {
                        if (hit.collider.CompareTag("Floor") || hit.collider.gameObject.layer == LayerMask.NameToLayer("GroundLayer"))
                        {
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
            RaycastHit[] hits = Physics.RaycastAll(gunBarrel.position, gunBarrel.forward, range, ~0, QueryTriggerInteraction.Collide);
            System.Array.Sort(hits, (h1, h2) => h1.distance.CompareTo(h2.distance));

            bool validHitFound = false;
            foreach (var hit in hits)
            {
                if (hit.collider is BoxCollider && hit.collider.isTrigger)
                {
                    if (hit.collider.CompareTag("Floor") || hit.collider.gameObject.layer == LayerMask.NameToLayer("GroundLayer"))
                    {
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

    private Vector3 GetRandomDirectionWithinSpread(Vector3 direction, float spreadAngle)
    {
        float randomAngleX = UnityEngine.Random.Range(-spreadAngle, spreadAngle);
        float randomAngleY = UnityEngine.Random.Range(-spreadAngle, spreadAngle);
        float randomAngleZ = UnityEngine.Random.Range(-spreadAngle, spreadAngle);
        Quaternion spreadRotation = Quaternion.Euler(randomAngleX, randomAngleY, 0);
        return spreadRotation * direction;
    }

    private void HandleHit(RaycastHit hit)
    {
        if (HitMarkerManager.Instance != null)
        {
            HitMarkerManager.Instance.ShowHitMarker(hit.point);
        }

        EnemyHealth enemyHealth = hit.collider.GetComponentInParent<EnemyHealth>();
        if (enemyHealth != null)
        {
            bool isHeadshot = hit.collider.CompareTag("Head");
            int damageToApply = isHeadshot ? damage * 2 : damage;
            enemyHealth.TakeDamage(damageToApply);

            if (pointsManager != null)
            {
                int pointsToAdd = isHeadshot ? 50 : 10;
                pointsManager.AddPoints(pointsToAdd);
            }
        }

        if (impactEffect != null)
        {
            Instantiate(impactEffect, hit.point, Quaternion.LookRotation(hit.normal));
        }
    }

    public void UpdateWeaponStats(int newDamage, float newRange, float newFireRate, int newMaxAmmo, float newReloadTime, bool isShotgun, float spreadAngle, int pelletCount, float recoilForce, float recoilIntensity)
    {
        damage = newDamage;
        range = newRange;
        fireRate = newFireRate;
        maxAmmo = newMaxAmmo;
        reloadTime = newReloadTime;
        this.recoilForce = recoilForce;
        this.recoilIntensity = recoilIntensity;
        this.isShotgun = isShotgun;
        this.spreadAngle = spreadAngle;
        this.pelletCount = pelletCount;
        ResetAmmo();
    }

    public void Reload()
    {
        if (isReloading || currentAmmo == maxAmmo || storedAmmo <= 0)
            return;

        isReloading = true;

        // Play reload sound
        if (weaponAudioSource != null && weaponManager?.currentWeapon?.reloadSound != null)
        {
            weaponAudioSource.PlayOneShot(weaponManager.currentWeapon.reloadSound);
        }

        // Special handling for shotgun reload
        if (isShotgun)
        {
            StartCoroutine(ShotgunReload());
        }
        else // Normal weapon reload
        {
            Invoke(nameof(FinishReload), weaponManager.currentWeapon.reloadTime);
        }
    }

    private IEnumerator ShotgunReload()
    {
        float reloadPerShell = weaponManager.currentWeapon.reloadTime / maxAmmo;

        while (currentAmmo < maxAmmo && storedAmmo > 0)
        {
            yield return new WaitForSeconds(reloadPerShell);

            if (!isReloading) // Check if reload was cancelled
                yield break;

            currentAmmo++;
            storedAmmo--;
            UpdateAmmoDisplay();
        }

        isReloading = false;
    }


    private void FinishReload()
    {
        int ammoNeeded = maxAmmo - currentAmmo;
        int ammoCanTake = Mathf.Min(ammoNeeded, storedAmmo);

        currentAmmo += ammoCanTake;
        storedAmmo -= ammoCanTake;

        isReloading = false;
        UpdateAmmoDisplay();
    }

    public void CancelReload()
    {
        if (isReloading)
        {
            StopAllCoroutines();
            CancelInvoke(nameof(FinishReload));
            isReloading = false;
            Debug.Log("Reload cancelled");
        }
    }

    public void ResetAmmo()
    {
        currentAmmo = maxAmmo;
        UpdateAmmoDisplay();
    }

    private void UpdateAmmoDisplay()
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
            return;
        }

        gunBarrel = weaponManager.currentWeaponModel.transform.Find("GunBarrel");
        weaponAudioSource = weaponManager.currentWeaponModel.GetComponent<AudioSource>();

        originalWeaponPosition = weaponManager.currentWeaponModel.transform.localPosition;
        originalWeaponRotation = weaponManager.currentWeaponModel.transform.localRotation;
    }
}