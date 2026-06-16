using System;
using System.IO;
using System.Linq;
using System.Text.Json;
using Roto.Core;

namespace Roto.Dumper
{
    public static class PersonalDataExtractor
    {
        public static void Extract(string resourceDirectory)
        {
            Console.WriteLine("Extracting Personal Data (Base Stats, Types, etc.)...");
            
            var table = PersonalTable.SV;
            var entries = Enumerable.Range(0, table.Count).Select(i => {
                var p = table[i];
                return new {
                    Index = i,
                    HP = p.HP,
                    ATK = p.ATK,
                    DEF = p.DEF,
                    SPA = p.SPA,
                    SPD = p.SPD,
                    SPE = p.SPE,
                    Type1 = p.Type1,
                    Type2 = p.Type2,
                    CatchRate = p.CatchRate,
                    BaseFriendship = p.BaseFriendship,
                    Gender = p.Gender,
                    BaseEXP = p.BaseEXP,
                    EvoStage = p.EvoStage
                };
            }).ToArray();
            
            var outputPath = Path.Combine(resourceDirectory, "personal_data.json");
            var json = JsonSerializer.Serialize(entries, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(outputPath, json);
        }
    }
}
