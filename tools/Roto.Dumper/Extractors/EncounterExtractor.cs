using System;
using System.IO;
using System.Linq;
using System.Text.Json;
using Roto.Core;

namespace Roto.Dumper
{
    public static class EncounterExtractor
    {
        public static void Extract(string resourceDirectory)
        {
            Console.WriteLine("Extracting Encounter Tables...");
            
            var events = EncounterEvent.GetAllEvents(sorted: true).Select(e => new {
                Species = e.Species,
                Form = e.Form,
                Level = e.Level,
                OT_Name = e.OriginalTrainerName,
                TID = e.TID16,
                SID = e.SID16,
                Ball = e.Ball,
                IsShiny = e.IsShiny,
                Moves = new ushort[] { e.Moves.Move1, e.Moves.Move2, e.Moves.Move3, e.Moves.Move4 }
            }).ToArray();
            
            var outputPath = Path.Combine(resourceDirectory, "encounters.json");
            var json = JsonSerializer.Serialize(events, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(outputPath, json);
        }
    }
}
