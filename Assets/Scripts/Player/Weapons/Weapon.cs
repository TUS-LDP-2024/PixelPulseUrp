using UnityEngine;
using System.Collections;

[CreateAssetMenu(fileName = "NewWeapon", menuName = "Weapon", order = 51)]
public class Weapon : ScriptableObject
{
    [Header("Weapon Stats")]
    public string weaponName; // Name of the weapon
    public int damage;        // Damage per shot
    public float range;       // Range of the weapon
    public float fireRate;    // Time between shots

    [Header("Ammo Settings")]
    public int maxAmmo = 30;  // Maximum ammo capacity
    public float reloadTime = 2f; // Time it takes to reload (full magazine)

    [Header("Recoil Settings")]
    public float recoilForce = 1f; // Base recoil force
    public float recoilIntensity = 1f; // Multiplier for recoil intensity

    [Header("Weapon Model")]
    public GameObject modelPrefab; // Prefab for the weapon model

    [Header("Audio Clips")]
    public AudioClip shootSound; // Sound effect for shooting
    public AudioClip reloadSound; // Sound effect for reloading

    [Header("Blend Shape Settings")]
    public int magEjectBlendShapeIndex = 0; // Index of the MagEject blend shape
    public int roundCycleBlendShapeIndex = 1; // Index of the RoundCycle blend shape
    public float blendShapeSpeed = 5f; // Speed of blend shape animation

    [Header("Shotgun Settings")]
    public bool isShotgun = false; // Is this weapon a shotgun?
    public float shellReloadInterval = 0.5f; // Time between shell reloads for shotguns
    public float spreadAngle = 10f; // Spread angle for shotgun pellets (in degrees)
    public int pelletCount = 8; // Number of pellets fired per shot

    // Method to instantiate the weapon model
    public GameObject InstantiateModel(Transform parent)
    {
        if (modelPrefab != null)
        {
            GameObject weaponInstance = Instantiate(modelPrefab, parent);
            Debug.Log("Weapon model instantiated: " + weaponInstance.name);

            // Add the WeaponInstance component to store runtime data
            WeaponInstance weaponInstanceComponent = weaponInstance.AddComponent<WeaponInstance>();

            // Get the SkinnedMeshRenderer from the instantiated model
            weaponInstanceComponent.skinnedMeshRenderer = weaponInstance.GetComponentInChildren<SkinnedMeshRenderer>();
            if (weaponInstanceComponent.skinnedMeshRenderer == null)
            {
                Debug.LogError("SkinnedMeshRenderer not found on the weapon model!");
            }
            else
            {
                Debug.Log("SkinnedMeshRenderer found: " + weaponInstanceComponent.skinnedMeshRenderer.name);
                Debug.Log("Blend Shape Count: " + weaponInstanceComponent.skinnedMeshRenderer.sharedMesh.blendShapeCount);
            }

            return weaponInstance;
        }
        Debug.LogError("Model prefab is null!");
        return null;
    }

    // Trigger the reload animation
    public void PlayReloadAnimation(GameObject weaponInstance, MonoBehaviour coroutineStarter)
    {
        if (weaponInstance == null)
        {
            Debug.LogError("Weapon instance is null!");
            return;
        }

        // Get the WeaponInstance component
        WeaponInstance weaponInstanceComponent = weaponInstance.GetComponent<WeaponInstance>();
        if (weaponInstanceComponent == null)
        {
            Debug.LogError("WeaponInstance component not found!");
            return;
        }

        if (weaponInstanceComponent.skinnedMeshRenderer == null)
        {
            Debug.LogError("SkinnedMeshRenderer not found in WeaponInstance!");
            return;
        }

        // Start the reload animation coroutine
        if (coroutineStarter != null)
        {
            Debug.Log("Starting ReloadAnimation coroutine");
            coroutineStarter.StartCoroutine(PlayReloadAnimationCoroutine(weaponInstanceComponent.skinnedMeshRenderer));
        }
        else
        {
            Debug.LogError("Coroutine starter is null!");
        }
    }

    private IEnumerator PlayReloadAnimationCoroutine(SkinnedMeshRenderer skinnedMeshRenderer)
    {
        Debug.Log("ReloadAnimation started");

        // Eject the magazine (MagEject blend shape from 0 to 100)
        float magEjectValue = 0f;
        while (magEjectValue < 100f)
        {
            magEjectValue += blendShapeSpeed * Time.deltaTime;
            skinnedMeshRenderer.SetBlendShapeWeight(magEjectBlendShapeIndex, magEjectValue);
            Debug.Log($"MagEject Value: {magEjectValue}");
            yield return null;
        }

        // Insert the magazine (MagEject blend shape from 100 to 0)
        while (magEjectValue > 0f)
        {
            magEjectValue -= blendShapeSpeed * Time.deltaTime;
            skinnedMeshRenderer.SetBlendShapeWeight(magEjectBlendShapeIndex, magEjectValue);
            Debug.Log($"MagEject Value: {magEjectValue}");
            yield return null;
        }

        // Cock the gun (RoundCycle blend shape from 0 to 100)
        float roundCycleValue = 0f;
        while (roundCycleValue < 100f)
        {
            roundCycleValue += blendShapeSpeed * Time.deltaTime;
            skinnedMeshRenderer.SetBlendShapeWeight(roundCycleBlendShapeIndex, roundCycleValue);
            Debug.Log($"RoundCycle Value: {roundCycleValue}");
            yield return null;
        }

        // Reset the RoundCycle blend shape
        while (roundCycleValue > 0f)
        {
            roundCycleValue -= blendShapeSpeed * Time.deltaTime;
            skinnedMeshRenderer.SetBlendShapeWeight(roundCycleBlendShapeIndex, roundCycleValue);
            Debug.Log($"RoundCycle Value: {roundCycleValue}");
            yield return null;
        }

        Debug.Log("ReloadAnimation completed");
    }
}