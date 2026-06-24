using System;
using System.IO;
using System.Collections.Generic;
using System.Text.Json;
using System.Linq;

namespace Lore.Dumper
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Starting Lore.Dumper...");
            string upstreamDir = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "../../../../../upstream_lore/data/v2/csv"));
            string outputDir = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "../../../../../resources/Lore"));
            
            if (!Directory.Exists(outputDir))
            {
                Directory.CreateDirectory(outputDir);
            }

            string flavorTextPath = Path.Combine(upstreamDir, "pokemon_species_flavor_text.csv");
            if (!File.Exists(flavorTextPath))
            {
                Console.WriteLine($"Error: {flavorTextPath} not found.");
                return;
            }

            var flavorTexts = new Dictionary<int, string>();
            
            // Simple CSV parsing, handling quoted newlines
            using (var reader = new StreamReader(flavorTextPath))
            {
                string header = reader.ReadLine();
                string line;
                while ((line = ReadCsvLine(reader)) != null)
                {
                    var cols = ParseCsvLine(line);
                    if (cols.Count >= 4)
                    {
                        if (int.TryParse(cols[0], out int speciesId) && int.TryParse(cols[2], out int languageId))
                        {
                            if (languageId == 9) // English
                            {
                                // Overwrite with the latest version's flavor text
                                string text = cols[3].Replace("\n", " ").Replace("\r", "").Replace("\f", " ").Replace("\u000c", " ");
                                flavorTexts[speciesId] = text;
                            }
                        }
                    }
                }
            }

            string outputPath = Path.Combine(outputDir, "flavor_text.json");
            File.WriteAllText(outputPath, JsonSerializer.Serialize(flavorTexts, new JsonSerializerOptions { WriteIndented = true }));
            Console.WriteLine($"Dumped flavor text for {flavorTexts.Count} species to {outputPath}");
            
            // Create a dummy link.json for now to satisfy the soft dependency
            string linkPath = Path.Combine(outputDir, "link.json");
            File.WriteAllText(linkPath, "{}");
            Console.WriteLine($"Created dummy link.json at {linkPath}");
        }

        static string ReadCsvLine(StreamReader reader)
        {
            if (reader.EndOfStream) return null;
            string line = reader.ReadLine();
            while (line.Count(c => c == '"') % 2 != 0 && !reader.EndOfStream)
            {
                line += "\n" + reader.ReadLine();
            }
            return line;
        }

        static List<string> ParseCsvLine(string line)
        {
            var result = new List<string>();
            bool inQuotes = false;
            string current = "";
            for (int i = 0; i < line.Length; i++)
            {
                if (line[i] == '"')
                {
                    inQuotes = !inQuotes;
                }
                else if (line[i] == ',' && !inQuotes)
                {
                    result.Add(current);
                    current = "";
                }
                else
                {
                    current += line[i];
                }
            }
            result.Add(current);
            return result;
        }
    }
}
