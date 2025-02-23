using UnityEngine;

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
    public float reloadTime = 2f; // Time it takes to reload

    [Header("Weapon Model")]
    public GameObject modelPrefab; // Prefab for the weapon model

    // Method to instantiate the weapon model
    public GameObject InstantiateModel(Transform parent)
    {
        if (modelPrefab != null)
        {
            return Instantiate(modelPrefab, parent);
        }
        return null;
    }
}