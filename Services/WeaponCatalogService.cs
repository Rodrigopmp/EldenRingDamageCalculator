using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using EldenRingDamageCalculator.Enums;
using EldenRingDamageCalculator.Models;

namespace EldenRingDamageCalculator.Services;

public class WeaponCatalogService
{
    private const string ErdbArmamentsUrl =
        "https://api.erdb.wiki/v1/latest/armaments/";

    private static readonly HttpClient HttpClient = new()
    {
        Timeout = TimeSpan.FromSeconds(20)
    };

    private readonly string _cacheFilePath;

    public WeaponCatalogService()
    {
        string applicationDataPath =
            Environment.GetFolderPath(
                Environment.SpecialFolder.LocalApplicationData);

        string cacheDirectory = Path.Combine(
            applicationDataPath,
            "EldenRingDamageCalculator",
            "Cache");

        _cacheFilePath = Path.Combine(
            cacheDirectory,
            "armaments.json");
    }

    public async Task<IReadOnlyList<Weapon>> LoadWeaponsAsync()
    {
        string json = await LoadJsonAsync();

        return ParseWeapons(json);
    }

    private async Task<string> LoadJsonAsync()
    {
        try
        {
            string json =
                await HttpClient.GetStringAsync(ErdbArmamentsUrl);

            string? directory =
                Path.GetDirectoryName(_cacheFilePath);

            if (!string.IsNullOrWhiteSpace(directory))
            {
                Directory.CreateDirectory(directory);
            }

            await File.WriteAllTextAsync(
                _cacheFilePath,
                json);

            return json;
        }
        catch
        {
            if (File.Exists(_cacheFilePath))
            {
                return await File.ReadAllTextAsync(
                    _cacheFilePath);
            }

            throw;
        }
    }

    private static IReadOnlyList<Weapon> ParseWeapons(
        string json)
    {
        using JsonDocument document =
            JsonDocument.Parse(json);

        var weapons = new List<Weapon>();

        foreach (
            JsonProperty weaponProperty
            in document.RootElement.EnumerateObject())
        {
            JsonElement weaponData =
                weaponProperty.Value;

            if (!weaponData.TryGetProperty(
                    "name",
                    out JsonElement nameElement))
            {
                continue;
            }

            string? name = nameElement.GetString();

            if (string.IsNullOrWhiteSpace(name))
            {
                continue;
            }

            string category = "";

            if (weaponData.TryGetProperty(
                    "category",
                    out JsonElement categoryElement))
            {
                category =
                    categoryElement.GetString() ?? "";
            }

            bool allowsCustomAffinity = false;

            if (weaponData.TryGetProperty(
                    "allow_ash_of_war",
                    out JsonElement ashElement))
            {
                allowsCustomAffinity =
                    ashElement.GetBoolean();
            }

            int maxUpgradeLevel = 0;

            if (weaponData.TryGetProperty(
                    "upgrade_costs",
                    out JsonElement upgradeCostsElement)
                &&
                upgradeCostsElement.ValueKind
                    == JsonValueKind.Array)
            {
                maxUpgradeLevel =
                    upgradeCostsElement.GetArrayLength();
            }

            WeaponUpgradeType upgradeType =
                maxUpgradeLevel switch
                {
                    10 =>
                        WeaponUpgradeType
                            .SomberSmithingStone,

                    25 =>
                        WeaponUpgradeType
                            .SmithingStone,

                    _ =>
                        WeaponUpgradeType.None
                };

            weapons.Add(new Weapon
            {
                DataKey = weaponProperty.Name,

                Name = name,

                Category = category,

                UpgradeType = upgradeType,

                MaxUpgradeLevel =
                    maxUpgradeLevel,

                AllowsCustomAffinity =
                    allowsCustomAffinity
            });
        }

        return weapons
            .OrderBy(
                weapon => weapon.Name,
                StringComparer.OrdinalIgnoreCase)
            .ToList();
    }
}