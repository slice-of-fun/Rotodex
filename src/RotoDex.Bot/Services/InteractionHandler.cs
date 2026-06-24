using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace RotoDex.Bot.Services;

public class InteractionHandler : IHostedService
{
    private readonly DiscordSocketClient _client;
    private readonly InteractionService _commands;
    private readonly IServiceProvider _services;
    private readonly IConfiguration _config;
    private readonly ILogger<InteractionHandler> _logger;

    public InteractionHandler(
        DiscordSocketClient client,
        InteractionService commands,
        IServiceProvider services,
        IConfiguration config,
        ILogger<InteractionHandler> logger)
    {
        _client = client;
        _commands = commands;
        _services = services;
        _config = config;
        _logger = logger;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        _client.Log += LogAsync;
        _commands.Log += LogAsync;
        _client.Ready += ReadyAsync;

        // Add the public modules that inherit InteractionModuleBase<T> to the InteractionService
        await _commands.AddModulesAsync(Assembly.GetEntryAssembly(), _services);

        // Process the InteractionCreated payloads to execute Interactions commands
        _client.InteractionCreated += HandleInteraction;

        var token = _config["DiscordToken"];
        if (string.IsNullOrWhiteSpace(token))
        {
            _logger.LogError("DiscordToken is missing from configuration. The bot cannot start.");
            return;
        }

        await _client.LoginAsync(TokenType.Bot, token);
        await _client.StartAsync();
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        await _client.StopAsync();
        await _client.LogoutAsync();
    }

    private async Task ReadyAsync()
    {
        // Register commands globally. In production, this can take up to an hour to cache.
        // For development, it's often better to register to a specific guild.
        var devGuildIdStr = _config["DevGuildId"];
        if (ulong.TryParse(devGuildIdStr, out var devGuildId))
        {
            _logger.LogInformation("Registering commands to Dev Guild: {DevGuildId}", devGuildId);
            await _commands.RegisterCommandsToGuildAsync(devGuildId);
        }
        else
        {
            _logger.LogInformation("Registering commands globally");
            await _commands.RegisterCommandsGloballyAsync();
        }

        _logger.LogInformation("Bot is connected and ready!");
    }

    private async Task HandleInteraction(SocketInteraction interaction)
    {
        try
        {
            // Create an execution context that matches the generic type parameter of your InteractionModuleBase<T> modules.
            var context = new SocketInteractionContext(_client, interaction);

            // Execute the incoming command.
            var result = await _commands.ExecuteCommandAsync(context, _services);

            if (!result.IsSuccess)
            {
                switch (result.Error)
                {
                    case InteractionCommandError.UnmetPrecondition:
                        // Ignore unmet precondition
                        break;
                    default:
                        _logger.LogError("Error executing interaction: {ErrorReason}", result.ErrorReason);
                        if (interaction.Type == InteractionType.ApplicationCommand)
                        {
                            await interaction.RespondAsync("An error occurred while executing this command.", ephemeral: true);
                        }
                        break;
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Exception handling interaction.");
            // If a Slash Command execution fails it is most likely that the original interaction acknowledgement will persist. It is a good idea to delete the original
            // response, or at least let the user know that something went wrong during the command execution.
            if (interaction.Type == InteractionType.ApplicationCommand)
            {
                await interaction.GetOriginalResponseAsync().ContinueWith(async (msg) => await msg.Result.DeleteAsync());
            }
        }
    }

    private Task LogAsync(LogMessage log)
    {
        switch (log.Severity)
        {
            case LogSeverity.Critical:
                _logger.LogCritical(log.Exception, "{Message}", log.Message);
                break;
            case LogSeverity.Error:
                _logger.LogError(log.Exception, "{Message}", log.Message);
                break;
            case LogSeverity.Warning:
                _logger.LogWarning(log.Exception, "{Message}", log.Message);
                break;
            case LogSeverity.Info:
                _logger.LogInformation(log.Exception, "{Message}", log.Message);
                break;
            case LogSeverity.Verbose:
                _logger.LogTrace(log.Exception, "{Message}", log.Message);
                break;
            case LogSeverity.Debug:
                _logger.LogDebug(log.Exception, "{Message}", log.Message);
                break;
        }
        return Task.CompletedTask;
    }
}
