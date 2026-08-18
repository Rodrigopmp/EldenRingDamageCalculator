using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using EldenRingDamageCalculator.Models;

namespace EldenRingDamageCalculator.ViewModels;

public partial class MainViewModel : ViewModelBase
{
    public Buff GoldenVow { get; } = new Buff
    {
        Name = "Golden Vow",
        DamageBonusPercent = 15m
    };

    [ObservableProperty]
    public partial decimal BaseDamage { get; set; } = 200m;

    [ObservableProperty]
    public partial decimal FinalDamage { get; set; } = 200m;

    [RelayCommand]
    private void CalculateDamage()
    {
        decimal multiplier = 1 + (GoldenVow.DamageBonusPercent / 100m);

        FinalDamage = BaseDamage * multiplier;
    }
}