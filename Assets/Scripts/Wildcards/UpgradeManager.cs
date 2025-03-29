using UnityEngine;
using System.Collections.Generic;
using StarterAssets;

public class UpgradeManager : MonoBehaviour
{
    public static UpgradeManager Instance { get; private set; }

    [System.Serializable]
    public class HealthUpgrade
    {
        public string upgradeName;
        public float healthIncrease;
        public float regenIncrease;
        public float damageReduction;
        [TextArea] public string description;
    }

    [System.Serializable]
    public class WeaponUpgrade
    {
        public string upgradeName;
        public float damageBoost;
        public float fireRateBoost;
        public float ammoBoost;
        public float reloadSpeedBoost;
        [TextArea] public string description;
    }

    [System.Serializable]
    public class MovementUpgrade
    {
        public string upgradeName;
        public float moveSpeedBoost;
        public float sprintSpeedBoost;
        public float jumpHeightBoost;
        [TextArea] public string description;
    }

    [Header("Health Upgrades")]
    public List<HealthUpgrade> healthUpgrades;

    [Header("Weapon Upgrades")]
    public List<WeaponUpgrade> weaponUpgrades;

    [Header("Movement Upgrades")]
    public List<MovementUpgrade> movementUpgrades;

    private List<HealthUpgrade> remainingHealthUpgrades;
    private List<WeaponUpgrade> remainingWeaponUpgrades;
    private List<MovementUpgrade> remainingMovementUpgrades;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        remainingHealthUpgrades = new List<HealthUpgrade>(healthUpgrades);
        remainingWeaponUpgrades = new List<WeaponUpgrade>(weaponUpgrades);
        remainingMovementUpgrades = new List<MovementUpgrade>(movementUpgrades);
    }

    public HealthUpgrade GetRandomHealthUpgrade()
    {
        if (remainingHealthUpgrades.Count == 0) return null;
        return remainingHealthUpgrades[Random.Range(0, remainingHealthUpgrades.Count)];
    }

    public WeaponUpgrade GetRandomWeaponUpgrade()
    {
        if (remainingWeaponUpgrades.Count == 0) return null;
        return remainingWeaponUpgrades[Random.Range(0, remainingWeaponUpgrades.Count)];
    }

    public MovementUpgrade GetRandomMovementUpgrade()
    {
        if (remainingMovementUpgrades.Count == 0) return null;
        return remainingMovementUpgrades[Random.Range(0, remainingMovementUpgrades.Count)];
    }

    public void ApplyHealthUpgrade(HealthUpgrade upgrade)
    {
        if (upgrade == null) return;

        PlayerHealth player = FindObjectOfType<PlayerHealth>();
        if (player != null)
        {
            if (upgrade.healthIncrease > 0)
                player.IncreaseMaxHealth(upgrade.healthIncrease);
            if (upgrade.regenIncrease > 0)
                player.IncreaseRegenRate(upgrade.regenIncrease);
            if (upgrade.damageReduction > 0)
                player.IncreaseDamageReduction(upgrade.damageReduction);
        }

        remainingHealthUpgrades.Remove(upgrade);
    }

    public void ApplyWeaponUpgrade(WeaponUpgrade upgrade)
    {
        if (upgrade == null) return;

        WeaponManager weaponManager = FindObjectOfType<WeaponManager>();
        if (weaponManager != null)
        {
            weaponManager.ApplyWeaponUpgrade(
                upgrade.damageBoost,
                upgrade.fireRateBoost,
                upgrade.ammoBoost,
                upgrade.reloadSpeedBoost
            );
        }

        remainingWeaponUpgrades.Remove(upgrade);
    }

    public void ApplyMovementUpgrade(MovementUpgrade upgrade)
    {
        if (upgrade == null) return;

        FirstPersonController playerController = FindObjectOfType<FirstPersonController>();
        if (playerController != null)
        {
            if (upgrade.moveSpeedBoost > 0)
                playerController.MoveSpeed += upgrade.moveSpeedBoost;
            if (upgrade.sprintSpeedBoost > 0)
                playerController.SprintSpeed += upgrade.sprintSpeedBoost;
            if (upgrade.jumpHeightBoost > 0)
                playerController.JumpHeight += upgrade.jumpHeightBoost;
        }

        remainingMovementUpgrades.Remove(upgrade);
    }
}