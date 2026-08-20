using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Avalonia.Threading;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using EldenRingDamageCalculator.Enums;
using EldenRingDamageCalculator.Models;
using EldenRingDamageCalculator.Services;

namespace EldenRingDamageCalculator.ViewModels;

public partial class MainViewModel : ViewModelBase
{
    private readonly DamageCalculator
        _damageCalculator = new();

    private readonly BuffStackingService
        _buffStackingService = new();

    private readonly WeaponCatalogService
        _weaponCatalogService = new();

    private readonly WeaponAcquisitionService
        _weaponAcquisitionService = new();

    private readonly WeaponAttackCalculatorService
        _weaponAttackCalculator = new();


    // =========================
    // ATRIBUTOS
    // =========================

    public PlayerStats Stats { get; } =
        new();


    // =========================
    // ARMAS
    // =========================

    public ObservableCollection<Weapon>
        Weapons { get; } = new();


    [ObservableProperty]
    public partial Weapon? SelectedWeapon
        { get; set; }


    [ObservableProperty]
    public partial decimal WeaponUpgradeLevel
        { get; set; }


    [ObservableProperty]
    public partial decimal WeaponUpgradeMaximum
        { get; set; } = 25m;


    [ObservableProperty]
    public partial bool CanSelectAffinity
        { get; set; }


    [ObservableProperty]
    public partial string WeaponCatalogStatus
        { get; set; } =
        "Carregando armas...";


    [ObservableProperty]
    public partial string AcquisitionCatalogStatus
        { get; set; } =
        "Carregando dados de obtenção...";


    // =========================
    // AFINIDADES
    // =========================

    public ObservableCollection<WeaponAffinity>
        AvailableAffinities
        { get; } = new();


    [ObservableProperty]
    public partial WeaponAffinity
        SelectedAffinity
        { get; set; } =
        WeaponAffinity.Standard;


    // =========================
    // ATTACK RATING
    // =========================

    [ObservableProperty]
    public partial int PhysicalBase
        { get; set; }

    [ObservableProperty]
    public partial int PhysicalScaling
        { get; set; }

    [ObservableProperty]
    public partial int PhysicalAr
        { get; set; }


    [ObservableProperty]
    public partial int MagicBase
        { get; set; }

    [ObservableProperty]
    public partial int MagicScaling
        { get; set; }

    [ObservableProperty]
    public partial int MagicAr
        { get; set; }


    [ObservableProperty]
    public partial int FireBase
        { get; set; }

    [ObservableProperty]
    public partial int FireScaling
        { get; set; }

    [ObservableProperty]
    public partial int FireAr
        { get; set; }


    [ObservableProperty]
    public partial int LightningBase
        { get; set; }

    [ObservableProperty]
    public partial int LightningScaling
        { get; set; }

    [ObservableProperty]
    public partial int LightningAr
        { get; set; }


    [ObservableProperty]
    public partial int HolyBase
        { get; set; }

    [ObservableProperty]
    public partial int HolyScaling
        { get; set; }

    [ObservableProperty]
    public partial int HolyAr
        { get; set; }


    [ObservableProperty]
    public partial int WeaponTotalAr
        { get; set; }


    [ObservableProperty]
    public partial string StrengthScalingDisplay
        { get; set; } = "-";

    [ObservableProperty]
    public partial string DexterityScalingDisplay
        { get; set; } = "-";

    [ObservableProperty]
    public partial string IntelligenceScalingDisplay
        { get; set; } = "-";

    [ObservableProperty]
    public partial string FaithScalingDisplay
        { get; set; } = "-";

    [ObservableProperty]
    public partial string ArcaneScalingDisplay
        { get; set; } = "-";


    [ObservableProperty]
    public partial string WeaponRequirementWarning
        { get; set; } = "";


    [ObservableProperty]
    public partial string WeaponArStatus
        { get; set; } =
        "Dados: regulation v1.14";


    // =========================
    // CINZAS DE GUERRA
    // =========================

