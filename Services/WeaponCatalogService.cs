using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using EldenRingDamageCalculator.Enums;
using EldenRingDamageCalculator.Models;

namespace EldenRingDamageCalculator.Services;

public class WeaponCatalogService
{
    private const string RegulationFileName =
        "regulation-vanilla-v1.14.js";


    // =========================
    // CARREGAR CATÁLOGO
    // =========================

    public async Task<IReadOnlyList<Weapon>> LoadWeaponsAsync()
    {
        string filePath =
            FindRegulationFile();

        string rawFile =
            await File.ReadAllTextAsync(filePath);

        string json =
            ExtractJson(rawFile);

        return ParseWeapons(json);
    }


    // =========================
    // LOCALIZAR ARQUIVO
    // =========================

    private static string FindRegulationFile()
    {
        string outputPath =
            Path.Combine(
                AppContext.BaseDirectory,
                "Data",
                RegulationFileName);

        if (File.Exists(outputPath))
        {
            return outputPath;
        }


        string projectPath =
            Path.Combine(
                Directory.GetCurrentDirectory(),
                "Data",
                RegulationFileName);

        if (File.Exists(projectPath))
        {
            return projectPath;
        }


        throw new FileNotFoundException(
            "O catálogo local de armas não foi encontrado. " +
            $"Arquivo esperado: Data/{RegulationFileName}");
    }


    // =========================
    // EXTRAIR JSON
    // =========================

    private static string ExtractJson(
        string rawFile)
    {
        int firstBrace =
            rawFile.IndexOf('{');

        int lastBrace =
            rawFile.LastIndexOf('}');


        if (firstBrace < 0 ||
            lastBrace < 0 ||
            lastBrace <= firstBrace)
        {
            throw new InvalidDataException(
                "Não foi possível encontrar os dados JSON.");
        }


        return rawFile.Substring(
            firstBrace,
            lastBrace - firstBrace + 1);
    }


    // =========================
    // PARSE PRINCIPAL
    // =========================

    private static IReadOnlyList<Weapon>
        ParseWeapons(string json)
    {
        using JsonDocument document =
            JsonDocument.Parse(json);

        JsonElement root =
            document.RootElement;


        JsonElement weaponsElement =
            GetRequiredProperty(
                root,
                "weapons");

        JsonElement reinforceTypesElement =
            GetRequiredProperty(
                root,
                "reinforceTypes");

        JsonElement calcCorrectGraphsElement =
            GetRequiredProperty(
                root,
                "calcCorrectGraphs");

        JsonElement attackElementCorrectsElement =
            GetRequiredProperty(
                root,
                "attackElementCorrects");


        Dictionary<int, double[]>
            calcCorrectGraphs =
                ParseCalcCorrectGraphs(
                    calcCorrectGraphsElement);


        Dictionary<
            int,
            Dictionary<
                DamageType,
                Dictionary<string, AttributeCorrection>>>
            attackElementCorrects =
                ParseAttackElementCorrects(
                    attackElementCorrectsElement);


        var parsedVariants =
            new List<ParsedWeaponVariant>();


        foreach (
            JsonElement weaponElement
            in weaponsElement.EnumerateArray())
        {
            ParsedWeaponVariant? variant =
                ParseWeaponVariant(
                    weaponElement,
                    reinforceTypesElement,
                    calcCorrectGraphs,
                    attackElementCorrects);

            if (variant is not null)
            {
                parsedVariants.Add(
                    variant);
            }
        }


        return BuildWeapons(
            parsedVariants);
    }


    // =========================
    // PARSE DE UMA VARIANTE
    // =========================

