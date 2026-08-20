using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using EldenRingDamageCalculator.Models;

namespace EldenRingDamageCalculator.Services;

public class WeaponAcquisitionService
{
    private const string AcquisitionJsonFileName =
        "weapon-acquisitions.json";

    private const string DlcAcquisitionJsonFileName =
        "dlc-weapon-acquisitions.json";

    private const string WeaponLocationsCsvFileName =
        "weapons-locations.csv";

    private const string MissingReportFileName =
        "missing-weapon-acquisitions.txt";


    // ============================================================
    // ALIASES / ERROS DE GRAFIA DO DATASET
    // ============================================================

    private static readonly Dictionary<string, string>
        CsvNameAliases =
            new(StringComparer.OrdinalIgnoreCase)
            {
                ["crepussblackkeycrossbow"] =
                    "crepusblackkeycrossbow",

                ["zamorcurvedsword"] =
                    "zamorcuvedsword",

                ["carianglintstonestaff"] =
                    "carianglinstonestaff",

                ["crescentmoonaxe"] =
                    "cresentmoonaxe",

                ["guardiansswordspear"] =
                    "gaurdiansswordspear",

                ["frenziedflameseal"] =
                    "freziedflameseal"
            };


    // ============================================================
    // APLICAR AQUISIÇÕES
    // ============================================================

    public async Task<int> ApplyAsync(
        IEnumerable<Weapon> weapons)
    {
        List<Weapon> weaponList =
            weapons.ToList();


        var acquisitions =
            new Dictionary<string, WeaponAcquisition>(
                StringComparer.OrdinalIgnoreCase);


        // ========================================================
        // 1. CSV DO BASE GAME
        // ========================================================

        string? csvPath =
            FindDataFile(
                WeaponLocationsCsvFileName);


        if (csvPath is not null)
        {
            Dictionary<string, WeaponAcquisition>
                csvAcquisitions =
                    await LoadCsvAsync(
                        csvPath);


            foreach (
                KeyValuePair<string, WeaponAcquisition>
                entry
                in csvAcquisitions)
            {
                acquisitions[entry.Key] =
                    entry.Value;
            }
        }


        // ========================================================
        // 2. JSON DA DLC
        //
        // SOBRESCREVE O CSV QUANDO HOUVER O MESMO NOME.
        // ========================================================

        string? dlcJsonPath =
            FindDataFile(
                DlcAcquisitionJsonFileName);


        if (dlcJsonPath is not null)
        {
            Dictionary<string, WeaponAcquisition>
                dlcAcquisitions =
                    await LoadJsonAsync(
                        dlcJsonPath);


            foreach (
                KeyValuePair<string, WeaponAcquisition>
                entry
                in dlcAcquisitions)
            {
                acquisitions[entry.Key] =
                    entry.Value;
            }
        }


        // ========================================================
        // 3. NOSSO JSON MANUAL / OVERRIDES
        //
        // TEM A MAIOR PRIORIDADE.
        // ========================================================

        string? jsonPath =
            FindDataFile(
                AcquisitionJsonFileName);


        if (jsonPath is not null)
        {
            Dictionary<string, WeaponAcquisition>
                jsonAcquisitions =
                    await LoadJsonAsync(
                        jsonPath);


            foreach (
                KeyValuePair<string, WeaponAcquisition>
                entry
                in jsonAcquisitions)
            {
                acquisitions[entry.Key] =
                    entry.Value;
            }
        }


        // ========================================================
        // 4. APLICAR NO CATÁLOGO
        // ========================================================

        int appliedCount = 0;


        foreach (Weapon weapon in weaponList)
        {
            string normalizedName =
                NormalizeWeaponName(
                    weapon.Name);


            if (normalizedName == "unarmed")
            {
                weapon.Acquisition =
                    CreateUnarmedAcquisition();

                appliedCount++;

                continue;
            }


            string lookupName =
                ResolveCsvAlias(
                    normalizedName);


            if (!acquisitions.TryGetValue(
                    lookupName,
                    out WeaponAcquisition?
                        acquisition))
            {
                continue;
            }


            weapon.Acquisition =
                acquisition;


            appliedCount++;
        }


        return appliedCount;
    }


    // ============================================================
    // UNARMED
    // ============================================================

    private static WeaponAcquisition
        CreateUnarmedAcquisition()
    {
        return new WeaponAcquisition
        {
            Region =
                "Sempre disponível",

            Location =
                "Desequipe a arma do slot ativo",

            AcquisitionType =
                "Estado sem arma",

            Description =
                "Unarmed não é um item coletável. "
                +
                "Para ficar desarmado, deixe o slot de arma vazio.",

            SourceName =
                "Fextralife",

            SourceUrl =
                "https://eldenring.wiki.fextralife.com/Kick"
        };
    }


