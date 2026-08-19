using System;
using System.Collections.Generic;
using System.Linq;
using EldenRingDamageCalculator.Enums;
using EldenRingDamageCalculator.Models;

namespace EldenRingDamageCalculator.Services;

public class WeaponAttackCalculatorService
{
    private const double IneffectiveAttributePenalty =
        0.4;


    public WeaponAttackResult Calculate(
        Weapon weapon,
        WeaponAffinity affinity,
        int upgradeLevel,
        PlayerStats stats)
    {
        WeaponVariant? variant =
            weapon.GetVariant(
                affinity);


        if (variant is null)
        {
            return new WeaponAttackResult();
        }


        if (variant.UpgradeLevels.Count == 0)
        {
            return new WeaponAttackResult();
        }


        upgradeLevel =
            Math.Clamp(
                upgradeLevel,
                0,
                variant.UpgradeLevels.Count - 1);


        WeaponUpgradeStats upgradeStats =
            variant.UpgradeLevels[
                upgradeLevel];


        WeaponUpgradeStats levelZeroStats =
            variant.UpgradeLevels[0];


        Dictionary<string, int> attributes =
            GetPlayerAttributes(
                stats);


        List<string> ineffectiveAttributes =
            variant.Requirements
                .Where(
                    requirement =>
                        attributes.TryGetValue(
                            requirement.Key,
                            out int value)
                        &&
                        value
                        <
                        requirement.Value)
                .Select(
                    requirement =>
                        requirement.Key)
                .ToList();


        var result =
            new WeaponAttackResult
            {
                RequirementsMet =
                    ineffectiveAttributes.Count
                    == 0,

                IneffectiveAttributes =
                    ineffectiveAttributes
            };


        foreach (
            DamageType damageType
            in Enum.GetValues<DamageType>())
        {
            if (!upgradeStats.Attack.TryGetValue(
                    damageType,
                    out double baseAttack))
            {
                continue;
            }


            if (baseAttack <= 0)
            {
                continue;
            }


            double totalScaling =
                1.0;


            Dictionary<string, AttributeCorrection>
                scalingAttributes =
                    variant.AttackElementCorrect
                        .TryGetValue(
                            damageType,
                            out var corrections)
                        ? corrections
                        : new();


            bool damageTypeHasInvalidRequirement =
                ineffectiveAttributes.Any(
                    attribute =>
                        scalingAttributes
                            .ContainsKey(
                                attribute));


            if (damageTypeHasInvalidRequirement)
            {
                totalScaling =
                    1.0
                    -
                    IneffectiveAttributePenalty;
            }
            else
            {
                foreach (
                    KeyValuePair<
                        string,
                        AttributeCorrection>
                    correction
                    in scalingAttributes)
                {
                    string attribute =
                        correction.Key;


                    if (!attributes.TryGetValue(
                            attribute,
                            out int attributeValue))
                    {
                        continue;
                    }


                    if (!upgradeStats
                        .AttributeScaling
                        .TryGetValue(
                            attribute,
                            out double
                                upgradedScaling))
                    {
                        continue;
                    }


                    double effectiveScaling;


                    if (correction.Value
                        .UseUpgradedScaling)
                    {
                        effectiveScaling =
                            upgradedScaling;
                    }
                    else
                    {
                        if (!levelZeroStats
                            .AttributeScaling
                            .TryGetValue(
                                attribute,
                                out double
                                    baseScaling))
                        {
                            continue;
                        }


                        if (baseScaling == 0)
                        {
                            continue;
                        }


                        effectiveScaling =
                            correction.Value
                                .FixedCorrection
                            *
                            upgradedScaling
                            /
                            baseScaling;
                    }


                    if (!variant
                        .ScalingCurves
                        .TryGetValue(
                            damageType,
                            out double[]?
                                curve))
                    {
                        continue;
                    }


                    if (curve.Length == 0)
                    {
                        continue;
                    }


                    int curveIndex =
                        Math.Clamp(
                            attributeValue,
                            1,
                            curve.Length - 1);


                    double correctedAttribute =
                        curve[curveIndex];


                    totalScaling +=
                        correctedAttribute
                        *
                        effectiveScaling;
                }
            }


            double totalAttack =
                baseAttack
                *
                totalScaling;


            double scalingBonus =
                totalAttack
                -
                baseAttack;


            result.BaseAttack[
                damageType] =
                baseAttack;


            result.ScalingBonus[
                damageType] =
                scalingBonus;


            result.TotalAttack[
                damageType] =
                totalAttack;
        }


        result.TotalAr =
            result.TotalAttack
                .Values
                .Sum();


        result.StrengthScaling =
            GetScaling(
                upgradeStats,
                "str");


        result.DexterityScaling =
            GetScaling(
                upgradeStats,
                "dex");


        result.IntelligenceScaling =
            GetScaling(
                upgradeStats,
                "int");


        result.FaithScaling =
            GetScaling(
                upgradeStats,
                "fai");


        result.ArcaneScaling =
            GetScaling(
                upgradeStats,
                "arc");


        return result;
    }


    private static Dictionary<string, int>
        GetPlayerAttributes(
            PlayerStats stats)
    {
        return new Dictionary<string, int>
        {
            ["str"] =
                (int)decimal.Truncate(
                    stats.Strength),

            ["dex"] =
                (int)decimal.Truncate(
                    stats.Dexterity),

            ["int"] =
                (int)decimal.Truncate(
                    stats.Intelligence),

            ["fai"] =
                (int)decimal.Truncate(
                    stats.Faith),

            ["arc"] =
                (int)decimal.Truncate(
                    stats.Arcane)
        };
    }


    private static double GetScaling(
        WeaponUpgradeStats stats,
        string attribute)
    {
        return stats.AttributeScaling
            .TryGetValue(
                attribute,
                out double value)
            ? value
            : 0;
    }
}


// ============================================================
// RESULTADO DO AR
// ============================================================

public class WeaponAttackResult
{
    public Dictionary<DamageType, double>
        BaseAttack
        { get; } = new();


    public Dictionary<DamageType, double>
        ScalingBonus
        { get; } = new();


    public Dictionary<DamageType, double>
        TotalAttack
        { get; } = new();


    public double TotalAr { get; set; }


    public bool RequirementsMet { get; set; } =
        true;


    public List<string>
        IneffectiveAttributes
        { get; set; } = new();


    public double StrengthScaling { get; set; }

    public double DexterityScaling { get; set; }

    public double IntelligenceScaling { get; set; }

    public double FaithScaling { get; set; }

    public double ArcaneScaling { get; set; }
}