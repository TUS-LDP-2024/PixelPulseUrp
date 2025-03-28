// UpgradeTypes.cs
public enum UpgradeType
{
    HealthBoost,
    // Future upgrades will be added here
    // DefenseBoost,
    // RegenBoost,
    // etc.
}

[System.Serializable]
public class UpgradeCard
{
    public UpgradeType type;
    public string title;
    public string description;
    public float value; // Amount to boost (e.g., +20 health)
}