    public ObservableCollection<AshOfWar>
        AshesOfWar { get; } = new();


    [ObservableProperty]
    public partial AshOfWar?
        SelectedAshOfWar
        { get; set; }


    // =========================
    // TALISMÃS
    // =========================

    public ObservableCollection<Talisman>
        Talismans { get; } = new();


    [ObservableProperty]
    public partial Talisman?
        SelectedTalisman1
        { get; set; }


    [ObservableProperty]
    public partial Talisman?
        SelectedTalisman2
        { get; set; }


    [ObservableProperty]
    public partial Talisman?
        SelectedTalisman3
        { get; set; }


    [ObservableProperty]
    public partial Talisman?
        SelectedTalisman4
        { get; set; }


    // =========================
    // BUFFS
    // =========================

    public ObservableCollection<Buff>
        AuraBuffs { get; } = new();

    public ObservableCollection<Buff>
        BodyBuffs { get; } = new();


    // =========================
    // ARMADURA
    // =========================

    public ObservableCollection<ArmorPiece>
        HeadArmor { get; } = new();

    public ObservableCollection<ArmorPiece>
        ChestArmor { get; } = new();

    public ObservableCollection<ArmorPiece>
        ArmArmor { get; } = new();

    public ObservableCollection<ArmorPiece>
        LegArmor { get; } = new();


    [ObservableProperty]
    public partial ArmorPiece?
        SelectedHeadArmor
        { get; set; }


    [ObservableProperty]
    public partial ArmorPiece?
        SelectedChestArmor
        { get; set; }


    [ObservableProperty]
    public partial ArmorPiece?
        SelectedArmArmor
        { get; set; }


    [ObservableProperty]
    public partial ArmorPiece?
        SelectedLegArmor
        { get; set; }


    // =========================
    // BOSSES
    // =========================

    public ObservableCollection<Boss>
        Bosses { get; } = new();

    public ObservableCollection<BossPhase>
        BossPhases { get; } = new();


    [ObservableProperty]
    public partial Boss?
        SelectedBoss
        { get; set; }


    [ObservableProperty]
    public partial BossPhase?
        SelectedBossPhase
        { get; set; }


    // =========================
    // QUICK DAMAGE
    // =========================

    [ObservableProperty]
    public partial decimal BaseDamage
        { get; set; } = 200m;


    [ObservableProperty]
    public partial decimal FinalDamage
        { get; set; } = 200m;


    [ObservableProperty]
    public partial string ValidationMessage
        { get; set; } = "";


    // =========================
    // CONSTRUTOR
    // =========================

    public MainViewModel()
    {
        Stats.PropertyChanged +=
            (_, _) =>
                RecalculateWeaponAr();

        LoadTemporaryData();

        _ =
            LoadWeaponCatalogAsync();
    }


    // =========================
    // CARREGAR ARMAS
    // =========================

    private async Task LoadWeaponCatalogAsync()
    {
        try
        {
            WeaponCatalogStatus =
                "Carregando catálogo de armas...";

            AcquisitionCatalogStatus =
                "Carregando dados de obtenção...";


            var loadedWeapons =
                await _weaponCatalogService
                    .LoadWeaponsAsync();


            int acquisitionCount =
                await _weaponAcquisitionService
                    .ApplyAsync(
                        loadedWeapons);


            var missingWeapons =
                await _weaponAcquisitionService
                    .GenerateMissingReportAsync(
                        loadedWeapons);


            await Dispatcher.UIThread.InvokeAsync(
                () =>
                {
                    Weapons.Clear();


                    foreach (
                        Weapon weapon
                        in loadedWeapons)
                    {
                        Weapons.Add(
                            weapon);
                    }


                    SelectedWeapon =
                        Weapons.FirstOrDefault(
                            weapon =>
                                weapon.Name
                                ==
                                "Greatsword")
                        ??
                        Weapons.FirstOrDefault();


                    WeaponCatalogStatus =
                        $"{Weapons.Count} armas carregadas.";


                    AcquisitionCatalogStatus =
                        $"{acquisitionCount} armas com dados de obtenção. "
                        +
                        $"{missingWeapons.Count} faltando. "
                        +
                        "Relatório gerado em Data/missing-weapon-acquisitions.txt";
                });
        }
        catch (Exception exception)
        {
            await Dispatcher.UIThread.InvokeAsync(
                () =>
                {
                    WeaponCatalogStatus =
                        "Erro ao carregar catálogo: "
                        +
                        exception.Message;

                    AcquisitionCatalogStatus =
                        "Dados de obtenção indisponíveis.";
                });
        }
    }


