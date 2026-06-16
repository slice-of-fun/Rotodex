using System;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Reflection;
using Roto.Core;

namespace Roto.Dumper
{
    public static class LearnsetExtractor
    {
        public static void Extract(string resourceDirectory)
        {
            Console.WriteLine("Extracting Learnsets...");
            
            var field = typeof(LearnSource9SV).GetField("Learnsets", BindingFlags.NonPublic | BindingFlags.Static);
            var learnsets = (Learnset[])field!.GetValue(null)!;
            
            var entries = learnsets.Select((l, i) => new {
                Index = i,
                Moves = l?.GetAllMoves().ToArray() ?? Array.Empty<ushort>(),
                Levels = l?.GetAllLevels().ToArray() ?? Array.Empty<byte>()
            }).ToArray();
            
            var outputPath = Path.Combine(resourceDirectory, "learnsets.json");
            var json = JsonSerializer.Serialize(entries, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(outputPath, json);
        }
    }
}
