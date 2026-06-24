using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Roto.Core;
using RotoDex.Analyzer.Models;

namespace RotoDex.Web.Services;

public class AiService
{
    private readonly HttpClient _http;

    public AiService(HttpClient http)
    {
        _http = http;
        // _apiKey is no longer needed on the client, the Python backend holds it securely!
    }

    public async Task<string> ExplainLegalityAsync(PKM pkm, AnalyzerCheckResult check)
    {
        var speciesName = ((Species)pkm.Species).ToString();
        var isLegal = check.Valid;
        var checkMsg = check.Message;

        var prompt = $@"
You are a Pokémon legality expert. I am analyzing a {speciesName}.
The legality checker returned the following result for this Pokémon:
Status: {(isLegal ? "Valid" : "Invalid")}
Message: {checkMsg}

Explain this legality check in 1-2 short, simple sentences. Don't use overly technical hexadecimal terms if possible. Make it easy for a casual player to understand why this is {(isLegal ? "legal" : "illegal")}.
";

        var requestBody = new
        {
            speciesName = speciesName,
            isLegal = isLegal,
            checkMsg = checkMsg
        };

        var request = new HttpRequestMessage(HttpMethod.Post, "http://127.0.0.1:8000/api/ai/explain");
        request.Content = JsonContent.Create(requestBody);

        try
        {
            var response = await _http.SendAsync(request);
            if (!response.IsSuccessStatusCode)
            {
                return "Failed to get an explanation from the AI.";
            }

            var json = await response.Content.ReadAsStringAsync();
            using var doc = JsonDocument.Parse(json);
            return doc.RootElement.GetProperty("content").GetString() ?? "No explanation generated.";
        }
        catch
        {
            return "Could not reach the AI Python backend. Is it running?";
        }
    }

    public async Task<string> SuggestTeamImprovementsAsync(string showdownExport)
    {
        var requestBody = new
        {
            showdownExport = showdownExport
        };

        var request = new HttpRequestMessage(HttpMethod.Post, "http://127.0.0.1:8000/api/ai/analyze-team");
        request.Content = JsonContent.Create(requestBody);

        try
        {
            var response = await _http.SendAsync(request);
            if (!response.IsSuccessStatusCode)
            {
                return "Failed to get an analysis from the AI.";
            }

            var json = await response.Content.ReadAsStringAsync();
            using var doc = JsonDocument.Parse(json);
            return doc.RootElement.GetProperty("content").GetString() ?? "No analysis generated.";
        }
        catch
        {
            return "Could not reach the AI Python backend. Is it running?";
        }
    }
}
