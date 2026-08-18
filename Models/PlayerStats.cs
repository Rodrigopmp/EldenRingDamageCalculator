using CommunityToolkit.Mvvm.ComponentModel;

namespace EldenRingDamageCalculator.Models;

public partial class PlayerStats : ObservableObject
{
    [ObservableProperty]
    public partial decimal Strength { get; set; } = 10m;

    [ObservableProperty]
    public partial decimal Dexterity { get; set; } = 10m;

    [ObservableProperty]
    public partial decimal Intelligence { get; set; } = 10m;

    [ObservableProperty]
    public partial decimal Faith { get; set; } = 10m;

    [ObservableProperty]
    public partial decimal Arcane { get; set; } = 10m;
}