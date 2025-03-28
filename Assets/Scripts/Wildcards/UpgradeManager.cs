using UnityEngine;
using System.Collections.Generic;

public class UpgradeManager : MonoBehaviour
{
    public static UpgradeManager Instance { get; private set; }

    [System.Serializable]
    public class UpgradeEffect
    {
        public string upgradeName;
        public float healthIncrease = 0f;
        public float regenIncrease = 0f;
        public float damageReduction = 0f;
        [TextArea] public string description;
    }

    [Header("Health Upgrades")]
    public List<UpgradeEffect> healthUpgrades;
    private int currentHealthUpgradeIndex = 0;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
        currentHealthUpgradeIndex = 0;
    }

    public void ApplyHealthUpgrade()
    {
        if (currentHealthUpgradeIndex >= healthUpgrades.Count) return;

        var upgrade = healthUpgrades[currentHealthUpgradeIndex];
        PlayerHealth playerHealth = FindObjectOfType<PlayerHealth>();

        if (playerHealth != null)
        {
            if (upgrade.healthIncrease > 0)
            {
                playerHealth.maxHealth += upgrade.healthIncrease;
                playerHealth.AddHealth(upgrade.healthIncrease);
            }

            if (upgrade.regenIncrease > 0)
            {
                playerHealth.regenRate += upgrade.regenIncrease;
            }

            Debug.Log($"Applied upgrade: {upgrade.upgradeName}");
        }

        currentHealthUpgradeIndex = Mathf.Min(currentHealthUpgradeIndex + 1, healthUpgrades.Count - 1);
    }

    public UpgradeEffect GetCurrentHealthUpgrade()
    {
        if (currentHealthUpgradeIndex < healthUpgrades.Count)
            return healthUpgrades[currentHealthUpgradeIndex];
        return null;
    }
}