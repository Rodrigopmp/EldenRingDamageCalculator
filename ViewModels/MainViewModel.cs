using System;
using System.Collections.ObjectModel;
using System.Linq;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using EldenRingDamageCalculator.Enums;
using EldenRingDamageCalculator.Models;
using EldenRingDamageCalculator.Services;

namespace EldenRingDamageCalculator.ViewModels;

public partial class MainViewModel : ViewModelBase
{
    private readonly DamageCalculator _damageCalculator = new();
    private readonly BuffStackingService _buffStackingService = new();

    public PlayerStats Stats { get; } = new();

    public ObservableCollection<Weapon> Weapons { get; } = new();

    public ObservableCollection<AshOfWar> AshesOfWar { get; } = new();

    public ObservableCollection<WeaponAffinity> Affinities { get; }
        = new(Enum.GetValues<WeaponAffinity>());

    public ObservableCollection<Talisman> Talismans { get; } = new();

    public ObservableCollection<Buff> AuraBuffs { get; } = new();

    public ObservableCollection<Buff> BodyBuffs { get; } = new();

    public ObservableCollection<ArmorPiece> HeadArmor { get; } = new();

    public ObservableCollection<ArmorPiece> ChestArmor { get; } = new();

    public ObservableCollection<ArmorPiece> ArmArmor { get; } = new();

    public ObservableCollection<ArmorPiece> LegArmor { get; } = new();

    public ObservableCollection<Boss> Bosses { get; } = new();

    public ObservableCollection<BossPhase> BossPhases { get; } = new();

    [ObservableProperty]
    public partial Weapon? SelectedWeapon { get; set; }

    [ObservableProperty]
    public partial AshOfWar? SelectedAshOfWar { get; set; }

    [ObservableProperty]
    public partial WeaponAffinity SelectedAffinity { get; set; }
        = WeaponAffinity.Standard;

    [ObservableProperty]
    public partial decimal WeaponUpgradeLevel { get; set; }

    [ObservableProperty]
    public partial Talisman? SelectedTalisman1 { get; set; }

    [ObservableProperty]
    public partial Talisman? SelectedTalisman2 { get; set; }

    [ObservableProperty]
    public partial Talisman? SelectedTalisman3 { get; set; }

    [ObservableProperty]
    public partial Talisman? SelectedTalisman4 { get; set; }

    [ObservableProperty]
    public partial ArmorPiece? SelectedHeadArmor { get; set; }

    [ObservableProperty]
    public partial ArmorPiece? SelectedChestArmor { get; set; }

    [ObservableProperty]
    public partial ArmorPiece? SelectedArmArmor { get; set; }

    [ObservableProperty]
    public partial ArmorPiece? SelectedLegArmor { get; set; }

    [ObservableProperty]
    public partial Boss? SelectedBoss { get; set; }

    [ObservableProperty]
    public partial BossPhase? SelectedBossPhase { get; set; }

    [ObservableProperty]
    public partial decimal BaseDamage { get; set; } = 200m;

    [ObservableProperty]
    public partial decimal FinalDamage { get; set; } = 200m;

    [ObservableProperty]
    public partial string ValidationMessage { get; set; } = "";

    public MainViewModel()
    {
        LoadTemporaryData();
    }

