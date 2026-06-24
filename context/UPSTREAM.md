# Upstream Strategy

RotoDex uses a **Dual-Upstream Strategy** to acquire its data without reinventing the wheel:
1. **Core Engine**: [PKHeX](https://github.com/kwsch/PKHeX) is the authoritative engine for save editing, legality, and battle mechanics.
2. **Lore Encyclopedia**: [PokeAPI](https://github.com/PokeAPI/pokeapi) is the authoritative source for rich UI data like flavor text, complex evolutions, and high-quality sprites.

This document explains the synchronization strategy and how RotoDex stays current with both upstream sources automatically.

---

## The Relationship

RotoDex does **not** fork its upstreams. RotoDex does **not** modify their source code or raw data files.

They are cloned directly from source:
- `upstream_pkhex/` undergoes a renaming process to become `Roto.Core`.
- `upstream_lore/` provides raw CSVs that are processed offline.

```
PKHeX (Clone & Rename) ──► RotoDex.Adapter ──► RotoDex.Core ──► Applications
PokeAPI (Raw CSV Clone) ─► Lore.Dumper ──────► RotoDex.Core (Optional Extension)
```

This means:
- Updates flow in automatically via Git.
- New game support and new Pokedex lore arrive without RotoDex code changes.
- RotoDex maintains 100% architectural independence.

---

## What RotoDex Inherits from PKHeX

| Domain | Inherited |
|---|---|
| Save parsing | ✅ |
| Pokémon data structures | ✅ |
| Legality analysis | ✅ |
| Encounter validation | ✅ |
| Mystery Gift support | ✅ |
| New game support | ✅ (on PKHeX release) |
| Mechanic updates | ✅ |
| Bug fixes | ✅ |

RotoDex does not reimplement any of these systems.

---

## Automated Upstream Monitoring

A GitHub Actions workflow runs daily to check for PKHeX updates.

### Workflow

```
Daily Trigger (cron)
        ↓
Check PKHeX upstream repository for new commits
        ↓
Has it changed since last sync?
        ↓
Yes ──► Pull latest source & run renaming process
        ↓
        Build RotoDex
        ↓
        Run Tests
        ↓
        Pass ──► Create Release Candidate branch
        Fail ──► Open GitHub Issue + Notify Maintainer
```

### No Automatic Public Releases

The automation creates a **Release Candidate** — it does not publish directly. A maintainer reviews and promotes it.

This prevents a broken PKHeX update from shipping to end users.

---

## Upstream Resource Dumping

Beyond the engine itself, the upstream repositories contain raw resource data:
- **From PKHeX**: Encounter tables, learnsets, game constants, ability names.
- **From PokeAPI**: Pokedex flavor text, sprite mappings, evolution chains.

These are dumped using two specialized offline tools: `tools/Roto.Dumper` (for Core) and `tools/Lore.Dumper` (for PokeAPI data).

### Dump Workflow

```
PKHeX Repo ──► Roto.Dumper ──► resources/Core/ JSONs
PokeAPI Repo ─► Lore.Dumper ──► resources/Lore/ JSONs + link.json
        ↓
ResourceManager loads them into memory at runtime
        ↓
Applications consume data through ResourceManager
```

### Triggering Resource Dumps

Resource dumps run:
- Automatically as part of the upstream monitoring workflow when an update is detected.
- Manually via:
  ```bash
  dotnet run --project tools/Roto.Dumper
  dotnet run --project tools/Lore.Dumper
  ```

---

## Adapter Compatibility

When PKHeX makes a **breaking API change**, only `RotoDex.Adapter` requires updates.

The workflow:

```
PKHeX breaks a public API
        ↓
Automated build fails
        ↓
Maintainer is notified
        ↓
Adapter is updated to absorb the change
        ↓
RotoDex.Core and applications unchanged
        ↓
Build passes → Release Candidate
```

This is the core value of the Adapter layer — it is the only place in RotoDex that understands PKHeX's internals.

---

## Version Tracking

The currently synced PKHeX version is recorded in:

```
upstream/UPSTREAM-VERSION.md
```

Format:

```
PKHeX Version: 24.xx.xx
Synced At: YYYY-MM-DD
Resources Dumped: YYYY-MM-DD
Notes: (any adapter changes required)
```

This file is updated automatically by the monitoring workflow.

---

## Manual Sync (If Needed)

```bash
# Pull upstream source and run renaming script
git pull https://github.com/kwsch/PKHeX.git?utm_source=chatgpt.com
./tools/RenameAndIntegrate.ps1

# Run resource dump
dotnet run --project tools/UpstreamDumper

# Build everything
dotnet build RotoDex.sln

# Run tests
dotnet test RotoDex.sln
```

---

## What RotoDex Does NOT Inherit

RotoDex does not use PKHeX's UI. It does not use PKHeX's Windows Forms layer. It does not use any PKHeX plugin interfaces. Only `Roto.Core` is referenced.
