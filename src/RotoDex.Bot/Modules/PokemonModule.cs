using Discord;
using Discord.Interactions;
using RotoDex.Analyzer.Models;
using RotoDex.Core;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace RotoDex.Bot.Modules;

public class PokemonModule : InteractionModuleBase<SocketInteractionContext>
{
    private readonly IHttpClientFactory _httpClientFactory;

    // We inject IHttpClientFactory so we can download attachments safely.
    public PokemonModule(IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
    }

    [SlashCommand("check", "Upload a .pk file to receive a legality report.")]
    public async Task CheckLegalityAsync(
        [Summary("file", "The Pokémon file to check (e.g., .pk8, .pk9)")] IAttachment file)
    {
        // Defer the response since downloading and processing might take a moment.
        await DeferAsync();

        if (file.Size > 1024 * 10) // Restrict size to prevent abuse
        {
            await FollowupAsync("File is too large. Please upload a valid Pokémon file.");
            return;
        }

        try
        {
            // Download the file
            var client = _httpClientFactory.CreateClient();
            var response = await client.GetAsync(file.Url);
            var data = await response.Content.ReadAsByteArrayAsync();

            var pokemon = RotoDex.Adapter.PokemonAdapter.Parse(data);
            if (pokemon == null)
            {
                await FollowupAsync("Failed to parse the file. Please ensure it is a valid Pokémon save file or entity.");
                return;
            }

            var report = RotoDex.Adapter.LegalityWrapper.CheckLegality(pokemon);

            var description = report.IsLegal ? "**This Pokémon is Legal!**" : "❌ **This Pokémon is Illegal!**";
            
            if (RotoDex.Core.Resources.ResourceManager.HasLore)
            {
                var flavorText = RotoDex.Core.Resources.ResourceManager.GetFlavorText(pokemon.Species);
                if (!string.IsNullOrEmpty(flavorText))
                {
                    description += $"\n\n*{flavorText}*";
                }
            }

            var embed = new EmbedBuilder()
                .WithTitle($"Legality Report: {file.Filename}")
                .WithDescription(description)
                .WithColor(report.IsLegal ? Color.Green : Color.Red);
                
            if (RotoDex.Core.Resources.ResourceManager.HasLore)
            {
                embed.WithThumbnailUrl($"https://raw.githubusercontent.com/PokeAPI/sprites/master/sprites/pokemon/{pokemon.Species}.png");
            }

            void AddFieldIfAny(string name, System.Collections.Generic.List<RotoDex.Analyzer.Models.AnalyzerCheckResult> checks)
            {
                if (checks.Count > 0)
                    embed.AddField(name, string.Join("\n", checks.Select(c => (c.Valid ? "✅" : "❌") + " " + c.Message)));
            }

            AddFieldIfAny("Origin", report.OriginChecks);
            AddFieldIfAny("Encounter", report.EncounterChecks);
            AddFieldIfAny("Ball", report.BallChecks);
            AddFieldIfAny("Moves", report.MoveChecks);
            AddFieldIfAny("Ribbons", report.RibbonChecks);
            AddFieldIfAny("Stats", report.StatsChecks);
            AddFieldIfAny("Misc", report.MiscChecks);
            AddFieldIfAny("PID", report.PIDChecks);

            await FollowupAsync(embed: embed.Build());
        }
        catch (System.Exception ex)
        {
            await FollowupAsync($"Failed to process the file: {ex.Message}");
        }
    }

    [SlashCommand("team", "Analyze a team of Pokémon.")]
    public async Task AnalyzeTeamAsync(
        [Summary("paste_file", "A .txt file containing a Showdown paste")] IAttachment file)
    {
        await DeferAsync();

        if (file.Size > 1024 * 50) // 50 KB max
        {
            await FollowupAsync("File is too large. Please upload a valid text file.");
            return;
        }

        try
        {
            var client = _httpClientFactory.CreateClient();
            var pasteText = await client.GetStringAsync(file.Url);

            var team = RotoDex.Adapter.TeamAdapter.ParseShowdown(pasteText);
            
            int total = 0;
            int legal = 0;
            
            var embed = new EmbedBuilder()
                .WithTitle($"Team Legality Report: {file.Filename}");

            bool first = true;

            foreach(var pkmn in team)
            {
                total++;
                var report = RotoDex.Adapter.LegalityWrapper.CheckLegality(pkmn);
                if (report.IsLegal)
                    legal++;
                
                string status = report.IsLegal ? "✅" : "❌";
                
                string details = "";
                if (!report.IsLegal)
                {
                    var errors = new System.Collections.Generic.List<string>();
                    errors.AddRange(report.EncounterChecks.Select(c => c.Message));
                    errors.AddRange(report.MoveChecks.Select(c => c.Message));
                    errors.AddRange(report.MiscChecks.Select(c => c.Message));
                    if (errors.Count > 0)
                        details = " (" + string.Join(", ", errors) + ")";
                }

                embed.AddField($"{status} {pkmn.Nickname}", $"Level {pkmn.Level}{details}");

                if (first && RotoDex.Core.Resources.ResourceManager.HasLore)
                {
                    embed.WithThumbnailUrl($"https://raw.githubusercontent.com/PokeAPI/sprites/master/sprites/pokemon/{pkmn.Species}.png");
                    first = false;
                }
            }
            
            if (total == 0)
            {
                await FollowupAsync("No valid Pokémon found in the provided paste.");
                return;
            }

            embed.WithDescription($"Parsed **{total}** Pokémon. **{legal}** Legal, **{total - legal}** Illegal.");
            embed.WithColor(legal == total ? Color.Green : Color.Red);

            await FollowupAsync(embed: embed.Build());
        }
        catch (System.Exception ex)
        {
            await FollowupAsync($"Failed to process the team file: {ex.Message}");
        }
    }