    // =========================
    // TROCA DE ARMA
    // =========================

    partial void OnSelectedWeaponChanged(
        Weapon? value)
    {
        AvailableAffinities.Clear();


        if (value is null)
        {
            ResetWeaponAr();

            return;
        }


        WeaponUpgradeMaximum =
            value.MaxUpgradeLevel;


        foreach (
            WeaponAffinity affinity
            in value.AvailableAffinities)
        {
            AvailableAffinities.Add(
                affinity);
        }


        if (AvailableAffinities.Count == 0)
        {
            AvailableAffinities.Add(
                WeaponAffinity.Standard);
        }


        CanSelectAffinity =
            value.AllowsCustomAffinity
            &&
            AvailableAffinities.Count > 1;


        if (AvailableAffinities.Contains(
                WeaponAffinity.Standard))
        {
            SelectedAffinity =
                WeaponAffinity.Standard;
        }
        else
        {
            SelectedAffinity =
                AvailableAffinities[0];
        }


        if (WeaponUpgradeLevel
            >
            WeaponUpgradeMaximum)
        {
            WeaponUpgradeLevel =
                WeaponUpgradeMaximum;
        }


        RecalculateWeaponAr();
    }


    // =========================
    // TROCA DE AFINIDADE
    // =========================

    partial void OnSelectedAffinityChanged(
        WeaponAffinity value)
    {
        RecalculateWeaponAr();
    }


    // =========================
    // TROCA DE UPGRADE
    // =========================

    partial void OnWeaponUpgradeLevelChanged(
        decimal value)
    {
        RecalculateWeaponAr();
    }


    // =========================
    // RECALCULAR AR
    // =========================

    private void RecalculateWeaponAr()
    {
        if (SelectedWeapon is null)
        {
            ResetWeaponAr();

            return;
        }


        int upgradeLevel =
            (int)decimal.Truncate(
                WeaponUpgradeLevel);


        WeaponAttackResult result =
            _weaponAttackCalculator
                .Calculate(
                    SelectedWeapon,
                    SelectedAffinity,
                    upgradeLevel,
                    Stats);


        PhysicalBase =
            GetDisplayedValue(
                result.BaseAttack,
                DamageType.Physical);


        PhysicalScaling =
            GetDisplayedValue(
                result.ScalingBonus,
                DamageType.Physical);


        PhysicalAr =
            GetDisplayedValue(
                result.TotalAttack,
                DamageType.Physical);


        MagicBase =
            GetDisplayedValue(
                result.BaseAttack,
                DamageType.Magic);


        MagicScaling =
            GetDisplayedValue(
                result.ScalingBonus,
                DamageType.Magic);


        MagicAr =
            GetDisplayedValue(
                result.TotalAttack,
                DamageType.Magic);


        FireBase =
            GetDisplayedValue(
                result.BaseAttack,
                DamageType.Fire);


        FireScaling =
            GetDisplayedValue(
                result.ScalingBonus,
                DamageType.Fire);


        FireAr =
            GetDisplayedValue(
                result.TotalAttack,
                DamageType.Fire);


        LightningBase =
            GetDisplayedValue(
                result.BaseAttack,
                DamageType.Lightning);


        LightningScaling =
            GetDisplayedValue(
                result.ScalingBonus,
                DamageType.Lightning);


        LightningAr =
            GetDisplayedValue(
                result.TotalAttack,
                DamageType.Lightning);


        HolyBase =
            GetDisplayedValue(
                result.BaseAttack,
                DamageType.Holy);


        HolyScaling =
            GetDisplayedValue(
                result.ScalingBonus,
                DamageType.Holy);


        HolyAr =
            GetDisplayedValue(
                result.TotalAttack,
                DamageType.Holy);


        WeaponTotalAr =
            PhysicalAr
            +
            MagicAr
            +
            FireAr
            +
            LightningAr
            +
            HolyAr;


        StrengthScalingDisplay =
            FormatScaling(
                result.StrengthScaling);


        DexterityScalingDisplay =
            FormatScaling(
                result.DexterityScaling);


        IntelligenceScalingDisplay =
            FormatScaling(
                result.IntelligenceScaling);


        FaithScalingDisplay =
            FormatScaling(
                result.FaithScaling);


        ArcaneScalingDisplay =
            FormatScaling(
                result.ArcaneScaling);


        WeaponRequirementWarning =
            result.RequirementsMet
                ? ""
                : "⚠ Os requisitos da arma não estão sendo atendidos. "
                  +
                  "O AR está sofrendo penalidade.";
    }