    private static ParsedWeaponVariant?
        ParseWeaponVariant(
            JsonElement weaponElement,
            JsonElement reinforceTypesElement,
            Dictionary<int, double[]> calcCorrectGraphs,
            Dictionary<
                int,
                Dictionary<
                    DamageType,
                    Dictionary<string, AttributeCorrection>>>
                attackElementCorrects)
    {
        string weaponName =
            weaponElement
                .GetProperty("weaponName")
                .GetString()
            ?? "";

        if (string.IsNullOrWhiteSpace(
                weaponName))
        {
            return null;
        }


        string fullName =
            weaponElement
                .GetProperty("name")
                .GetString()
            ?? weaponName;


        int affinityId =
            weaponElement
                .GetProperty("affinityId")
                .GetInt32();


        WeaponAffinity? affinity =
            ConvertAffinityId(
                affinityId);

        if (affinity is null)
        {
            return null;
        }


        int weaponType =
            weaponElement
                .GetProperty("weaponType")
                .GetInt32();


        int reinforceTypeId =
            weaponElement
                .GetProperty("reinforceTypeId")
                .GetInt32();


        int attackElementCorrectId =
            weaponElement
                .GetProperty("attackElementCorrectId")
                .GetInt32();


        bool isDlc =
            weaponElement.TryGetProperty(
                "dlc",
                out JsonElement dlcElement)
            &&
            dlcElement.ValueKind
                == JsonValueKind.True;


        Dictionary<string, int> requirements =
            ParseRequirements(
                weaponElement);


        Dictionary<DamageType, double>
            unupgradedAttack =
                ParseBaseAttack(
                    weaponElement);


        Dictionary<string, double>
            unupgradedScaling =
                ParseBaseScaling(
                    weaponElement);


        Dictionary<int, int>
            calcCorrectGraphIds =
                ParseCalcCorrectGraphIds(
                    weaponElement);


        List<WeaponUpgradeStats>
            upgradeLevels =
                BuildUpgradeLevels(
                    reinforceTypesElement,
                    reinforceTypeId,
                    unupgradedAttack,
                    unupgradedScaling);


        Dictionary<
            DamageType,
            Dictionary<string, AttributeCorrection>>
            attackElementCorrect =
                attackElementCorrects.TryGetValue(
                    attackElementCorrectId,
                    out var correction)
                ? correction
                : new();


        Dictionary<DamageType, double[]>
            scalingCurves =
                new();


        foreach (
            DamageType damageType
            in Enum.GetValues<DamageType>())
        {
            int graphId =
                calcCorrectGraphIds.TryGetValue(
                    (int)damageType,
                    out int customGraphId)
                ? customGraphId
                : 0;


            if (calcCorrectGraphs.TryGetValue(
                    graphId,
                    out double[]? graph))
            {
                scalingCurves[damageType] =
                    graph;
            }
        }


        return new ParsedWeaponVariant
        {
            WeaponName =
                weaponName,

            FullName =
                fullName,

            Affinity =
                affinity.Value,

            WeaponType =
                weaponType,

            IsDlc =
                isDlc,

            Requirements =
                requirements,

            Variant =
                new WeaponVariant
                {
                    FullName =
                        fullName,

                    Affinity =
                        affinity.Value,

                    Requirements =
                        requirements,

                    UpgradeLevels =
                        upgradeLevels,

                    AttackElementCorrect =
                        attackElementCorrect,

                    ScalingCurves =
                        scalingCurves
                }
        };
    }


    // =========================
    // UPGRADE LEVELS
    // =========================

