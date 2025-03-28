public enum UpgradeType
{
    HealthBoost,
    DamageBoost,
    FireRateBoost,
    AmmoBoost,
    ReloadSpeedBoost
}

[System.Serializable]
public class UpgradeCard
{
    public UpgradeType type;
    public string title;
    public string description;
    public float value;
}