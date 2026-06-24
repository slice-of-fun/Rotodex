using System;
using System.Collections.Generic;
using Roto.Core;

namespace RotoDex.Adapter;

public static class TeamAdapter
{
    /// <summary>
    /// Parses a standard Pokémon Showdown paste and returns a collection of IPokemon (Gen 9 defaults).
    /// </summary>
    /// <param name="paste">The raw Showdown paste text.</param>
    /// <returns>Collection of IPokemon wrapping the parsed PKMs.</returns>
    public static IEnumerable<IPokemon> ParseShowdown(string paste)
    {
        if (string.IsNullOrWhiteSpace(paste))
            yield break;

        var sets = ShowdownParsing.GetShowdownSets(paste, BattleTemplateLocalization.Default);
        foreach (var set in sets)
        {
            // Generate a blank Gen 9 Pokémon
            var blank = EntityBlank.GetBlank((byte)9);
            blank.Version = GameVersion.SV;
            
            // Apply the Showdown set to the blank
            blank.ApplySetDetails(set);
            
            yield return new PokemonAdapter(blank);
        }
    }
}