    private static List<WeaponUpgradeStats>
        BuildUpgradeLevels(
            JsonElement reinforceTypesElement,
            int reinforceTypeId,
            Dictionary<DamageType, double>
                unupgradedAttack,
            Dictionary<string, double>
                unupgradedScaling)
    {
        string reinforceKey =
            reinforceTypeId.ToString(
                CultureInfo.InvariantCulture);


        if (!reinforceTypesElement.TryGetProperty(
                reinforceKey,
                out JsonElement reinforceLevels))
        {
            throw new InvalidDataException(
                $"ReinforceType {reinforceTypeId} não encontrado.");
        }


        var result =
            new List<WeaponUpgradeStats>();


        foreach (
            JsonElement reinforceLevel
            in reinforceLevels.EnumerateArray())
        {
            var stats =
                new WeaponUpgradeStats();


            JsonElement attackMultipliers =
                reinforceLevel.GetProperty(
                    "attack");


            foreach (
                KeyValuePair<DamageType, double>
                baseAttack
                in unupgradedAttack)
            {
                string damageKey =
                    ((int)baseAttack.Key)
                    .ToString(
                        CultureInfo.InvariantCulture);


                double multiplier =
                    GetNumberProperty(
                        attackMultipliers,
                        damageKey);


                stats.Attack[
                    baseAttack.Key] =
                    baseAttack.Value
                    *
                    multiplier;
            }


            JsonElement scalingMultipliers =
                reinforceLevel.GetProperty(
                    "attributeScaling");


            foreach (
                KeyValuePair<string, double>
                baseScaling
                in unupgradedScaling)
            {
                double multiplier =
                    GetNumberProperty(
                        scalingMultipliers,
                        baseScaling.Key);


                stats.AttributeScaling[
                    baseScaling.Key] =
                    baseScaling.Value
                    *
                    multiplier;
            }


            result.Add(stats);
        }


        return result;
    }


    // =========================
    // ATTACK BASE
    // =========================

    private static Dictionary<DamageType, double>
        ParseBaseAttack(
            JsonElement weaponElement)
    {
        var result =
            new Dictionary<DamageType, double>();


        if (!weaponElement.TryGetProperty(
                "attack",
                out JsonElement attackElement))
        {
            return result;
        }


        foreach (
            JsonElement pair
            in attackElement.EnumerateArray())
        {
            int attackType =
                pair[0].GetInt32();

            if (attackType < 0 ||
                attackType > 4)
            {
                continue;
            }


            result[
                (DamageType)attackType] =
                pair[1].GetDouble();
        }


        return result;
    }


    // =========================
    // SCALING BASE
    // =========================

    private static Dictionary<string, double>
        ParseBaseScaling(
            JsonElement weaponElement)
    {
        var result =
            new Dictionary<string, double>();


        if (!weaponElement.TryGetProperty(
                "attributeScaling",
                out JsonElement scalingElement))
        {
            return result;
        }


        foreach (
            JsonElement pair
            in scalingElement.EnumerateArray())
        {
            string attribute =
                pair[0].GetString()
                ?? "";

            if (string.IsNullOrWhiteSpace(
                    attribute))
            {
                continue;
            }


            result[attribute] =
                pair[1].GetDouble();
        }


        return result;
    }


    // =========================
    // REQUISITOS
    // =========================

    private static Dictionary<string, int>
        ParseRequirements(
            JsonElement weaponElement)
    {
        var result =
            new Dictionary<string, int>();


        if (!weaponElement.TryGetProperty(
                "requirements",
                out JsonElement requirements))
        {
            return result;
        }


        foreach (
            JsonProperty property
            in requirements.EnumerateObject())
        {
            result[property.Name] =
                property.Value.GetInt32();
        }


        return result;
    }


    // =========================
    // CALC CORRECT IDS
    // =========================

    private static Dictionary<int, int>
        ParseCalcCorrectGraphIds(
            JsonElement weaponElement)
    {
        var result =
            new Dictionary<int, int>();


        if (!weaponElement.TryGetProperty(
                "calcCorrectGraphIds",
                out JsonElement graphIds))
        {
            return result;
        }


        foreach (
            JsonProperty property
            in graphIds.EnumerateObject())
        {
            if (int.TryParse(
                    property.Name,
                    out int damageType))
            {
                result[damageType] =
                    property.Value.GetInt32();
            }
        }


        return result;
    }


    // =========================
    // CALC CORRECT GRAPHS
    // =========================

