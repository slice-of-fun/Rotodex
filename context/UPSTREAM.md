# Upstream Strategy

RotoDex uses [PKHeX](https://github.com/kwsch/PKHeX) as its authoritative Pokémon engine. This document explains the relationship, the synchronization strategy, and how RotoDex stays current with upstream changes automatically.

---

## The Relationship

RotoDex does **not** fork PKHeX.

RotoDex does **not** modify PKHeX source code.

PKHeX is cloned directly from source and undergoes a renaming process before being integrated. RotoDex builds on top of it through the Adapter layer.

```
PKHeX (Clone & Rename) ──► RotoDex.Adapter ──► RotoDex.Core ──► Applications
```

This means:

- PKHeX updates flow in automatically
- PKHeX bug fixes are inherited for free
- New game support arrives without RotoDex code changes (in most cases)
- RotoDex never diverges from upstream's Pokémon logic

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

Beyond the engine itself, PKHeX's repository contains raw resource data that RotoDex also benefits from:

```
Encounter tables
Learnsets
Mystery Gift event files
Species personal data
Location name tables
Ability name tables
Move name tables
Game constants
```

These are dumped using the **UpstreamDumper tool** (`tools/UpstreamDumper`) and stored in the `resources/` directory of RotoDex.

### Dump Workflow

```
PKHeX Repository (git clone or submodule reference)
        ↓
UpstreamDumper extracts and transforms resources
        ↓
Resources written to resources/ as flat files
        ↓
ResourceManager loads them into memory at runtime
        ↓
Applications consume data through ResourceManager
```

### Triggering Resource Dumps

Resource dumps run:

- Automatically as part of the upstream monitoring workflow when a PKHeX update is detected
- Manually via `dotnet run --project tools/UpstreamDumper`

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
