using System;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace RotoDex.Desktop;

public static class AiApiClient
{
    private static readonly HttpClient _httpClient = new HttpClient();
    private static string? _apiKey;

    static AiApiClient()
    {
        _apiKey = LoadApiKey();
    }

    private static string? LoadApiKey()
    {
        // Try to find .env file by traversing up from the executable directory
        string? dir = AppDomain.CurrentDomain.BaseDirectory;
        while (!string.IsNullOrEmpty(dir))
        {
            string envPath = Path.Combine(dir, ".env");
            if (File.Exists(envPath))
            {
                foreach (var line in File.ReadAllLines(envPath))
                {
                    if (line.StartsWith("GROQ_API_KEY="))
                    {
                        return line.Substring("GROQ_API_KEY=".Length).Trim();
                    }
                }
            }
            dir = Path.GetDirectoryName(dir);
        }
        return null;
    }

    public static async Task<string> ExplainLegalityAsync(string speciesName, bool isLegal, string checkMsg)
    {
        if (string.IsNullOrEmpty(_apiKey))
            return "Error: GROQ_API_KEY not found in .env file.";

        string prompt = $@"
You are a Pokémon legality expert. I am analyzing a {speciesName}.
The legality checker returned the following result for this Pokémon:
Status: {(isLegal ? "Valid" : "Invalid")}
Message: {checkMsg}

Explain this legality check in 1-2 short, simple sentences. Don't use overly technical hexadecimal terms if possible. Make it easy for a casual player to understand why this is {(isLegal ? "legal" : "illegal")}.
";
        return await CallGroqAsync(prompt, "You are a helpful Pokémon legality expert.", 150);
    }

    public static async Task<string> AnalyzeTeamAsync(string showdownExport)
    {
        if (string.IsNullOrEmpty(_apiKey))
            return "Error: GROQ_API_KEY not found in .env file.";

        string prompt = $@"
You are a competitive Pokémon team building expert. I am building a Pokémon team and here is my current team in Showdown format:

{showdownExport}

Please analyze this team for competitive battles. Provide:
1. A brief overview of the team's strengths and core strategy.
2. The team's biggest weaknesses (e.g. type weaknesses, lack of hazards, speed control).
3. 2-3 specific suggestions for improvement (e.g. swap a move, change an item, or suggest a specific Pokémon to replace an existing one to improve synergy).
Keep your response concise, using Markdown formatting, and strictly focused on competitive Pokémon mechanics.
";
        return await CallGroqAsync(prompt, "You are a competitive Pokémon team building expert.", 500);
    }

    private static async Task<string> CallGroqAsync(string userPrompt, string systemPrompt, int maxTokens)
    {
        var requestBody = new
        {
            model = "llama3-8b-8192",
            messages = new[]
            {
                new { role = "system", content = systemPrompt },
                new { role = "user", content = userPrompt }
            },
            temperature = 0.4,
            max_tokens = maxTokens
        };

        var content = new StringContent(JsonSerializer.Serialize(requestBody), Encoding.UTF8, "application/json");

        var request = new HttpRequestMessage(HttpMethod.Post, "https://api.groq.com/openai/v1/chat/completions");
        request.Headers.Add("Authorization", $"Bearer {_apiKey}");
        request.Content = content;

        try
        {
            var response = await _httpClient.SendAsync(request);
            response.EnsureSuccessStatusCode();

            var responseJson = await response.Content.ReadAsStringAsync();
            using var doc = JsonDocument.Parse(responseJson);
            var resultText = doc.RootElement.GetProperty("choices")[0].GetProperty("message").GetProperty("content").GetString();
            return resultText?.Trim() ?? "No response content from Groq.";
        }
        catch (Exception ex)
        {
            return $"Error calling Groq API: {ex.Message}";
        }
    }
}
