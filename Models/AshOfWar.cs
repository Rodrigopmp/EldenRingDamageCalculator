using EldenRingDamageCalculator.Enums;

namespace EldenRingDamageCalculator.Models;

public class AshOfWar
{
    public string Name { get; set; } = "";

    public WeaponAffinity NativeAffinity { get; set; }

    public string EffectDescription { get; set; } = "";

    public override string ToString()
    {
        return Name;
    }
}