    private static int GetDisplayedValue(
        System.Collections.Generic
            .Dictionary<DamageType, double>
            values,
        DamageType damageType)
    {
        if (!values.TryGetValue(
                damageType,
                out double value))
        {
            return 0;
        }


        return (int)Math.Floor(
            value);
    }


    private static string FormatScaling(
        double scaling)
    {
        if (scaling <= 0)
        {
            return "-";
        }


        return scaling.ToString(
            "0.000");
    }


    private void ResetWeaponAr()
    {
        PhysicalBase = 0;
        PhysicalScaling = 0;
        PhysicalAr = 0;

        MagicBase = 0;
        MagicScaling = 0;
        MagicAr = 0;

        FireBase = 0;
        FireScaling = 0;
        FireAr = 0;

        LightningBase = 0;
        LightningScaling = 0;
        LightningAr = 0;

        HolyBase = 0;
        HolyScaling = 0;
        HolyAr = 0;

        WeaponTotalAr = 0;

        StrengthScalingDisplay = "-";
        DexterityScalingDisplay = "-";
        IntelligenceScalingDisplay = "-";
        FaithScalingDisplay = "-";
        ArcaneScalingDisplay = "-";

        WeaponRequirementWarning = "";
    }


    // =========================
    // DADOS TEMPORÁRIOS
    // =========================

