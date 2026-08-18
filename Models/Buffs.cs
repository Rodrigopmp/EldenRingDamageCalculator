using CommunityToolkit.Mvvm.ComponentModel;
using EldenRingDamageCalculator.Enums;

namespace EldenRingDamageCalculator.Models;

public partial class Buff : ObservableObject
{
    public string Name { get; set; } = "";

    public decimal DamageBonusPercent { get; set; }

    public BuffCategory Category { get; set; }

    public string StackGroup { get; set; } = "";

    public string EffectDescription { get; set; } = "";

    [ObservableProperty]
    public partial bool IsSelected { get; set; }

    public override string ToString()
    {
        return Name;
    }
}