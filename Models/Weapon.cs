using EldenRingDamageCalculator.Enums;

namespace EldenRingDamageCalculator.Models;

public class Weapon
{
    public string DataKey { get; set; } = "";

    public string Name { get; set; } = "";

    public string Category { get; set; } = "";

    public WeaponUpgradeType UpgradeType { get; set; }

    public int MaxUpgradeLevel { get; set; }

    public bool AllowsCustomAffinity { get; set; }

    public override string ToString()
    {
        return Name;
    }
}