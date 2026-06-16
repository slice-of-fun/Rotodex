# Features

Full feature list for each RotoDex product. Items are marked by phase.

---

## RotoDex Desktop (Windows)

The primary application. Full save editing power.

### Save Management

| Feature | Phase |
|---|---|
| Open save file from disk | 1 |
| Write save file to disk | 1 |
| Auto-backup on open | 1 |
| Manual backup and restore | 2 |
| Multi-tab (multiple saves open) | 2 |
| Save comparison view (diff two saves) | 2 |
| Recent files list | 1 |

### Pokémon Editing

| Feature | Phase |
|---|---|
| View all Pokémon in boxes | 1 |
| Edit species, level, moves | 1 |
| Edit IVs, EVs, nature, ability | 1 |
| Edit held item, friendship, markings | 1 |
| Edit OT, TID, SID, origins | 1 |
| Edit ribbons | 2 |
| Clone Pokémon | 1 |
| Delete Pokémon | 1 |
| Import `.pk*` files from disk | 1 |
| Export `.pk*` files to disk | 1 |
| Undo / redo history | 2 |

### Legality

| Feature | Phase |
|---|---|
| Basic legal / illegal indicator | 1 |
| Human-readable legality breakdown (Analyzer) | 2 |
| Origin game display | 2 |
| Encounter method explanation | 2 |
| Ball legality explanation | 2 |
| Move legality explanation | 2 |
| Ribbon legality explanation | 2 |
| Generation path display | 2 |

### Trainer & Game Data

| Feature | Phase |
|---|---|
| View and edit trainer info (name, ID, etc.) | 1 |
| Item bag editor | 2 |
| Pokédex editor | 2 |

### Batch Editing

| Feature | Phase |
|---|---|
| Batch editor (apply changes to multiple Pokémon) | 2 |
| Filter and select by condition | 2 |

### Plugins

| Feature | Phase |
|---|---|
| Plugin loader | 2 |
| Plugin API (via `RotoDex.Core`) | 2 |
| Plugin manager UI | 2 |

---

## RotoDex Mobile (Android)

Focused on fast, touch-friendly offline editing.

| Feature | Phase |
|---|---|
| Open save from device storage | 4 |
| View Pokémon in boxes | 4 |
| Edit Pokémon (species, moves, stats) | 4 |
| Box management (move, swap) | 4 |
| Legality indicator | 4 |
| Import `.pk*` files | 4 |
| Export `.pk*` files | 4 |
| Offline operation (no network) | 4 |

---

## RotoDex Web (Browser)

Read-focused tools. No save writing in Phase 5.

| Feature | Phase |
|---|---|
| Save inspection (read-only) | 5 |
| Legality report display | 5 |
| Pokémon viewer | 5 |
| Team builder (Showdown import/export) | 5 |
| Event / Mystery Gift database browser | 5 |

---

## RotoDex Bot (Discord)

Command-driven remote access.

| Command | What it does | Phase |
|---|---|---|
| `/check` | Legality report for attached `.pk*` file | 3 |
| `/team` | Analyze a team (paste or attach) | 3 |
| `/generate` | Generate a legal Pokémon from a spec | 3 |
| `/compare` | Compare two Pokémon files | 3 |
| `/eventdex` | Look up event / Mystery Gift history | 3 |

---

## RotoDex Vault (Future — Phase 6)

Local Pokémon archive. Offline. No cloud.

| Feature | Phase |
|---|---|
| Store Pokémon in local boxes | 6 |
| Store named teams | 6 |
| Create named collections | 6 |
| Search by species, move, ball, game | 6 |
| Import from save | 6 |
| Export to save | 6 |

---

## RotoDex Events (Phase 6)

Searchable historical event and Mystery Gift database.

| Feature | Phase |
|---|---|
| Browse all known distributions | 6 |
| Filter by game, region, date | 6 |
| View event details | 6 |
| Download Wonder Card files where available | 6 |

---

## RotoDex Analyzer (Phase 2+)

Explains legality in human-readable terms, not just Legal / Illegal.

Instead of:

```
❌ Illegal
```

RotoDex shows:

```
Origin Game        : Pokémon Scarlet
Encounter Method   : Koraidon (starter) — Box 1 Slot 1
Ball               : ✅ Poké Ball (valid for starter)
Moves              : ✅ All legal for level 5
Ribbons            : ✅ None — as expected
PID                : ✅ Valid for Gen 9
Generation Path    : Gen 9 → (no transfers)
```

---

## AI Layer (Phase 6 — Optional)

All AI features are additive. They never replace core editing.

| Feature | Notes |
|---|---|
| Team suggestions | Suggest competitive team based on preferences |
| Plain-language legality | Explain legality report in simple terms |
| Pokémon improvement | Suggest EV spread, moveset improvements |
| Must not replace editing | AI assists; humans decide and execute |
