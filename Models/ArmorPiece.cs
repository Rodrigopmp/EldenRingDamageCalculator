using EldenRingDamageCalculator.Enums;

namespace EldenRingDamageCalculator.Models;

public class ArmorPiece
{
    public string Name { get; set; } = "";

    public ArmorSlot Slot { get; set; }

    public string EffectDescription { get; set; } = "";

    public bool IsNone { get; set; }

    public override string ToString()
    {
        return Name;
    }
}