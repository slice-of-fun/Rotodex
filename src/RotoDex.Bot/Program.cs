using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using RotoDex.Bot.Services;
using RotoDex.Core;
using System;
using System.Threading.Tasks;

namespace RotoDex.Bot;

public class Program
{
    public static async Task Main(string[] args)
    {
        DotNetEnv.Env.Load();
        
        // Initialize Core Resources (including Lore)
        string resourceDir = System.IO.Path.GetFullPath(System.IO.Path.Combine(AppContext.BaseDirectory, "../../../../../resources"));
        RotoDex.Core.Resources.ResourceManager.Initialize(resourceDir);

        var host = Host.CreateDefaultBuilder(args)
            .ConfigureAppConfiguration((context, config) =>
            {
                config.AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);
                config.AddEnvironmentVariables();
            })
            .ConfigureServices((context, services) =>
            {
                // Discord.Net configuration
                var discordSocketConfig = new DiscordSocketConfig
                {
                    GatewayIntents = GatewayIntents.Guilds | GatewayIntents.GuildMessages,
                    LogLevel = LogSeverity.Info,
                    AlwaysDownloadUsers = true
                };

                services.AddSingleton(discordSocketConfig);
                services.AddSingleton<DiscordSocketClient>();

                var interactionServiceConfig = new InteractionServiceConfig
                {
                    LogLevel = LogSeverity.Info,
                    DefaultRunMode = RunMode.Async
                };

                services.AddSingleton(interactionServiceConfig);
                services.AddSingleton<InteractionService>();

                // Hosted service that manages the connection and commands
                services.AddHostedService<InteractionHandler>();
                services.AddHttpClient();

                // Optional: Register RotoDex.Core managers here if needed
                // e.g., services.AddSingleton<PokemonManager>();
                // Though usually PKHeX.Core initialization is static.
            })
            .Build();

        await host.RunAsync();
    }
}
