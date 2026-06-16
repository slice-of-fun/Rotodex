using System;
using System.Collections.Generic;
using RotoDex.Adapter;

namespace RotoDex.Core.Managers
{
    public class BatchExecutionManager
    {
        /// <summary>
        /// Executes a list of script lines (e.g. "Level=100") on the specified boxes in a save file.
        /// </summary>
        /// <returns>The number of Pokémon successfully modified.</returns>
        public int ExecuteBatch(ISaveFile saveFile, IEnumerable<string> scriptLines, IEnumerable<int> targetBoxes)
        {
            var parsedInstructions = ParseScript(scriptLines);
            if (parsedInstructions.Count == 0) return 0;

            int modifiedCount = 0;

            foreach (var boxIndex in targetBoxes)
            {
                if (boxIndex < 0 || boxIndex >= saveFile.BoxCount) continue;

                var pokemonList = saveFile.GetBox(boxIndex);
                int slotIndex = 0;

                foreach (var pokemon in pokemonList)
                {
                    if (pokemon.Species != 0) // Don't modify empty slots
                    {
                        bool modified = false;
                        foreach (var instruction in parsedInstructions)
                        {
                            if (PokemonPropertyAccessor.TrySetProperty(pokemon, instruction.Key, instruction.Value))
                            {
                                modified = true;
                            }
                        }

                        if (modified)
                        {
                            saveFile.SetBoxPokemon(boxIndex, slotIndex, pokemon);
                            modifiedCount++;
                        }
                    }
                    slotIndex++;
                }
            }

            return modifiedCount;
        }

        private Dictionary<string, string> ParseScript(IEnumerable<string> lines)
        {
            var dict = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

            foreach (var line in lines)
            {
                if (string.IsNullOrWhiteSpace(line) || line.StartsWith("//") || line.StartsWith("#"))
                    continue;

                // Support both PKHeX syntax (.Level=100) and standard (Level=100)
                var cleanLine = line.Trim();
                if (cleanLine.StartsWith("."))
                    cleanLine = cleanLine.Substring(1);

                var parts = cleanLine.Split('=', 2, StringSplitOptions.RemoveEmptyEntries);
                if (parts.Length == 2)
                {
                    dict[parts[0].Trim()] = parts[1].Trim();
                }
            }

            return dict;
        }
    }
}
