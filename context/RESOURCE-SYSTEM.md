# Resource System

RotoDex uses a **file-based resource system** — no SQL, no database, no server required. This follows PKHeX's own philosophy and keeps the application completely offline.

---

## Philosophy

Resources are static data that change only when PKHeX updates. They do not need a database. They are:

- Loaded into memory at application startup
- Never mutated at runtime
- Refreshed only when the upstream dumper runs

---

## Resource Directory Structure

RotoDex uses a **Dual-Folder Structure** to strictly separate game mechanics (Core) from encyclopedia data (Lore).

```
resources/
│
├── Core/
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
│       └── resource-version.json
│
├── Lore/  (Optional Extension)
│   ├── link.json      <-- The mapping bridge between Core and Lore
│   ├── FlavorText/
│   ├── FormSprites/
│   ├── Evolutions/
│   └── Metadata/
│       └── pokeapi-version.json
```

---

## Resource Metadata

`resources/Metadata/resource-version.json` tracks which PKHeX version these resources were dumped from:

```json
{
  "pkhex_version": "24.xx.xx",
  "dumped_at": "2024-01-01T00:00:00Z",
  "resource_hash": "abc123..."
}
```

The `ResourceManager` reads this on startup and logs if resources are outdated relative to the current Roto.Core engine.

---

## ResourceManager

`ResourceManager` lives in `RotoDex.Core`.

Responsibilities:

- Load all resources into memory on startup
- Provide typed accessors to other managers and adapters
- Validate that resources match the current PKHeX version
- Emit a warning (not a crash) if resources are stale

### API Surface (example)

```csharp
// Species names
string name = ResourceManager.GetSpeciesName(speciesId, Language.English);

// Learnset
Learnset learnset = ResourceManager.GetLearnset(speciesId, generation);

// Locations
string location = ResourceManager.GetLocationName(locationId, game, Language.English);

// Move name
string move = ResourceManager.GetMoveName(moveId, Language.English);
```

---

## UpstreamDumper Tool

Located at `tools/UpstreamDumper/`.

This is a command-line tool that reads from the PKHeX repository and exports resources into the `resources/` directory.

### Running It

```bash
dotnet run --project tools/UpstreamDumper -- --pkhex-path /path/to/PKHeX --output resources/
```

Or using defaults (extracted from the cloned upstream repository):

```bash
dotnet run --project tools/UpstreamDumper
```

### What It Dumps

| Resource | Source in PKHeX |
|---|---|
| Species data | `Roto.Core` embedded resources |
| Learnsets | `Roto.Core` embedded resources |
| Move data | `Roto.Core` embedded resources |
| Ability data | `Roto.Core` embedded resources |
| Encounter tables | `Roto.Core` embedded resources |
| Mystery Gifts | `Roto.Core` embedded resources |
| Location tables | `Roto.Core` embedded resources |
| Personal data | `Roto.Core` embedded resources |

---

## Loading Strategy

Resources are loaded **once at startup**, not on demand.

```
Application Start
       ↓
ResourceManager.Initialize()
       ↓
Load all files into typed in-memory models
       ↓
Validate version metadata
       ↓
Application Ready
```

This means all resource lookups are O(1) dictionary or array accesses — no file I/O during editing.

---

## Adding a New Resource Type

1. Define the data model in `RotoDex.Core/Resources/Models/`
2. Add a loader in `RotoDex.Core/Resources/Loaders/`
3. Register in `ResourceManager.Initialize()`
4. Add dump logic in `tools/UpstreamDumper/Dumpers/`
5. Write a unit test that loads the resource and validates key entries

---

## No Database — By Design

This is a deliberate architectural choice, not a limitation.

Databases add:
- Deployment complexity
- Migration management
- A running service requirement
- Conflicts with offline-first operation

File-based resources add:
- None of the above
- Simple version control (resources are just files in git)
- Trivial distribution (ship the files with the app)
- Zero startup dependencies
