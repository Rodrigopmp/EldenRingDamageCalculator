namespace EldenRingDamageCalculator.Models;

public class Weapon
{
    public string Name { get; set; } = "";

    public int MaxUpgradeLevel { get; set; }

    public bool AllowsCustomAffinity { get; set; }

    public override string ToString()
    {
        return Name;
    }
}