    // ============================================================
    // ALIAS
    // ============================================================

    private static string ResolveCsvAlias(
        string normalizedName)
    {
        if (CsvNameAliases.TryGetValue(
                normalizedName,
                out string? alias))
        {
            return alias;
        }


        return normalizedName;
    }


    // ============================================================
    // GERAR RELATÓRIO DE FALTANTES
    // ============================================================

    public async Task<IReadOnlyList<Weapon>>
        GenerateMissingReportAsync(
            IEnumerable<Weapon> weapons)
    {
        List<Weapon> missingWeapons =
            weapons
                .Where(
                    weapon =>
                        weapon.Acquisition is null)
                .OrderBy(
                    weapon =>
                        weapon.IsDlc
                            ? 1
                            : 0)
                .ThenBy(
                    weapon =>
                        weapon.Category,
                    StringComparer.OrdinalIgnoreCase)
                .ThenBy(
                    weapon =>
                        weapon.Name,
                    StringComparer.OrdinalIgnoreCase)
                .ToList();


        string dataDirectory =
            FindWritableDataDirectory();


        string reportPath =
            Path.Combine(
                dataDirectory,
                MissingReportFileName);


        var report =
            new StringBuilder();


        report.AppendLine(
            "ELDEN RING DAMAGE CALCULATOR");

        report.AppendLine(
            "Missing Weapon Acquisitions");

        report.AppendLine(
            "========================================");

        report.AppendLine();


        report.AppendLine(
            $"Total de armas sem dados de obtenção: {missingWeapons.Count}");

        report.AppendLine();


        int baseGameCount =
            missingWeapons.Count(
                weapon =>
                    !weapon.IsDlc);

        int dlcCount =
            missingWeapons.Count(
                weapon =>
                    weapon.IsDlc);


        report.AppendLine(
            $"Base Game: {baseGameCount}");

        report.AppendLine(
            $"Shadow of the Erdtree: {dlcCount}");

        report.AppendLine();


        report.AppendLine(
            "========================================");

        report.AppendLine();


        report.AppendLine(
            "BASE GAME");

        report.AppendLine(
            "========================================");

        report.AppendLine();


        List<Weapon> baseGameWeapons =
            missingWeapons
                .Where(
                    weapon =>
                        !weapon.IsDlc)
                .ToList();


        if (baseGameWeapons.Count == 0)
        {
            report.AppendLine(
                "Nenhuma arma faltando.");
        }
        else
        {
            AppendWeaponsToReport(
                report,
                baseGameWeapons);
        }


        report.AppendLine();

        report.AppendLine(
            "SHADOW OF THE ERDTREE");

        report.AppendLine(
            "========================================");

        report.AppendLine();


        List<Weapon> dlcWeapons =
            missingWeapons
                .Where(
                    weapon =>
                        weapon.IsDlc)
                .ToList();


        if (dlcWeapons.Count == 0)
        {
            report.AppendLine(
                "Nenhuma arma faltando.");
        }
        else
        {
            AppendWeaponsToReport(
                report,
                dlcWeapons);
        }


        report.AppendLine();

        report.AppendLine(
            "========================================");

        report.AppendLine(
            "LISTA SIMPLES");

        report.AppendLine(
            "========================================");

        report.AppendLine();


        for (
            int i = 0;
            i < missingWeapons.Count;
            i++)
        {
            report.AppendLine(
                $"{i + 1}. {missingWeapons[i].Name}");
        }


        await File.WriteAllTextAsync(
            reportPath,
            report.ToString(),
            Encoding.UTF8);


        return missingWeapons;
    }


    // ============================================================
    // ESCREVER ARMAS NO RELATÓRIO
    // ============================================================

    private static void AppendWeaponsToReport(
        StringBuilder report,
        List<Weapon> weapons)
    {
        string? currentCategory =
            null;


        foreach (Weapon weapon in weapons)
        {
            if (!string.Equals(
                    currentCategory,
                    weapon.Category,
                    StringComparison.OrdinalIgnoreCase))
            {
                currentCategory =
                    weapon.Category;


                report.AppendLine();

                report.AppendLine(
                    $"[{currentCategory}]");

                report.AppendLine();
            }


            report.AppendLine(
                $"- {weapon.Name}");
        }
    }


    // ============================================================
    // JSON
    // ============================================================

    private static async Task<
        Dictionary<string, WeaponAcquisition>>
        LoadJsonAsync(
            string filePath)
    {
        string json =
            await File.ReadAllTextAsync(
                filePath);


        var options =
            new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive =
                    true
            };


        List<WeaponAcquisitionRecord>
            records =
                JsonSerializer.Deserialize<
                    List<WeaponAcquisitionRecord>>(
                        json,
                        options)
                ??
                new();


        var result =
            new Dictionary<string, WeaponAcquisition>(
                StringComparer.OrdinalIgnoreCase);


