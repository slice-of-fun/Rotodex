using System;
using RotoDex.Analyzer;
using RotoDex.Analyzer.Models;

namespace RotoDex.Adapter
{
    public static class LegalityWrapper
    {
        public static AnalyzerReport CheckLegality(IPokemon pokemon)
        {
            if (pokemon is PokemonAdapter adapter)
            {
                var rawPkm = adapter.GetUnderlyingPkm();
                return LegalityReportGenerator.Generate(rawPkm);
            }
            throw new ArgumentException("Unsupported Pokemon model.");
        }
    }
}
