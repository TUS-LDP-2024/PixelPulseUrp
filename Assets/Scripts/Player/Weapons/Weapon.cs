using UnityEngine;

[CreateAssetMenu(fileName = "NewWeapon", menuName = "Weapon", order = 51)]
public class Weapon : ScriptableObject
{
    public string weaponName; // Name of the weapon
    public int damage;        // Damage per shot
    public float range;       // Range of the weapon
    public float fireRate;    // Time between shots
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