        foreach (
            WeaponAcquisitionRecord record
            in records)
        {
            if (string.IsNullOrWhiteSpace(
                    record.WeaponName))
            {
                continue;
            }


            string normalizedName =
                NormalizeWeaponName(
                    record.WeaponName);


            result[normalizedName] =
                new WeaponAcquisition
                {
                    Region =
                        record.Region,

                    Location =
                        record.Location,

                    AcquisitionType =
                        record.AcquisitionType,

                    Description =
                        record.Description,

                    SourceName =
                        string.IsNullOrWhiteSpace(
                            record.SourceName)
                            ? "Fextralife"
                            : record.SourceName,

                    SourceUrl =
                        record.SourceUrl
                };
        }


        return result;
    }


    // ============================================================
    // CSV
    // ============================================================

    private static async Task<
        Dictionary<string, WeaponAcquisition>>
        LoadCsvAsync(
            string filePath)
    {
        string[] lines =
            await File.ReadAllLinesAsync(
                filePath);


        var result =
            new Dictionary<string, WeaponAcquisition>(
                StringComparer.OrdinalIgnoreCase);


        foreach (string line in lines)
        {
            if (string.IsNullOrWhiteSpace(
                    line))
            {
                continue;
            }


            List<string> columns =
                ParseCsvLine(
                    line);


            TryAddCsvWeapon(
                result,
                columns,
                0,
                1,
                2);


            TryAddCsvWeapon(
                result,
                columns,
                4,
                5,
                6);
        }


        return result;
    }


    // ============================================================
    // ADICIONAR ENTRADA DO CSV
    // ============================================================

    private static void TryAddCsvWeapon(
        Dictionary<string, WeaponAcquisition>
            result,
        List<string> columns,
        int weaponNameIndex,
        int locationIndex,
        int collectedIndex)
    {
        if (columns.Count
            <= collectedIndex)
        {
            return;
        }


        string weaponName =
            columns[weaponNameIndex]
                .Trim();


        string location =
            columns[locationIndex]
                .Trim();


        string collected =
            columns[collectedIndex]
                .Trim();


        if (!string.Equals(
                collected,
                "FALSE",
                StringComparison.OrdinalIgnoreCase))
        {
            return;
        }


        if (string.IsNullOrWhiteSpace(
                weaponName)
            ||
            string.IsNullOrWhiteSpace(
                location))
        {
            return;
        }


        weaponName =
            CleanWeaponName(
                weaponName);


        if (string.IsNullOrWhiteSpace(
                weaponName))
        {
            return;
        }


        string normalizedName =
            NormalizeWeaponName(
                weaponName);


        result[normalizedName] =
            CreateAcquisitionFromCsvLocation(
                location);
    }


    // ============================================================
    // CONVERTER LOCALIZAÇÃO DO CSV
    // ============================================================

    private static WeaponAcquisition
        CreateAcquisitionFromCsvLocation(
            string rawLocation)
    {
        List<string> acquisitionTypes =
            GetAcquisitionTypes(
                rawLocation);


        string cleanLocation =
            CleanLocation(
                rawLocation);


        string acquisitionType =
            acquisitionTypes.Count > 0
                ? string.Join(
                    " / ",
                    acquisitionTypes)
                : "Obtenção";


        return new WeaponAcquisition
        {
            Region =
                "",

            Location =
                cleanLocation,

            AcquisitionType =
                acquisitionType,

            Description =
                BuildDescription(
                    cleanLocation,
                    acquisitionTypes),

            SourceName =
                "Fextralife / Elden Ring Index",

            SourceUrl =
                "https://eldenring.wiki.fextralife.com/Weapons"
        };
    }


    // ============================================================
    // DESCRIÇÃO
    // ============================================================

    private static string BuildDescription(
        string cleanLocation,
        List<string> acquisitionTypes)
    {
        if (acquisitionTypes.Count == 0)
        {
            return
                $"Obtida em: {cleanLocation}.";
        }


        if (acquisitionTypes.Count > 1)
        {
            return
                $"Possui múltiplas formas de obtenção: {cleanLocation}.";
        }


        return acquisitionTypes[0] switch
        {
            "Item no mundo" =>
                $"Encontrada no mundo em: {cleanLocation}.",

            "Drop de inimigo" =>
                $"Pode ser obtida como drop em: {cleanLocation}.",

            "Boss / Invasor" =>
                $"Obtida ao derrotar: {cleanLocation}.",

            "Loja" =>
                $"Pode ser comprada em: {cleanLocation}.",

            "Quest" =>
                $"Obtida através de: {cleanLocation}.",

            "Remembrance" =>
                $"Obtida através de: {cleanLocation}.",

            _ =>
                $"Obtida em: {cleanLocation}."
        };
    }


    // ============================================================
    // TIPO DE ACQUISITION
    // ============================================================

    private static List<string>
        GetAcquisitionTypes(
            string location)
    {
        var types =
            new List<string>();


        if (location.Contains("🌱"))
        {
            types.Add(
                "Item no mundo");
        }


        if (location.Contains("🦴"))
        {
            types.Add(
                "Drop de inimigo");
        }


        if (location.Contains("💀"))
        {
            types.Add(
                "Boss / Invasor");
        }


        if (location.Contains("💰"))
        {
            types.Add(
                "Loja");
        }


        if (location.Contains("📜"))
        {
            types.Add(
                "Quest");
        }


        if (location.Contains("💎"))
        {
            types.Add(
                "Remembrance");
        }


        return types;
    }


    // ============================================================
    // LIMPAR LOCALIZAÇÃO
    // ============================================================

    private static string CleanLocation(
        string location)
    {
        return location
            .Replace("🌱", "")
            .Replace("🦴", "")
            .Replace("💀", "")
            .Replace("💰", "")
            .Replace("📜", "")
            .Replace("💎", "")
            .Trim();
    }


    // ============================================================
    // LIMPAR NOME
    // ============================================================

    private static string CleanWeaponName(
        string weaponName)
    {
        weaponName =
            weaponName.Trim();


        while (
            weaponName.EndsWith(
                "*",
                StringComparison.Ordinal))
        {
            weaponName =
                weaponName[..^1]
                    .TrimEnd();
        }


        return weaponName;
    }


    // ============================================================
    // NORMALIZAÇÃO ROBUSTA
    // ============================================================

    private static string NormalizeWeaponName(
        string weaponName)
    {
        string cleaned =
            CleanWeaponName(
                weaponName);


        cleaned =
            cleaned
                .Replace('’', '\'')
                .Replace('‘', '\'');


        string decomposed =
            cleaned.Normalize(
                NormalizationForm.FormD);


        var builder =
            new StringBuilder();


        foreach (char character in decomposed)
        {
            UnicodeCategory category =
                CharUnicodeInfo.GetUnicodeCategory(
                    character);


            if (category ==
                UnicodeCategory.NonSpacingMark)
            {
                continue;
            }


            if (char.IsLetterOrDigit(
                    character))
            {
                builder.Append(
                    char.ToLowerInvariant(
                        character));
            }
        }


        return builder.ToString();
    }


    // ============================================================
    // PARSER CSV
    // ============================================================

    private static List<string>
        ParseCsvLine(
            string line)
    {
        var result =
            new List<string>();


        var current =
            new StringBuilder();


        bool insideQuotes =
            false;


        for (
            int i = 0;
            i < line.Length;
            i++)
        {
            char character =
                line[i];


            if (character == '"')
            {
                if (insideQuotes
                    &&
                    i + 1 < line.Length
                    &&
                    line[i + 1] == '"')
                {
                    current.Append(
                        '"');

                    i++;

                    continue;
                }


                insideQuotes =
                    !insideQuotes;

                continue;
            }


            if (character == ','
                &&
                !insideQuotes)
            {
                result.Add(
                    current.ToString());

                current.Clear();

                continue;
            }


            current.Append(
                character);
        }


        result.Add(
            current.ToString());


        return result;
    }


    // ============================================================
    // LOCALIZAR DATA
    // ============================================================

    private static string? FindDataFile(
        string fileName)
    {
        string projectPath =
            Path.Combine(
                Directory.GetCurrentDirectory(),
                "Data",
                fileName);


        if (File.Exists(
                projectPath))
        {
            return projectPath;
        }


        string outputPath =
            Path.Combine(
                AppContext.BaseDirectory,
                "Data",
                fileName);


        if (File.Exists(
                outputPath))
        {
            return outputPath;
        }


        return null;
    }


    // ============================================================
    // DIRETÓRIO DO RELATÓRIO
    // ============================================================

    private static string
        FindWritableDataDirectory()
    {
        string projectDataDirectory =
            Path.Combine(
                Directory.GetCurrentDirectory(),
                "Data");


        if (Directory.Exists(
                projectDataDirectory))
        {
            return projectDataDirectory;
        }


        string outputDataDirectory =
            Path.Combine(
                AppContext.BaseDirectory,
                "Data");


        Directory.CreateDirectory(
            outputDataDirectory);


        return outputDataDirectory;
    }


    // ============================================================
    // FORMATO DO JSON
    // ============================================================

    private sealed class WeaponAcquisitionRecord
    {
        public string WeaponName
            { get; set; } = "";

        public string Region
            { get; set; } = "";

        public string Location
            { get; set; } = "";

        public string AcquisitionType
            { get; set; } = "";

        public string Description
            { get; set; } = "";

        public string SourceName
            { get; set; } = "";

        public string SourceUrl
            { get; set; } = "";
    }
}