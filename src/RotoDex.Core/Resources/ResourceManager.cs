using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using RotoDex.Core.Resources.Models;

namespace RotoDex.Core.Resources;

public static class ResourceManager
{
    private static List<EncounterModel> _encounters = new();
    private static List<LearnsetModel> _learnsets = new();
    private static List<PersonalDataModel> _personalData = new();
    private static Dictionary<int, string> _flavorTexts = new();

    public static bool IsInitialized { get; private set; }
    public static bool HasLore { get; private set; }

    public static void Initialize(string resourceDirectory)
    {
        if (IsInitialized) return;

        var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };

        var encountersPath = Path.Combine(resourceDirectory, "encounters.json");
        if (File.Exists(encountersPath))
        {
            var json = File.ReadAllText(encountersPath);
            _encounters = JsonSerializer.Deserialize<List<EncounterModel>>(json, options) ?? new();
        }

        var learnsetsPath = Path.Combine(resourceDirectory, "learnsets.json");
        if (File.Exists(learnsetsPath))
        {
            var json = File.ReadAllText(learnsetsPath);
            _learnsets = JsonSerializer.Deserialize<List<LearnsetModel>>(json, options) ?? new();
        }

        var personalPath = Path.Combine(resourceDirectory, "Core", "personal_data.json");
        if (!File.Exists(personalPath)) personalPath = Path.Combine(resourceDirectory, "personal_data.json");
        if (File.Exists(personalPath))
        {
            var json = File.ReadAllText(personalPath);
            _personalData = JsonSerializer.Deserialize<List<PersonalDataModel>>(json, options) ?? new();
        }

        // Soft-dependency for Lore
        var flavorPath = Path.Combine(resourceDirectory, "..", "Lore", "flavor_text.json");
        if (!File.Exists(flavorPath)) flavorPath = Path.Combine(resourceDirectory, "Lore", "flavor_text.json");
        
        if (File.Exists(flavorPath))
        {
            try
            {
                var json = File.ReadAllText(flavorPath);
                _flavorTexts = JsonSerializer.Deserialize<Dictionary<int, string>>(json, options) ?? new();
                HasLore = true;
            }
            catch
            {
                HasLore = false;
            }
        }

        IsInitialized = true;
    }

    public static IReadOnlyList<EncounterModel> Encounters => _encounters;
    public static IReadOnlyList<LearnsetModel> Learnsets => _learnsets;
    public static IReadOnlyList<PersonalDataModel> PersonalData => _personalData;

    public static PersonalDataModel? GetPersonalData(ushort species, byte form)
    {
        return _personalData.FirstOrDefault(p => p.Species == species && p.Form == form);
    }

    public static LearnsetModel? GetLearnset(int index)
    {
        return _learnsets.FirstOrDefault(l => l.Index == index);
    }

    public static string? GetFlavorText(ushort speciesId)
    {
        if (!HasLore) return null;
        return _flavorTexts.TryGetValue(speciesId, out var text) ? text : null;
    }
}