    private static Dictionary<int, double[]>
        ParseCalcCorrectGraphs(
            JsonElement element)
    {
        var result =
            new Dictionary<int, double[]>();


        foreach (
            JsonProperty graphProperty
            in element.EnumerateObject())
        {
            if (!int.TryParse(
                    graphProperty.Name,
                    out int graphId))
            {
                continue;
            }


            var stages =
                new List<CalcCorrectStage>();


            foreach (
                JsonElement stage
                in graphProperty.Value
                    .EnumerateArray())
            {
                stages.Add(
                    new CalcCorrectStage
                    {
                        MaxVal =
                            stage.GetProperty(
                                "maxVal")
                            .GetInt32(),

                        MaxGrowVal =
                            stage.GetProperty(
                                "maxGrowVal")
                            .GetDouble(),

                        AdjPt =
                            stage.GetProperty(
                                "adjPt")
                            .GetDouble()
                    });
            }


            result[graphId] =
                EvaluateCalcCorrectGraph(
                    stages);
        }


        return result;
    }


    // =========================
    // CURVA DE SCALING
    // =========================

    private static double[]
        EvaluateCalcCorrectGraph(
            List<CalcCorrectStage> stages)
    {
        var values =
            new double[149];


        for (
            int i = 1;
            i < stages.Count;
            i++)
        {
            CalcCorrectStage previous =
                stages[i - 1];

            CalcCorrectStage current =
                stages[i];


            int minAttributeValue =
                i == 1
                    ? 1
                    : previous.MaxVal + 1;


            int maxAttributeValue =
                i == stages.Count - 1
                    ? 148
                    : current.MaxVal;


            for (
                int attributeValue =
                    minAttributeValue;

                attributeValue <=
                    maxAttributeValue;

                attributeValue++)
            {
                double denominator =
                    current.MaxVal
                    -
                    previous.MaxVal;


                double ratio =
                    denominator == 0
                        ? 0
                        : (
                            attributeValue
                            -
                            previous.MaxVal)
                          /
                          denominator;


                ratio =
                    Math.Max(
                        0,
                        Math.Min(
                            1,
                            ratio));


                if (previous.AdjPt > 0)
                {
                    ratio =
                        Math.Pow(
                            ratio,
                            previous.AdjPt);
                }
                else if (
                    previous.AdjPt < 0)
                {
                    ratio =
                        1
                        -
                        Math.Pow(
                            1 - ratio,
                            -previous.AdjPt);
                }


                values[attributeValue] =
                    previous.MaxGrowVal
                    +
                    (
                        current.MaxGrowVal
                        -
                        previous.MaxGrowVal
                    )
                    *
                    ratio;
            }
        }


        return values;
    }


    // =========================
    // ATTACK ELEMENT CORRECT
    // =========================

    private static Dictionary<
        int,
        Dictionary<
            DamageType,
            Dictionary<string, AttributeCorrection>>>
        ParseAttackElementCorrects(
            JsonElement element)
    {
        var result =
            new Dictionary<
                int,
                Dictionary<
                    DamageType,
                    Dictionary<string, AttributeCorrection>>>();


        foreach (
            JsonProperty correctionProperty
            in element.EnumerateObject())
        {
            if (!int.TryParse(
                    correctionProperty.Name,
                    out int correctionId))
            {
                continue;
            }


            var damageCorrections =
                new Dictionary<
                    DamageType,
                    Dictionary<string, AttributeCorrection>>();


            foreach (
                JsonProperty damageProperty
                in correctionProperty.Value
                    .EnumerateObject())
            {
                if (!int.TryParse(
                        damageProperty.Name,
                        out int damageTypeId))
                {
                    continue;
                }


                if (damageTypeId < 0 ||
                    damageTypeId > 4)
                {
                    continue;
                }


                var attributes =
                    new Dictionary<
                        string,
                        AttributeCorrection>();


                foreach (
                    JsonProperty attributeProperty
                    in damageProperty.Value
                        .EnumerateObject())
                {
                    if (
                        attributeProperty.Value
                            .ValueKind
                        ==
                        JsonValueKind.True)
                    {
                        attributes[
                            attributeProperty.Name] =
                            new AttributeCorrection
                            {
                                UseUpgradedScaling =
                                    true
                            };
                    }
                    else if (
                        attributeProperty.Value
                            .ValueKind
                        ==
                        JsonValueKind.Number)
                    {
                        attributes[
                            attributeProperty.Name] =
                            new AttributeCorrection
                            {
                                UseUpgradedScaling =
                                    false,

                                FixedCorrection =
                                    attributeProperty.Value
                                    .GetDouble()
                            };
                    }
                }


                damageCorrections[
                    (DamageType)damageTypeId] =
                    attributes;
            }


            result[correctionId] =
                damageCorrections;
        }


        return result;
    }


