# Plugin System

RotoDex supports a plugin system that allows third-party tools, utilities, and features to extend the editor without modifying core code.

Plugins are introduced in **Phase 2** and expanded in later phases.

---

## Design Goals

- Plugins extend RotoDex without touching `RotoDex.Core` or `RotoDex.Adapter`
- Plugins consume a stable, documented API surface
- A broken plugin cannot crash the host application
- Plugins are sandboxed from PKHeX — they never reference it directly
- Discovery is file-based (drop a DLL into a folder)

---

## Plugin API

Plugins interact with RotoDex through the `IRotoDexPlugin` interface, defined in `RotoDex.Core`.

```csharp
namespace RotoDex.Core.Plugins;

public interface IRotoDexPlugin
{
    string Name { get; }
    string Author { get; }
    string Version { get; }
    string Description { get; }

    void Initialize(IPluginContext context);
    void OnSaveOpened(ISaveContext save);
    void OnSaveClosed();
}
```

Plugins receive a context object — they never receive raw PKHeX types.

---

## Plugin Context

```csharp
public interface IPluginContext
{
    IResourceManager Resources { get; }
    ISaveManager SaveManager { get; }
    IPokémonManager PokémonManager { get; }
    IEventBus Events { get; }
}
```

This surface is the only thing plugins can see. PKHeX internals are not exposed.

---

## Plugin Types

| Type | Description |
|---|---|
| **Tool** | Adds a menu item or panel to the Desktop UI |
| **Utility** | Background helper (auto-formatter, validator, etc.) |
| **Generator** | Generates Pokémon from templates or rulesets |
| **Report** | Produces export files (CSV, text, Showdown) |
| **Custom Feature** | Anything else — full freedom within the API |

---

## Directory Structure

Plugins are loaded from:

```
%AppData%/RotoDex/Plugins/
    └── MyPlugin/
        ├── MyPlugin.dll
        └── plugin.json
```

`plugin.json` declares metadata:

```json
{
  "id": "com.example.myplugin",
  "name": "My Plugin",
  "author": "Example Dev",
  "version": "1.0.0",
  "description": "Does something useful.",
  "entry": "MyPlugin.dll",
  "requires_roto_dex_version": "1.0.0"
}
```

---

## Plugin Lifecycle

```
Application Start
       ↓
PluginManager.DiscoverPlugins()
       ↓
For each valid plugin directory:
    Load plugin.json
    Load DLL into isolated AssemblyLoadContext
    Resolve IRotoDexPlugin implementation
    Call plugin.Initialize(context)
       ↓
Plugin is active
       ↓
Application Events fire plugin callbacks
       ↓
Application Shutdown
    plugin.Dispose() if IDisposable
```

---

## Isolation

Each plugin loads into its own `AssemblyLoadContext`. This means:

- Plugin dependency conflicts don't affect RotoDex or other plugins
- A plugin that throws an unhandled exception is caught and logged — it does not crash the host

---

## What Plugins Cannot Do

| Restriction | Reason |
|---|---|
| Reference `Roto.Core` directly | Breaks the Adapter abstraction |
| Reference `RotoDex.Adapter` | Same reason |
| Access the filesystem outside designated plugin storage | Security |
| Open network connections without explicit permission | Security |

These restrictions are enforced through the limited API surface and, in future, Roslyn Analyzer rules.

---

## Writing a Plugin

1. Create a new Class Library project targeting `.NET 8`
2. Add a NuGet reference to `RotoDex.Plugin.Sdk` (once published)
3. Implement `IRotoDexPlugin`
4. Add a `plugin.json`
5. Build and place the output folder in `%AppData%/RotoDex/Plugins/`

A minimal example:

```csharp
using RotoDex.Core.Plugins;

public class HelloPlugin : IRotoDexPlugin
{
    public string Name => "Hello Plugin";
    public string Author => "Example";
    public string Version => "1.0.0";
    public string Description => "Prints a message when a save is opened.";

    private IPluginContext _context = null!;

    public void Initialize(IPluginContext context)
    {
        _context = context;
    }

    public void OnSaveOpened(ISaveContext save)
    {
        Console.WriteLine($"Save opened: {save.TrainerName}");
    }

    public void OnSaveClosed() { }
}
```

---

## Plugin Manager UI (Desktop)

`RotoDex.Desktop` includes a Plugin Manager panel:

- List installed plugins
- Enable / disable per plugin
- View plugin metadata
- Open plugin folder
- Reload plugins without restarting (where possible)
