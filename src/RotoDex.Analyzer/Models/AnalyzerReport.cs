using System.Collections.Generic;

namespace RotoDex.Analyzer.Models;

public class AnalyzerReport
{
    public bool IsLegal { get; set; }
    public List<AnalyzerCheckResult> OriginChecks { get; set; } = new();
    public List<AnalyzerCheckResult> EncounterChecks { get; set; } = new();
    public List<AnalyzerCheckResult> BallChecks { get; set; } = new();
    public List<AnalyzerCheckResult> MoveChecks { get; set; } = new();
    public List<AnalyzerCheckResult> RibbonChecks { get; set; } = new();
    public List<AnalyzerCheckResult> StatsChecks { get; set; } = new();
    public List<AnalyzerCheckResult> PIDChecks { get; set; } = new();
    public List<AnalyzerCheckResult> PathChecks { get; set; } = new();
    public List<AnalyzerCheckResult> MiscChecks { get; set; } = new();
}