    // =========================
    // CRIAR ARMAS AGRUPADAS
    // =========================

    private static IReadOnlyList<Weapon>
        BuildWeapons(
            List<ParsedWeaponVariant> variants)
    {
        var weapons =
            new List<Weapon>();


        var groups =
            variants.GroupBy(
                variant =>
                    variant.WeaponName,
                StringComparer.OrdinalIgnoreCase);


        foreach (var group in groups)
        {
            List<ParsedWeaponVariant>
                groupVariants =
                    group.ToList();


            ParsedWeaponVariant baseVariant =
                groupVariants
                    .FirstOrDefault(
                        variant =>
                            variant.Affinity
                            ==
                            WeaponAffinity.Standard)
                ??
                groupVariants.First();


            var weapon =
                new Weapon
                {
                    DataKey =
                        baseVariant.WeaponName,

                    Name =
                        baseVariant.WeaponName,

                    Category =
                        GetWeaponCategory(
                            baseVariant.WeaponType),

                    IsDlc =
                        groupVariants.Any(
                            variant =>
                                variant.IsDlc),

                    StrengthRequirement =
                        GetRequirement(
                            baseVariant.Requirements,
                            "str"),

                    DexterityRequirement =
                        GetRequirement(
                            baseVariant.Requirements,
                            "dex"),

                    IntelligenceRequirement =
                        GetRequirement(
                            baseVariant.Requirements,
                            "int"),

                    FaithRequirement =
                        GetRequirement(
                            baseVariant.Requirements,
                            "fai"),

                    ArcaneRequirement =
                        GetRequirement(
                            baseVariant.Requirements,
                            "arc")
                };


            foreach (
                ParsedWeaponVariant variant
                in groupVariants)
            {
                if (!weapon.Variants.ContainsKey(
                        variant.Affinity))
                {
                    weapon.Variants[
                        variant.Affinity] =
                        variant.Variant;
                }
            }


            weapon.AvailableAffinities =
                weapon.Variants
                    .Keys
                    .OrderBy(
                        affinity =>
                            (int)affinity)
                    .ToList();


            weapon.AllowsCustomAffinity =
                weapon.AvailableAffinities.Count
                > 1;


            weapon.MaxUpgradeLevel =
                baseVariant
                    .Variant
                    .UpgradeLevels
                    .Count
                - 1;


            weapon.UpgradeType =
                weapon.MaxUpgradeLevel switch
                {
                    25 =>
                        WeaponUpgradeType
                            .SmithingStone,

                    10 =>
                        WeaponUpgradeType
                            .SomberSmithingStone,

                    _ =>
                        WeaponUpgradeType.None
                };


            weapons.Add(weapon);
        }


        return weapons
            .OrderBy(
                weapon =>
                    weapon.Name,
                StringComparer.OrdinalIgnoreCase)
            .ToList();
    }


    // =========================
    // AFINIDADE
    // =========================

