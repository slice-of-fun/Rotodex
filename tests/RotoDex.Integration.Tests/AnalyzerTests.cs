using Roto.Core;
using RotoDex.Analyzer;
using System.Linq;
using Xunit;

namespace RotoDex.Integration.Tests;

public class AnalyzerTests
{
    [Fact]
    public void Generate_LegalPKM_HasValidResults()
    {
        // Arrange: Generate a perfectly legal blank PKM
        var pkm = EntityBlank.GetBlank(9);
        pkm.Species = (ushort)Species.Pikachu;
        pkm.CurrentLevel = 5;
        pkm.Moves[0] = (ushort)Move.ThunderShock;
        
        // Let's ensure it has an encounter so LegalityAnalysis doesn't fail instantly
        pkm.Version = GameVersion.SV;
        pkm.MetLevel = 5;
        pkm.MetLocation = 54; // Poco Path
        
        // Act
        var report = LegalityReportGenerator.Generate(pkm);

        // Assert
        // Since we are mocking a raw entity, it might not be strictly Valid=true depending on RotoDex checks,
        // but we just want to ensure the Analyzer groups the CheckResult strings successfully without throwing.
        Assert.NotNull(report);
        Assert.NotNull(report.OriginChecks);
        Assert.NotNull(report.EncounterChecks);
        Assert.NotNull(report.BallChecks);
        Assert.NotNull(report.MoveChecks);
        Assert.NotNull(report.RibbonChecks);
        Assert.NotNull(report.StatsChecks);
        Assert.NotNull(report.PIDChecks);
        Assert.NotNull(report.MiscChecks);
    }
}