    private void LoadTemporaryData()
    {
        AshesOfWar.Add(
            new AshOfWar
            {
                Name =
                    "Lion's Claw",

                NativeAffinity =
                    WeaponAffinity.Heavy
            });


        AshesOfWar.Add(
            new AshOfWar
            {
                Name =
                    "Unsheathe",

                NativeAffinity =
                    WeaponAffinity.Keen
            });


        var noTalisman =
            new Talisman
            {
                Name =
                    "Nenhum",

                IsNone =
                    true
            };


        Talismans.Add(
            noTalisman);


        Talismans.Add(
            new Talisman
            {
                Name =
                    "Axe Talisman",

                EffectDescription =
                    "Aumenta ataques carregados."
            });


        Talismans.Add(
            new Talisman
            {
                Name =
                    "Shard of Alexander",

                EffectDescription =
                    "Aumenta o dano de skills."
            });


        AuraBuffs.Add(
            new Buff
            {
                Name =
                    "Golden Vow",

                DamageBonusPercent =
                    15m,

                Category =
                    BuffCategory.Aura,

                StackGroup =
                    "Aura Buff",

                EffectDescription =
                    "Aumenta o dano geral."
            });


        AuraBuffs.Add(
            new Buff
            {
                Name =
                    "Rallying Standard",

                DamageBonusPercent =
                    20m,

                Category =
                    BuffCategory.Aura,

                StackGroup =
                    "Aura Buff",

                EffectDescription =
                    "Não stacka com Golden Vow."
            });


        BodyBuffs.Add(
            new Buff
            {
                Name =
                    "Flame, Grant Me Strength",

                DamageBonusPercent =
                    20m,

                Category =
                    BuffCategory.Body,

                StackGroup =
                    "Body Buff",

                EffectDescription =
                    "Aumenta dano Physical e Fire."
            });


        BodyBuffs.Add(
            new Buff
            {
                Name =
                    "Howl of Shabriri",

                DamageBonusPercent =
                    25m,

                Category =
                    BuffCategory.Body,

                StackGroup =
                    "Body Buff",

                EffectDescription =
                    "Aumenta dano, mas também dano recebido."
            });


        AddEmptyArmorSlots();


        var malenia =
            new Boss
            {
                Name =
                    "Malenia, Blade of Miquella"
            };


        malenia.Phases.Add(
            new BossPhase
            {
                Name =
                    "Phase 1"
            });


        malenia.Phases.Add(
            new BossPhase
            {
                Name =
                    "Phase 2"
            });


        Bosses.Add(
            malenia);


        SelectedAshOfWar =
            AshesOfWar.FirstOrDefault();


        SelectedTalisman1 =
            noTalisman;

        SelectedTalisman2 =
            noTalisman;

        SelectedTalisman3 =
            noTalisman;

        SelectedTalisman4 =
            noTalisman;


        SelectedHeadArmor =
            HeadArmor.FirstOrDefault();

        SelectedChestArmor =
            ChestArmor.FirstOrDefault();

        SelectedArmArmor =
            ArmArmor.FirstOrDefault();

        SelectedLegArmor =
            LegArmor.FirstOrDefault();


        SelectedBoss =
            Bosses.FirstOrDefault();
    }


    // =========================
    // ARMADURA VAZIA
    // =========================

    private void AddEmptyArmorSlots()
    {
        HeadArmor.Add(
            new ArmorPiece
            {
                Name =
                    "Nenhum",

                Slot =
                    ArmorSlot.Head,

                IsNone =
                    true
            });


        ChestArmor.Add(
            new ArmorPiece
            {
                Name =
                    "Nenhum",

                Slot =
                    ArmorSlot.Chest,

                IsNone =
                    true
            });


        ArmArmor.Add(
            new ArmorPiece
            {
                Name =
                    "Nenhum",

                Slot =
                    ArmorSlot.Arms,

                IsNone =
                    true
            });


        LegArmor.Add(
            new ArmorPiece
            {
                Name =
                    "Nenhum",

                Slot =
                    ArmorSlot.Legs,

                IsNone =
                    true
            });
    }


    // =========================
    // TROCA DE BOSS
    // =========================

    partial void OnSelectedBossChanged(
        Boss? value)
    {
        BossPhases.Clear();


        if (value is null)
        {
            SelectedBossPhase =
                null;

            return;
        }


        foreach (
            BossPhase phase
            in value.Phases)
        {
            BossPhases.Add(
                phase);
        }


        SelectedBossPhase =
            BossPhases.FirstOrDefault();
    }


    // =========================
    // QUICK DAMAGE
    // =========================

    [RelayCommand]
    private void CalculateDamage()
    {
        var selectedBuffs =
            AuraBuffs
                .Concat(BodyBuffs)
                .Where(
                    buff =>
                        buff.IsSelected)
                .ToList();


        ValidationMessage =
            _buffStackingService
                .FindConflicts(
                    selectedBuffs);


        if (!string.IsNullOrWhiteSpace(
                ValidationMessage))
        {
            FinalDamage =
                BaseDamage;

            return;
        }


        FinalDamage =
            _damageCalculator
                .CalculateQuickDamage(
                    BaseDamage,
                    selectedBuffs);
    }
}