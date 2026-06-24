# Roadmap

RotoDex is built in phases. Each phase produces a shippable increment — nothing is left in a broken half-built state at the end of a phase.

---

## Overview

| Phase | Focus | Target |
|---|---|---|
| Phase 1 | Core Foundation + Desktop | First working save editor |
| Phase 2 | Tooling + Legality + Resources | Production-ready desktop |
| Phase 3 | Discord Bot | Remote access |
| Phase 4 | Android Application | Mobile editing |
| Phase 5 | Web Application | Browser tools |
| Phase 6 | Advanced Features + AI | Long-term capabilities |

---

## Phase 1 — Core Foundation

**Goal:** A working Pokémon save editor on Windows.

### Deliverables

- [x] PKHeX cloned and renaming process implemented
- [x] `RotoDex.Adapter` project scaffolded
  - [x] Save loading and writing wrapped
  - [x] Pokémon model conversion
  - [x] Legality check wrapper
- [x] `RotoDex.Core` project scaffolded
  - [x] `SaveManager` implemented
  - [x] `PokémonManager` implemented
  - [x] `BackupManager` implemented (auto-backup on open)
- [x] `RotoDex.Desktop` project scaffolded
  - [x] Open save file
  - [x] View Pokémon in boxes
  - [x] Edit Pokémon fields
  - [x] Write save file
  - [x] Basic legality indicator
- [x] Unit tests for Adapter and Core
- [x] GitHub Actions CI pipeline (build + test)

### Exit Criteria

A user can open a save file, edit a Pokémon, and write the save back — offline, with no account.

---

## Phase 2 — Tooling & Production Polish

**Goal:** A desktop application worth daily use.

### Deliverables

- [x] Upstream Dumper tools
  - [x] `tools/Roto.Dumper` (PKHeX core data)
    - [x] Dump encounters from PKHeX
    - [x] Dump learnsets
    - [x] Dump Mystery Gifts
    - [x] Dump personal data
  - [x] `tools/Lore.Dumper` (PokeAPI data)
    - [x] Dump flavor text and sprites
    - [x] Generate `link.json` map
- [x] `ResourceManager` in Core consumes dumped files (Lore data)
- [x] `RotoDex.Analyzer` — human-readable legality explanations
  - [x] Origin game
  - [x] Encounter method
  - [x] Ball validation
  - [x] Move validation
  - [x] Ribbon validation
  - [x] Generation path tracking
- [x] Desktop UI Modernization
  - [x] Dark Mode / Glassmorphism Theme
  - [x] Pokémon Sprite Rendering
  - [x] Detailed Property Grid
  - [x] Analyzer Legality Integration
- [x] Batch Editor
  - [x] Bulk apply attributes (e.g. `Level=100`) support
- [x] Multi-tab save support
- [x] Undo / redo history
  - [x] Command Manager
  - [x] UI Bindings
- [x] Plugin system foundation
  - [x] Plugin loader
  - [x] Plugin API surface (`RotoDex.Core` only)
- [x] Automated upstream monitoring GitHub Action
  - [x] Daily PKHeX commit check
  - [x] Auto-update dependency
  - [x] Build and test
  - [x] Notify on failure

### Exit Criteria

A fully featured, beautiful desktop editor with legality analysis, backup, batch editing, and automated upstream tracking.

---

## Phase 3 — Discord Bot

**Goal:** Remote RotoDex access via Discord.

### Deliverables

- [x] `RotoDex.Bot` project scaffolded (Discord.NET)
- [x] Commands implemented:
  - [x] `/check` — legality report for attached Pokémon file
  - [x] `/team` — team analysis
  - [x] `/generate` — generate a legal Pokémon
  - [x] `/compare` — compare two Pokémon files
  - [x] `/eventdex` — look up event distributions
- [x] Bot consumes `RotoDex.Core` only
- [x] Bot utilizes `PokeAPI/Lore` data for rich embeds
  - [x] Load `resources/Lore/` assets
  - [x] Render high-quality sprites in legality report
  - [x] Append flavor text to reports
- [x] Deployment documentation

### Exit Criteria

Bot is deployable and responds to all commands with accurate data.

---

## Phase 4 — Android Application

**Goal:** On-device save editing for Android.

### Deliverables

- [x] `RotoDex.Mobile` project scaffolded (.NET MAUI)
- [x] Open save from device storage
- [x] View and edit Pokémon
- [x] Box management
- [x] Legality check display
- [x] Import / export `.pk*` files
- [x] Offline-first, no network required
- [x] Touch-optimized UI

### Exit Criteria

Users can open, edit, and save Pokémon saves on Android without a desktop.

---

## Phase 5 — Web Application

**Goal:** Browser-based Pokémon tooling.

### Deliverables

- [x] `RotoDex.Web` project scaffolded (Blazor WASM)
- [x] Save inspection (no edit, read-only viewer)
- [x] Legality report display
- [x] Pokémon viewer
- [x] Team builder (import/export Showdown format)
- [x] Event database browser

### Exit Criteria

Core web tools are accessible in a modern browser with no install required.

---

## Phase 6 — Advanced Features

**Goal:** Long-term platform capabilities.

### Deliverables

- [x] **RotoDex Vault** — local Pokémon archive (boxes, teams, collections)
- [x] **Event Database** — offline Mystery Gift searching and injection
- [x] **Advanced Analyzer** — full generation path and validity explanation
- [x] **AI Layer**
  - [x] Team builder suggestions
  - [x] Legality explanation in plain language
  - [x] Pokémon improvement suggestions
  - [x] Must not replace any core editing functionality

### Exit Criteria

RotoDex is a complete long-term Pokémon platform.

---

## Ongoing — Upstream Sync

At every phase, the upstream synchronization system runs continuously:

```
Daily GitHub Check
        ↓
PKHeX New Commit?
        ↓
Update Dependency
        ↓
Build RotoDex
        ↓
Run Tests
        ↓
Pass → Release Candidate
Fail  → Notify Maintainer
```

No new game, no legality fix, no encounter update should require manual RotoDex changes.