    [SlashCommand("generate", "Generate a legal Pokémon based on parameters.")]
    public async Task GeneratePokemonAsync([Summary("set", "The species name or Showdown paste")] string set)
    {
        await DeferAsync();

        try
        {
            var result = RotoDex.Adapter.GeneratorAdapter.GenerateLegalPokemon(set);
            
            if (result == null)
            {
                await FollowupAsync("❌ Failed to generate a legal Pokémon for the requested set. Please ensure the moves, stats, and abilities are legally possible.");
                return;
            }

            // Write the file to a temporary stream and upload it
            using var ms = new MemoryStream(result.Value.Data);
            
            var embed = new EmbedBuilder()
                .WithTitle("Generation Successful!")
                .WithDescription($"Successfully generated a 100% legal **{result.Value.Filename.Replace(".pk9", "")}**!")
                .WithColor(Color.Green)
                .Build();

            await FollowupWithFileAsync(ms, result.Value.Filename, embed: embed);
        }
        catch (System.Exception ex)
        {
            await FollowupAsync($"❌ An error occurred during generation: {ex.Message}");
        }
    }

    [SlashCommand("compare", "Compare two Pokémon files.")]
    public async Task ComparePokemonAsync(
        [Summary("file1", "First file")] IAttachment file1, 
        [Summary("file2", "Second file")] IAttachment file2)
    {
        await DeferAsync();

        if (file1.Size > 1024 * 10 || file2.Size > 1024 * 10)
        {
            await FollowupAsync("❌ One or both files are too large. Please upload valid Pokémon save/entity files.");
            return;
        }

        try
        {
            var client = _httpClientFactory.CreateClient();
            var data1 = await client.GetByteArrayAsync(file1.Url);
            var data2 = await client.GetByteArrayAsync(file2.Url);

            var diff = RotoDex.Adapter.CompareAdapter.Compare(data1, data2);

            if (diff == null)
            {
                await FollowupAsync("❌ Failed to parse one or both files. Please ensure they are valid Pokémon entities.");
                return;
            }

            var embed = new EmbedBuilder()
                .WithTitle($"Comparison: {file1.Filename} vs {file2.Filename}")
                .WithColor(Color.Blue);

            if (diff.Count == 0)
            {
                embed.WithDescription("✅ **These Pokémon are perfectly identical.**");
                embed.WithColor(Color.Green);
            }
            else
            {
                embed.WithDescription($"Found **{diff.Count}** difference(s):");
                string diffList = string.Join("\n", diff);
                
                // Discord limits field values to 1024 characters.
                if (diffList.Length > 1024)
                    diffList = diffList.Substring(0, 1000) + "...\n(Truncated)";
                    
                embed.AddField("Differences", $"```diff\n- {string.Join("\n- ", diff)}\n```");
            }

            await FollowupAsync(embed: embed.Build());
        }
        catch (System.Exception ex)
        {
            await FollowupAsync($"❌ An error occurred during comparison: {ex.Message}");
        }
    }

    [SlashCommand("eventdex", "Look up event distributions for a Pokémon.")]
    public async Task EventDexAsync([Summary("species", "The species of the Pokémon")] string species)
    {
        await DeferAsync();

        try
        {
            var events = RotoDex.Adapter.EventDexAdapter.GetEventsForSpecies(species);

            if (events == null)
            {
                await FollowupAsync($"❌ Unknown species: `{species}`. Please provide a valid species name.");
                return;
            }

            if (events.Count == 0)
            {
                await FollowupAsync($"ℹ️ No official event distributions found for **{species}**.");
                return;
            }

            string realSpriteUrl = $"https://raw.githubusercontent.com/PokeAPI/sprites/master/sprites/pokemon/other/official-artwork/{Enum.Parse<Roto.Core.Species>(species, true).ToString("d")}.png";

            var embed = new EmbedBuilder()
                .WithTitle($"Event Distributions for {char.ToUpper(species[0]) + species.Substring(1).ToLower()}")
                .WithDescription($"Found **{events.Count}** event(s) for this species.")
                .WithThumbnailUrl(realSpriteUrl)
                .WithColor(Color.Gold);

            // Group by generation
            var grouped = events.GroupBy(e => e.Generation).OrderByDescending(g => g.Key);

            foreach (var group in grouped)
            {
                var eventList = group.Select(e => $"- **Lv.{e.Level}** {e.Title}").ToList();
                string fieldContent = string.Join("\n", eventList);

                if (fieldContent.Length > 1024)
                    fieldContent = fieldContent.Substring(0, 1000) + "...\n(Truncated)";

                embed.AddField($"Generation {group.Key}", fieldContent);
            }

            await FollowupAsync(embed: embed.Build());
        }
        catch (System.Exception ex)
        {
            await FollowupAsync($"❌ An error occurred: {ex.Message}");
        }
    }
}
