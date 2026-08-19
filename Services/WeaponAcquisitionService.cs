using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using EldenRingDamageCalculator.Models;

namespace EldenRingDamageCalculator.Services;

public class WeaponAcquisitionService
{
    private const string AcquisitionFileName =
        "weapon-acquisitions.json";


    public async Task<int> ApplyAsync(
        IEnumerable<Weapon> weapons)
    {
        string? filePath =
            FindAcquisitionFile();

        if (filePath is null)
        {
            return 0;
        }


        string json =
            await File.ReadAllTextAsync(
                filePath);


        var options =
            new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };


        List<WeaponAcquisitionRecord> records =
            JsonSerializer.Deserialize<
                List<WeaponAcquisitionRecord>>(
                    json,
                    options)
            ?? new();


        Dictionary<string, WeaponAcquisitionRecord>
            lookup =
                records
                    .Where(
                        record =>
                            !string.IsNullOrWhiteSpace(
                                record.WeaponName))
                    .GroupBy(
                        record =>
                            record.WeaponName,
                        StringComparer.OrdinalIgnoreCase)
                    .ToDictionary(
                        group =>
                            group.Key,
                        group =>
                            group.First(),
                        StringComparer.OrdinalIgnoreCase);


        int appliedCount = 0;


        foreach (Weapon weapon in weapons)
        {
            if (!lookup.TryGetValue(
                    weapon.Name,
                    out WeaponAcquisitionRecord? record))
            {
                continue;
            }


            weapon.Acquisition =
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
                        record.SourceName,

                    SourceUrl =
                        record.SourceUrl
                };


            appliedCount++;
        }


        return appliedCount;
    }


    private static string? FindAcquisitionFile()
    {
        string outputPath =
            Path.Combine(
                AppContext.BaseDirectory,
                "Data",
                AcquisitionFileName);


        if (File.Exists(outputPath))
        {
            return outputPath;
        }


        string projectPath =
            Path.Combine(
                Directory.GetCurrentDirectory(),
                "Data",
                AcquisitionFileName);


        if (File.Exists(projectPath))
        {
            return projectPath;
        }


        return null;
    }


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