using System;
using System.Linq;
using Roto.Core;

namespace RotoDex.Adapter;

public static class GeneratorAdapter
{
    /// <summary>
    /// Attempts to brute-force generate a legal Pokémon from the given Showdown paste or species name.
    /// Returns the raw byte array of the generated legal PKM and a suggested filename.
    /// </summary>
    public static (byte[] Data, string Filename)? GenerateLegalPokemon(string showdownText)
    {
        if (string.IsNullOrWhiteSpace(showdownText))
            return null;

        var sets = ShowdownParsing.GetShowdownSets(showdownText, BattleTemplateLocalization.Default);
        var set = sets.FirstOrDefault();
        if (set == null)
            return null;

        // Default to Generation 9 Scarlet/Violet for bot generation
        var blank = EntityBlank.GetBlank((byte)9);
        blank.Version = GameVersion.SV;
        
        // Apply the requested details to set the Species, Form, etc.
        blank.ApplySetDetails(set);
        
        var tr = new SimpleTrainerInfo(GameVersion.SV);
        
        // Retrieve mathematically possible encounter templates that could result in this Pokémon
        var encounters = EncounterMovesetGenerator.GenerateEncounters(blank, tr, Array.Empty<ushort>());
        
        foreach(var enc in encounters)
        {
            if (enc is IEncounterConvertible conv)
            {
                var pk = conv.ConvertToPKM(tr);
                
                // Re-apply the user's requested ShowdownSet data (moves, EVs, nature, etc.)
                pk.ApplySetDetails(set);
                
                // If the user requested shiny but the template overrides it or it's not set
                if (set.Shiny)
                    pk.SetShiny();
                
                // Run full legality analysis on this newly minted PKM
                var la = new LegalityAnalysis(pk);
                
                if (la.Valid)
                {
                    // Found a 100% legal match! Return the data and filename.
                    var name = string.IsNullOrWhiteSpace(pk.Nickname) ? Roto.Core.SpeciesName.GetSpeciesNameGeneration(pk.Species, pk.Language, pk.Format) : pk.Nickname;
                    return (pk.Data.ToArray(), $"{name}.pk9");
                }
            }
        }
        
        // No mathematically legal combination found
        return null;
    }
}
