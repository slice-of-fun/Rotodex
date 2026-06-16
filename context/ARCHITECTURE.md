# Architecture

This document describes the full system design of RotoDex — how layers are structured, what each one owns, and how data flows through the system.

---

## Guiding Principles

1. **Applications never talk to PKHeX directly.** All PKHeX access goes through `RotoDex.Adapter`.
2. **The Adapter absorbs upstream changes.** When PKHeX updates its API, only the Adapter needs to change.
3. **Core is UI-agnostic.** `RotoDex.Core` has no knowledge of any front-end.
4. **No database dependencies.** Resources are file-based and loaded into memory at runtime.
5. **Offline-first.** Every product must work without any network access.

---

## Layer Diagram

```
┌─────────────────────────────────────────────────────┐
│              Applications (Consumers)               │
│                                                     │
│   RotoDex.Desktop   RotoDex.Mobile                  │
│   RotoDex.Web       RotoDex.Bot                     │
└────────────────────┬────────────────────────────────┘
                     │
                     ▼
┌─────────────────────────────────────────────────────┐
│                  RotoDex.Core                       │
│                                                     │
│   Save Manager      Pokémon Manager                 │
│   Team Manager      Backup Manager                  │
│   Event Manager     Plugin Manager                  │
│   Resource Manager  AI Systems (future)             │
└────────────────────┬────────────────────────────────┘
                     │
                     ▼
┌─────────────────────────────────────────────────────┐
│                RotoDex.Adapter                      │
│                                                     │
│   Wraps PKHeX APIs                                  │
│   Normalizes data models                            │
│   Handles version compatibility                     │
│   Provides stable interfaces                        │
└────────────────────┬────────────────────────────────┘
                     │
                     ▼
┌─────────────────────────────────────────────────────┐
│                  Roto.Core                          │
│                                                     │
│   Save parsing      Legality analysis               │
│   Pokémon parsing   Encounter data                  │
│   Mystery Gifts     Game support                    │
└─────────────────────────────────────────────────────┘
```

---

## Layer Responsibilities

### Roto.Core

PKHeX is used by cloning the official repository and applying an automated renaming process.

Responsibilities owned by PKHeX:

- Save file parsing and serialization
- Pokémon data structures
- Legality analysis engine
- Encounter table validation
- Mystery Gift support
- New game support on upstream release
- Game mechanic constants and logic

RotoDex does not reimplement any of this.

---

### RotoDex.Adapter

The Adapter is the **firewall** between PKHeX and the rest of RotoDex.

Responsibilities:

- Import and re-export PKHeX types as RotoDex-stable interfaces
- Convert PKHeX models into RotoDex domain models
- Abstract away PKHeX API surface
- Handle version compatibility shims when PKHeX breaks changes

When PKHeX releases an update that changes its public API, only this layer needs updating. Everything above it continues to function unchanged.

All PKHeX `using` statements live exclusively inside this project. No other RotoDex project references PKHeX directly.

---

### RotoDex.Core

The business logic layer. Fully platform-agnostic.

Managers:

| Manager | Responsibility |
|---|---|
| `SaveManager` | Open, write, track state of save files |
| `PokémonManager` | Edit, clone, validate Pokémon |
| `TeamManager` | Build, import, export competitive teams |
| `BackupManager` | Auto-backup on open, manual restore |
| `EventManager` | Mystery Gift and event distribution data |
| `PluginManager` | Load, validate, execute plugins |
| `ResourceManager` | Load file-based resource data into memory |

This layer depends on `RotoDex.Adapter` and nothing else.

---

### Applications

Each application consumes `RotoDex.Core` only.

| Application | Platform | Notes |
|---|---|---|
| `RotoDex.Desktop` | Windows (WPF / WinUI) | Full feature set |
| `RotoDex.Mobile` | Android (MAUI) | Touch-optimized, offline |
| `RotoDex.Web` | Browser (Blazor WASM) | Inspect, view, report |
| `RotoDex.Bot` | Discord.NET | Command-driven access |

---

## Data Flow: Editing a Pokémon

```
User selects Pokémon in UI
        ↓
RotoDex.Desktop calls RotoDex.Core.PokémonManager
        ↓
PokémonManager calls RotoDex.Adapter.GetPokémon()
        ↓
Adapter calls Roto.Core internally
        ↓
Adapter returns a RotoDex PokémonModel (PKHeX-free)
        ↓
Core applies edits and validates
        ↓
Adapter converts back and passes to PKHeX for legality check
        ↓
Result surfaces to UI
```

---

## Data Flow: Opening a Save

```
User opens file
        ↓
Desktop passes path to SaveManager
        ↓
SaveManager calls Adapter.LoadSave(path)
        ↓
Adapter calls Roto.Core save parser
        ↓
Adapter returns RotoDex.SaveFile model
        ↓
BackupManager creates a timestamped copy
        ↓
UI renders save contents
```

---

## Solution Structure

```
RotoDex.sln
│
├── src/
│   ├── RotoDex.Adapter/
│   ├── RotoDex.Core/
│   ├── RotoDex.Desktop/
│   ├── RotoDex.Mobile/
│   ├── RotoDex.Web/
│   └── RotoDex.Bot/
│
├── tests/
│   ├── RotoDex.Adapter.Tests/
│   ├── RotoDex.Core.Tests/
│   └── RotoDex.Integration.Tests/
│
├── resources/
│   ├── Encounters/
│   ├── Learnsets/
│   ├── MysteryGifts/
│   ├── PersonalData/
│   ├── Species/
│   ├── Moves/
│   ├── Abilities/
│   ├── Locations/
│   ├── Text/
│   └── Metadata/
│
├── tools/
│   └── UpstreamDumper/
│
└── .github/
    └── workflows/
```

---

## Dependency Rules (Enforced)

| Project | May reference |
|---|---|
| `RotoDex.Adapter` | `Roto.Core` only |
| `RotoDex.Core` | `RotoDex.Adapter` only |
| `RotoDex.Desktop` | `RotoDex.Core` only |
| `RotoDex.Mobile` | `RotoDex.Core` only |
| `RotoDex.Web` | `RotoDex.Core` only |
| `RotoDex.Bot` | `RotoDex.Core` only |

Violations of this rule should be caught by architecture tests in `RotoDex.Integration.Tests`.
