using UnityEngine;
using System.Collections.Generic;

public class UpgradeManager : MonoBehaviour
{
    public static UpgradeManager Instance { get; private set; }

    [System.Serializable]
    public class UpgradeEffect
    {
        public string upgradeName;
        public float healthIncrease;
        public float regenIncrease;
        public float damageReduction;
        [TextArea] public string description;
    }

    [Header("Health Upgrades")]
    public List<UpgradeEffect> healthUpgrades;
    private List<UpgradeEffect> remainingUpgrades;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        remainingUpgrades = new List<UpgradeEffect>(healthUpgrades);
    }

    public UpgradeEffect GetRandomHealthUpgrade()
    {
        if (remainingUpgrades.Count == 0) return null;
        return remainingUpgrades[Random.Range(0, remainingUpgrades.Count)];
    }

    public void ApplyUpgrade(UpgradeEffect upgrade)
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

        remainingUpgrades.Remove(upgrade);
    }
}