    private static WeaponAffinity?
        ConvertAffinityId(
            int affinityId)
    {
        return affinityId switch
        {
            -1 =>
                WeaponAffinity.Standard,

            0 =>
                WeaponAffinity.Standard,

            1 =>
                WeaponAffinity.Heavy,

            2 =>
                WeaponAffinity.Keen,

            3 =>
                WeaponAffinity.Quality,

            4 =>
                WeaponAffinity.Fire,

            5 =>
                WeaponAffinity.FlameArt,

            6 =>
                WeaponAffinity.Lightning,

            7 =>
                WeaponAffinity.Sacred,

            8 =>
                WeaponAffinity.Magic,

            9 =>
                WeaponAffinity.Cold,

            10 =>
                WeaponAffinity.Poison,

            11 =>
                WeaponAffinity.Blood,

            12 =>
                WeaponAffinity.Occult,

            _ =>
                null
        };
    }


    // =========================
    // CATEGORIA
    // =========================

    private static string GetWeaponCategory(
        int weaponType)
    {
        return weaponType switch
        {
            1 => "Dagger",
            3 => "Straight Sword",
            5 => "Greatsword",
            7 => "Colossal Sword",
            9 => "Curved Sword",
            11 => "Curved Greatsword",
            13 => "Katana",
            14 => "Twinblade",
            15 => "Thrusting Sword",
            16 => "Heavy Thrusting Sword",
            17 => "Axe",
            19 => "Greataxe",
            21 => "Hammer",
            23 => "Great Hammer",
            24 => "Flail",
            25 => "Spear",
            28 => "Great Spear",
            29 => "Halberd",
            31 => "Reaper",
            35 => "Fist",
            37 => "Claw",
            39 => "Whip",
            41 => "Colossal Weapon",
            50 => "Light Bow",
            51 => "Bow",
            53 => "Greatbow",
            55 => "Crossbow",
            56 => "Ballista",
            57 => "Glintstone Staff",
            59 => "Dual Catalyst",
            61 => "Sacred Seal",
            65 => "Small Shield",
            67 => "Medium Shield",
            69 => "Greatshield",
            87 => "Torch",
            88 => "Hand-to-Hand",
            89 => "Perfume Bottle",
            90 => "Thrusting Shield",
            91 => "Throwing Blade",
            92 => "Backhand Blade",
            93 => "Light Greatsword",
            94 => "Great Katana",
            95 => "Beast Claw",

            _ =>
                $"Unknown ({weaponType})"
        };
    }


    // =========================
    // HELPERS
    // =========================

    private static JsonElement
        GetRequiredProperty(
            JsonElement element,
            string propertyName)
    {
        if (!element.TryGetProperty(
                propertyName,
                out JsonElement property))
        {
            throw new InvalidDataException(
                $"Propriedade '{propertyName}' não encontrada.");
        }

        return property;
    }


    private static double GetNumberProperty(
        JsonElement element,
        string propertyName)
    {
        if (!element.TryGetProperty(
                propertyName,
                out JsonElement property))
        {
            return 0;
        }

        if (property.ValueKind
            != JsonValueKind.Number)
        {
            return 0;
        }

        return property.GetDouble();
    }


    private static int GetRequirement(
        Dictionary<string, int> requirements,
        string attribute)
    {
        return requirements.TryGetValue(
                attribute,
                out int value)
            ? value
            : 0;
    }


    // =========================
    // TIPOS INTERNOS
    // =========================

    private sealed class ParsedWeaponVariant
    {
        public string WeaponName
            { get; init; } = "";

        public string FullName
            { get; init; } = "";

        public WeaponAffinity Affinity
            { get; init; }

        public int WeaponType
            { get; init; }

        public bool IsDlc
            { get; init; }

        public Dictionary<string, int>
            Requirements
            { get; init; } = new();

        public WeaponVariant Variant
            { get; init; } = new();
    }


    private sealed class CalcCorrectStage
    {
        public int MaxVal
            { get; init; }

        public double MaxGrowVal
            { get; init; }

        public double AdjPt
            { get; init; }
    }
}