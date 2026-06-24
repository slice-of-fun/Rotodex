using System;
using Roto.Core;
using RotoDex.Analyzer.Models;

namespace RotoDex.Analyzer;

public static class LegalityReportGenerator
{
    public static AnalyzerReport Generate(PKM pkm, string language = "en")
    {
        var analysis = new LegalityAnalysis(pkm);
        var context = LegalityLocalizationContext.Create(analysis, language);

        var report = new AnalyzerReport { IsLegal = analysis.Valid };

        foreach (var result in analysis.Results)
        {
            var message = context.Humanize(result, verbose: false);
            var cr = new AnalyzerCheckResult { Valid = result.Valid, Message = message };

            switch (result.Identifier)
            {
                case CheckIdentifier.GameOrigin:
                case CheckIdentifier.Evolution:
                case CheckIdentifier.Geography:
                    report.OriginChecks.Add(cr);
                    break;
                
                case CheckIdentifier.Encounter:
                case CheckIdentifier.Fateful:
                case CheckIdentifier.Egg:
                    report.EncounterChecks.Add(cr);
                    break;
                
                case CheckIdentifier.Ball:
                    report.BallChecks.Add(cr);
                    break;
                
                case CheckIdentifier.CurrentMove:
                case CheckIdentifier.RelearnMove:
                    report.MoveChecks.Add(cr);
                    break;
                
                case CheckIdentifier.Ribbon:
                case CheckIdentifier.RibbonMark:
                case CheckIdentifier.Marking:
                    report.RibbonChecks.Add(cr);
                    break;
                
                case CheckIdentifier.EVs:
                case CheckIdentifier.IVs:
                case CheckIdentifier.AVs:
                case CheckIdentifier.GVs:
                case CheckIdentifier.Level:
                case CheckIdentifier.Nature:
                case CheckIdentifier.Ability:
                case CheckIdentifier.Form:
                case CheckIdentifier.Gender:
                    report.StatsChecks.Add(cr);
                    break;

                case CheckIdentifier.PID:
                case CheckIdentifier.EC:
                case CheckIdentifier.Shiny:
                    report.PIDChecks.Add(cr);
                    break;
                
                default:
                    report.MiscChecks.Add(cr);
                    break;
            }
        }

        GeneratePathHistory(pkm, report);

        return report;
    }

    private static void GeneratePathHistory(PKM pkm, AnalyzerReport report)
    {
        int originGen = pkm.Generation;
        int currentGen = pkm.Format;
        
        if (originGen == currentGen)
        {
            report.PathChecks.Add(new AnalyzerCheckResult { Valid = true, Message = $"Caught in Gen {originGen} ({GameInfo.GetVersionName(pkm.Version)})." });
            return;
        }

        var path = new System.Collections.Generic.List<string>();
        path.Add($"Gen {originGen} ({GameInfo.GetVersionName(pkm.Version)})");

        if (originGen <= 3 && currentGen >= 4) path.Add("Pal Park");
        if (originGen <= 4 && currentGen >= 5) path.Add("Poké Transfer");
        if (originGen <= 5 && currentGen >= 6) path.Add("Poké Transporter");
        if (originGen <= 7 && currentGen >= 6) path.Add("Pokémon Bank");
        if (currentGen >= 8 && originGen < currentGen) path.Add("Pokémon HOME");
        
        path.Add($"Gen {currentGen}");
        
        report.PathChecks.Add(new AnalyzerCheckResult { Valid = true, Message = string.Join(" → ", path) });

        // Validate HOME Tracker if moved to Gen 8+
        if (currentGen >= 8 && originGen < currentGen && pkm is Roto.Core.IHomeTrack homeTrack)
        {
            if (!homeTrack.HasTracker)
            {
                report.PathChecks.Add(new AnalyzerCheckResult { Valid = false, Message = "Invalid Transfer: Missing HOME Tracker for cross-generation migration." });
                report.IsLegal = false;
            }
        }
    }
}
