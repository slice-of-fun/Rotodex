using System.Collections.Generic;

namespace RotoDex.Analyzer.Models;

public class AnalyzerReport
{
    public bool IsLegal { get; set; }
    public List<string> OriginChecks { get; set; } = new();
    public List<string> EncounterChecks { get; set; } = new();
    public List<string> BallChecks { get; set; } = new();
    public List<string> MoveChecks { get; set; } = new();
    public List<string> RibbonChecks { get; set; } = new();
    public List<string> StatsChecks { get; set; } = new();
    public List<string> PIDChecks { get; set; } = new();
    public List<string> PathChecks { get; set; } = new();
    public List<string> MiscChecks { get; set; } = new();
}
