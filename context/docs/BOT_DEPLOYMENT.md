# RotoDex Discord Bot Deployment

This guide explains how to deploy and configure the `RotoDex.Bot` project. 
Because the bot directly references the `RotoDex.Core` ecosystem, it requires the same .NET runtime (currently .NET 10) as the desktop application.

## Prerequisites
1. **.NET SDK** (matching the solution version)
2. **A Discord Developer Application** (to acquire a bot token)

## Acquiring a Bot Token
1. Go to the [Discord Developer Portal](https://discord.com/developers/applications).
2. Click **New Application** and name it `RotoDex`.
3. Go to the **Bot** tab and click **Add Bot**.
4. Under the **Token** section, click **Reset Token** and copy the resulting string.
   > [!WARNING]
   > Never share this token or commit it to a public GitHub repository. Doing so will allow malicious users to hijack your bot.
5. Under **Privileged Gateway Intents**, enable **Message Content Intent** (needed if you plan to parse normal messages, though Slash Commands don't strictly require it).

## Configuration
The bot relies on an `appsettings.json` file for configuration. This file is ignored by Git to prevent accidental token leaks.

1. Create a file named `appsettings.json` in the `src/RotoDex.Bot/` directory.
2. Add your token and your development server ID:

```json
{
  "DiscordToken": "YOUR_TOKEN_HERE",
  "DevGuildId": "YOUR_SERVER_ID_HERE"
}
```

> [!TIP]
> Setting the `DevGuildId` forces slash commands to register instantly to your specific server during development. Global registration can take up to an hour to propagate across Discord.

## Running the Bot
To start the bot, run the following command from the root directory:

```bash
dotnet run --project src/RotoDex.Bot/RotoDex.Bot.csproj
```

The bot will connect, register its slash commands, and await inputs like `/check`.

## Architecture Notes
The `RotoDex.Bot` project uses `Microsoft.Extensions.Hosting` to manage its lifecycle. 
- `Program.cs` builds the generic host and configures Dependency Injection.
- `InteractionHandler.cs` is a Hosted Service that connects to Discord, routes interactions, and registers the Slash Commands.
- `PokemonModule.cs` contains the actual commands (`/check`, `/team`, `/generate`, etc.).

## PokeAPI Lore Integration
The bot relies on the core engine for legality, but it strictly requires the PokeAPI Lore data to provide rich responses (such as high-quality Pokémon sprites or flavor text in its embeds).
To ensure the bot functions with rich embeds:
1. Ensure you have followed the [POKEAPI-INTEGRATION.md](./POKEAPI-INTEGRATION.md) guide to clone `upstream_lore` and dump the resources.
2. The `ResourceManager` in `RotoDex.Core` will automatically load the `resources/Lore/` directory upon the bot's startup.
3. The bot's embeds (like the legality report) will utilize this data to render high-quality sprites and localized flavor text.