    private void LoadTemporaryData()
    {
        Weapons.Add(new Weapon
        {
            Name = "Greatsword",
            MaxUpgradeLevel = 25,
            AllowsCustomAffinity = true
        });

        Weapons.Add(new Weapon
        {
            Name = "Uchigatana",
            MaxUpgradeLevel = 25,
            AllowsCustomAffinity = true
        });

        AshesOfWar.Add(new AshOfWar
        {
            Name = "Lion's Claw",
            NativeAffinity = WeaponAffinity.Heavy
        });

        AshesOfWar.Add(new AshOfWar
        {
            Name = "Unsheathe",
            NativeAffinity = WeaponAffinity.Keen
        });

        var noTalisman = new Talisman
        {
            Name = "Nenhum",
            IsNone = true
        };

        Talismans.Add(noTalisman);

        Talismans.Add(new Talisman
        {
            Name = "Axe Talisman",
            EffectDescription = "Aumenta ataques carregados."
        });

        Talismans.Add(new Talisman
        {
            Name = "Shard of Alexander",
            EffectDescription = "Aumenta o dano de skills."
        });

        AuraBuffs.Add(new Buff
        {
            Name = "Golden Vow",
            DamageBonusPercent = 15m,
            Category = BuffCategory.Aura,
            StackGroup = "Aura Buff",
            EffectDescription = "Aumenta o dano geral."
        });

        AuraBuffs.Add(new Buff
        {
            Name = "Rallying Standard",
            DamageBonusPercent = 20m,
            Category = BuffCategory.Aura,
            StackGroup = "Aura Buff",
            EffectDescription = "Não stacka com Golden Vow."
        });

        BodyBuffs.Add(new Buff
        {
            Name = "Flame, Grant Me Strength",
            DamageBonusPercent = 20m,
            Category = BuffCategory.Body,
            StackGroup = "Body Buff",
            EffectDescription = "Aumenta dano Physical e Fire."
        });

        BodyBuffs.Add(new Buff
        {
            Name = "Howl of Shabriri",
            DamageBonusPercent = 25m,
            Category = BuffCategory.Body,
            StackGroup = "Body Buff",
            EffectDescription = "Aumenta dano, mas também dano recebido."
        });

        AddEmptyArmorSlots();

        var malenia = new Boss
        {
            Name = "Malenia, Blade of Miquella"
        };

        malenia.Phases.Add(new BossPhase
        {
            Name = "Phase 1"
        });

        malenia.Phases.Add(new BossPhase
        {
            Name = "Phase 2"
        });

        Bosses.Add(malenia);

        SelectedWeapon = Weapons.FirstOrDefault();

        SelectedAshOfWar = AshesOfWar.FirstOrDefault();

        SelectedTalisman1 = noTalisman;
        SelectedTalisman2 = noTalisman;
        SelectedTalisman3 = noTalisman;
        SelectedTalisman4 = noTalisman;

        SelectedHeadArmor = HeadArmor.FirstOrDefault();
        SelectedChestArmor = ChestArmor.FirstOrDefault();
        SelectedArmArmor = ArmArmor.FirstOrDefault();
        SelectedLegArmor = LegArmor.FirstOrDefault();

        SelectedBoss = Bosses.FirstOrDefault();
    }

    private void AddEmptyArmorSlots()
    {
        HeadArmor.Add(new ArmorPiece
        {
            Name = "Nenhum",
            Slot = ArmorSlot.Head,
            IsNone = true
        });

        ChestArmor.Add(new ArmorPiece
        {
            Name = "Nenhum",
            Slot = ArmorSlot.Chest,
            IsNone = true
        });

        ArmArmor.Add(new ArmorPiece
        {
            Name = "Nenhum",
            Slot = ArmorSlot.Arms,
            IsNone = true
        });

        LegArmor.Add(new ArmorPiece
        {
            Name = "Nenhum",
            Slot = ArmorSlot.Legs,
            IsNone = true
        });
    }

    partial void OnSelectedBossChanged(Boss? value)
    {
        BossPhases.Clear();

        if (value is null)
        {
            SelectedBossPhase = null;
            return;
        }

        foreach (var phase in value.Phases)
        {
            BossPhases.Add(phase);
        }

        SelectedBossPhase = BossPhases.FirstOrDefault();
    }

    [RelayCommand]
    private void CalculateDamage()
    {
        var selectedBuffs = AuraBuffs
            .Concat(BodyBuffs)
            .Where(buff => buff.IsSelected)
            .ToList();

        ValidationMessage =
            _buffStackingService.FindConflicts(selectedBuffs);

        if (!string.IsNullOrWhiteSpace(ValidationMessage))
        {
            FinalDamage = BaseDamage;
            return;
        }

        FinalDamage =
            _damageCalculator.CalculateQuickDamage(
                BaseDamage,
                selectedBuffs);
    }
}