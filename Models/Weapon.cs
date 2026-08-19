using System.Collections.Generic;
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

    public bool IsDlc { get; set; }


    // =========================
    // REQUISITOS
    // =========================

    public int StrengthRequirement { get; set; }

    public int DexterityRequirement { get; set; }

    public int IntelligenceRequirement { get; set; }

    public int FaithRequirement { get; set; }

    public int ArcaneRequirement { get; set; }


    // =========================
    // AFINIDADES
    // =========================

    public List<WeaponAffinity> AvailableAffinities
        { get; set; } = new();

    public Dictionary<WeaponAffinity, WeaponVariant> Variants
        { get; set; } = new();


    // =========================
    // AQUISIÇÃO
    // =========================

    public WeaponAcquisition? Acquisition { get; set; }


    public bool HasAcquisitionData
    {
        get
        {
            return Acquisition is not null;
        }
    }


    public string AcquisitionLocationDisplay
    {
        get
        {
            if (Acquisition is null)
            {
                return "Ainda não cadastrado.";
            }


            bool hasRegion =
                !string.IsNullOrWhiteSpace(
                    Acquisition.Region);

            bool hasLocation =
                !string.IsNullOrWhiteSpace(
                    Acquisition.Location);


            if (hasRegion && hasLocation)
            {
                return
                    $"{Acquisition.Region} — {Acquisition.Location}";
            }


            if (hasLocation)
            {
                return Acquisition.Location;
            }


            if (hasRegion)
            {
                return Acquisition.Region;
            }


            return "Local não informado.";
        }
    }


    public string AcquisitionMethodDisplay
    {
        get
        {
            if (Acquisition is null ||
                string.IsNullOrWhiteSpace(
                    Acquisition.AcquisitionType))
            {
                return "-";
            }


            return Acquisition.AcquisitionType;
        }
    }


    public string AcquisitionDescriptionDisplay
    {
        get
        {
            if (Acquisition is null)
            {
                return
                    "Os dados de obtenção desta arma ainda serão adicionados.";
            }


            if (string.IsNullOrWhiteSpace(
                    Acquisition.Description))
            {
                return
                    "Nenhuma informação adicional.";
            }


            return Acquisition.Description;
        }
    }


    public string AcquisitionSourceDisplay
    {
        get
        {
            if (Acquisition is null ||
                string.IsNullOrWhiteSpace(
                    Acquisition.SourceName))
            {
                return "";
            }


            return $"Fonte: {Acquisition.SourceName}";
        }
    }


    // =========================
    // DISPLAY
    // =========================

    public string UpgradeMaterialDisplay
    {
        get
        {
            return UpgradeType switch
            {
                WeaponUpgradeType.SmithingStone =>
                    "Smithing Stone",

                WeaponUpgradeType.SomberSmithingStone =>
                    "Somber Smithing Stone",

                _ =>
                    "Sem upgrade"
            };
        }
    }


    public string GameOriginDisplay
    {
        get
        {
            return IsDlc
                ? "Shadow of the Erdtree"
                : "Base Game";
        }
    }


    public string RequirementsDisplay
    {
        get
        {
            var requirements =
                new List<string>();


            if (StrengthRequirement > 0)
            {
                requirements.Add(
                    $"STR {StrengthRequirement}");
            }


            if (DexterityRequirement > 0)
            {
                requirements.Add(
                    $"DEX {DexterityRequirement}");
            }


            if (IntelligenceRequirement > 0)
            {
                requirements.Add(
                    $"INT {IntelligenceRequirement}");
            }


            if (FaithRequirement > 0)
            {
                requirements.Add(
                    $"FAI {FaithRequirement}");
            }


            if (ArcaneRequirement > 0)
            {
                requirements.Add(
                    $"ARC {ArcaneRequirement}");
            }


            if (requirements.Count == 0)
            {
                return "Sem requisitos";
            }


            return string.Join(
                "   |   ",
                requirements);
        }
    }


    // =========================
    // PEGAR VARIANTE
    // =========================

    public WeaponVariant? GetVariant(
        WeaponAffinity affinity)
    {
        if (Variants.TryGetValue(
                affinity,
                out WeaponVariant? variant))
        {
            return variant;
        }


        if (Variants.TryGetValue(
                WeaponAffinity.Standard,
                out WeaponVariant? standardVariant))
        {
            return standardVariant;
        }


        foreach (
            WeaponVariant availableVariant
            in Variants.Values)
        {
            return availableVariant;
        }


        return null;
    }


    public override string ToString()
    {
        return Name;
    }
}


// ============================================================
// VARIANTE
// ============================================================

public class WeaponVariant
{
    public string FullName { get; set; } = "";

    public WeaponAffinity Affinity { get; set; }


    public Dictionary<string, int> Requirements
        { get; set; } = new();


    public List<WeaponUpgradeStats> UpgradeLevels
        { get; set; } = new();


    public Dictionary<
        DamageType,
        Dictionary<string, AttributeCorrection>>
        AttackElementCorrect
        { get; set; } = new();


    public Dictionary<DamageType, double[]>
        ScalingCurves
        { get; set; } = new();
}


// ============================================================
// UPGRADE
// ============================================================

public class WeaponUpgradeStats
{
    public Dictionary<DamageType, double> Attack
        { get; set; } = new();


    public Dictionary<string, double> AttributeScaling
        { get; set; } = new();
}


// ============================================================
// CORREÇÃO DE SCALING
// ============================================================

public class AttributeCorrection
{
    public bool UseUpgradedScaling { get; set; }

    public double FixedCorrection { get; set; }
}