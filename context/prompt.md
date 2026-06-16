## `# RotoDex — Complete Master Project Specification` 

## `## Project Overview` 

```
Build a modern Pokémon save editing ecosystem called **RotoDex**.
```

```
RotoDex is inspired by PKHeX but is designed as a modern, expandable, cross-
platform platform with a clean architecture and long-term maintainability.
```

```
Primary goals:
```

- `Modern desktop application` 

- `Android application` 

- `Web application` 

- `Discord bot` 

- `Shared Pokémon engine` 

- `Offline-first operation` 

- `Automatic upstream synchronization from PKHeX where feasible` 

- `Clean separation between upstream Pokémon logic and RotoDex-specific features` 

```
---
```

## `# Project Name` 

```
**RotoDex**
```

```
Product lineup:
```

```
```text
RotoDex Desktop
RotoDex Mobile
RotoDex Web
RotoDex Bot
RotoDex Core
```
```

```
---
```

## `# Core Philosophy` 

```
RotoDex follows the same fundamental philosophy as PKHeX:
```

- `Offline-first` 

- `No mandatory database` 

- `No mandatory account system` 

- `No mandatory cloud service` 

- `No mandatory internet connection` 

- `File-based save editing` 

```
Users should be able to:
```

```
```text
Open Save
     ↓
Edit Save
     ↓
Save File
```
```

```
without requiring any online service.
```

```
---
```

```
# PKHeX Relationship
```

```
Official upstream source:
```

```
[PKHeX GitHub Repository](https://github.com/kwsch/PKHeX.git?
utm_source=chatgpt.com)
```

```
RotoDex does NOT fork PKHeX.
```

```
RotoDex does NOT directly modify PKHeX source.
```

```
Instead:
```

```
```text
PKHeX acts as upstream.
RotoDex acts as a separate ecosystem.
```
```

```
---
```

## `# Upstream Strategy` 

```
Use PKHeX as the authoritative upstream Pokémon engine.
```

```
Responsibilities inherited from PKHeX:
```

- `Save parsing` 

- `Pokémon structures` 

- `Legality analysis` 

- `Encounter data` 

- `Mystery Gift support` 

- `New game support` 

- `Game mechanic updates` 

- `Bug fixes` 

## `Whenever PKHeX updates:` 

```
```text
PKHeX Update
```

```
      ↓
RotoDex Upstream Sync
```

```
      ↓
Dependency Refresh
```

```
      ↓
Build
```

```
      ↓
Tests
```

```
      ↓
Release Candidate
```

```
```
```

```
Goal:
```

```
Automatically inherit:
```

- `New Pokémon games` 

- `Legality updates` 

- `Event updates` 

- `Encounter updates` 

- `Fixes` 

```
whenever possible.
```

```
---
```

```
# Upstream Dumping & Synchronization System
```

```
RotoDex should include an automated upstream dumping system.
Purpose:
```

```
Extract and synchronize data from PKHeX resources.
```

```
Examples:
```

```
```text
Encounter Tables
Learnsets
Mystery Gifts
Species Data
Locations
Abilities
Moves
Personal Data
Game Constants
```
```

```
Workflow:
```text
PKHeX Repository
        ↓
Dump Resources
        ↓
Transform Resources
        ↓
RotoDex Resource Cache
        ↓
Applications Consume Data
```
```

```
This allows RotoDex to remain synchronized with PKHeX while preserving
architectural independence.
```

```
---
```

```
# Automated Upstream Monitoring
```

```
Implement automated monitoring.
```

```
Workflow:
```

```
```text
Daily GitHub Check
         ↓
PKHeX New Commit?
         ↓
Yes
         ↓
Update Dependency
         ↓
Build RotoDex
         ↓
Run Tests
         ↓
Generate Report
```
```

```
If build succeeds:
```

```
```text
Create Release Candidate
```
```

```
If build fails:
```

```
```text
Notify Maintainer
```
```

```
No automatic public release without validation.
```

```
---
```

```
# High-Level Architecture
```

```
```text
RotoDex
│
├── PKHeX.Core
│
├── RotoDex.Adapter
│
├── RotoDex.Core
│
├── RotoDex.Desktop
│
├── RotoDex.Mobile
│
├── RotoDex.Web
│
├── RotoDex.Bot
│
└── Resources
```
```

```
---
```

```
# True Dependency Flow
```

```
```text
RotoDex.Desktop
RotoDex.Mobile
RotoDex.Web
RotoDex.Bot
          ↓
     RotoDex.Core
          ↓
    RotoDex.Adapter
          ↓
      PKHeX.Core
```
```

```
Applications never communicate directly with PKHeX.
All communication passes through:
```

```
```text
RotoDex.Adapter
```
```

```
---
```

```
# PKHeX.Core Layer
```

```
Purpose:
```

```
Use PKHeX as the Pokémon engine.
```

```
Responsibilities:
```

- `Save parsing` 

- `Pokémon parsing` 

- `Legality checking` 

- `Encounter validation` 

- `Event validation` 

- `Mystery Gift support` 

- `New game support` 

```
RotoDex does not reimplement these systems.
```

```
---
```

```
# RotoDex.Adapter Layer
```

```
Purpose:
```

```
Protect RotoDex from upstream API changes.
```

```
Responsibilities:
```

- `Wrap PKHeX APIs` 

- `Normalize data` 

- `Convert PKHeX models` 

- `Provide stable interfaces` 

- `Handle version compatibility` 

```
Example:
```

```
```text
PKHeX changes
      ↓
Adapter updated
      ↓
RotoDex continues working
```
```

```
All PKHeX references stay inside this layer.
```

```
---
```

```
# RotoDex.Core Layer
```

```
Purpose:
```

```
Business logic layer.
```

```
Responsibilities:
```

- `Save Manager` 

- `Pokémon Manager` 

- `Team Manager` 

- `Backup Manager` 

- `Event Manager` 

- `Plugin Manager` 

- `Resource Manager` 

- `AI Systems (future)` 

```
This layer is independent of UI.
```

```
---
```

```
# Resource System
```

```
Follow PKHeX's philosophy.
```

```
No SQL.
```

```
No PostgreSQL.
```

```
No MongoDB.
```

```
No database dependency.
```

```
Resources stored as files:
```

```
```text
Resources
├── Encounters
├── Learnsets
├── MysteryGifts
├── PersonalData
├── Species
├── Moves
├── Abilities
├── Locations
├── Text
└── Metadata
```
```

```
Loaded into memory during runtime.
```

```
---
```

```
# Desktop Application
```

```
Name:
```text
RotoDex Desktop
```
```

```
Platform:
```

```
Windows
```

```
Features:
```

- `Full save editing` 

- `Pokémon editing` 

- `Trainer editing` 

- `Box editing` 

- `Item editing` 

- `Batch editor` 

- `Legality analysis` 

- `Multi-tab support * Save comparison * Undo history * Backup system * Plugin support` 

```
Goal:
```

```
A modern replacement for traditional PKHeX UI.
```

```
---
```

```
# Mobile Application
```

```
Name:
```

```
```text
RotoDex Mobile
```
```

```
Platform:
```

```
Android
```

```
Features:
```

- `Open saves` 

- `Pokémon editing` 

- `Box management` 

- `Legality checking` 

- `Import/export Pokémon files` 

```
Focus:
```

- `Fast loading` 

- `Touch-friendly design` 

- `Offline operation` 

```
---
```

## `# Web Application` 

```
Name:
```

```
```text
RotoDex Web
```
```

```
Purpose:
```

```
Browser-based Pokémon tools.
```

```
Features:
```

- `Save inspection` 

- `Legality reports` 

- `Pokémon viewer` 

- `Team builder` 

- `Event database` 

```
Shares the same core logic.
```

```
---
```

## `# Discord Bot` 

```
Name:
```

```
```text
RotoDex Bot
```

```
```
```

```
Purpose:
```

```
Remote access to RotoDex systems.
```

```
Commands:
```

```
```text
/check
/team
/generate
/compare
/eventdex
```
```

```
Capabilities:
```

```
* Legality reports
```

```
* Team analysis
```

```
* Pokémon inspection
```

```
* Resource lookups
```

```
---
```

```
# Backup System
```

```
Every save modification should support:
```

```
```text
Auto Backup
```
```

```
Workflow:
```

```
```text
Save Opened
      ↓
Backup Created
      ↓
User Edits
      ↓
Save Written
```
```

```
---
```

```
# Plugin System
```

```
Future support:
```

```
```text
Plugins
├── Tools
├── Utilities
├── Generators
├── Reports
└── Custom Features
```
```

```
Plugins should consume:
```

```
```text
RotoDex.Core
```

```
```
```

```
instead of accessing PKHeX directly.
```

```
---
```

```
# RotoDex Vault (Future)
```

```
Optional.
```

```
Purpose:
```

```
Local Pokémon archive.
```

```
Structure:
```

```
```text
Vault
 ├── Boxes
 ├── Pokémon
 ├── Teams
 └── Collections
```
```

```
Offline by default.
```

```
---
```

```
# RotoDex Events
```

```
Purpose:
```

```
Searchable event database.
```

```
Includes:
```

```
* Mystery Gifts
```

```
* Historical distributions
```

```
* Event Pokémon
```

```
Data synchronized from PKHeX resources.
```

```
---
```

```
# RotoDex Analyzer
```

```
Purpose:
```

```
Advanced legality explanation.
```

```
Instead of:
```

```
```text
Legal
```
```

```
Display:
```

```
```text
Origin Game
Encounter Method
Ball Validation
Move Validation
Ribbon Validation
```

```
PID Validation
Generation Path
```
```

```
Explain WHY.
```

```
---
```

```
# RotoDex Team Builder
```

```
Features:
```

```
* Competitive teams
```

```
* Team import/export
```

```
* Team storage
```

```
* Team analysis
```

```
---
```

```
# AI Layer (Future)
```

```
Optional.
```

```
Capabilities:
```

```
```text
Build Team
Explain Legality
Suggest Improvements
Analyze Pokémon
```
```

```
Must never replace core editing functionality.
```

```
---
```

```
# Technology Stack
```

```
Language:
```

```
```text
C#
```
```

```
Framework:
```

```
```text
.NET 8+
```
```

```
Version Control:
```

```
```text
Git
```
```

```
CI/CD:
```

```
```text
GitHub Actions
```
```

```
Discord:
```

```
```text
Discord.NET
```
```

```
Shared Architecture:
```

```
```text
RotoDex.Core
```
```

```
---
```

```
# Development Roadmap
## Phase 1
Build:
```text
PKHeX Integration
RotoDex.Adapter
RotoDex.Core
Desktop Application
```
```

```
---
```

```
## Phase 2
Build:
```text
Legality Tools
Resource Dumping
Backup Manager
Plugin Foundation
```
```

```
---
```

```
## Phase 3
Build:
```text
Discord Bot
```
```

```
---
```

```
## Phase 4
Build:
```text
Android Application
```
```

```
---
```

```
## Phase 5
Build:
```

```
```text
Web Application
```
```

```
---
```

## `## Phase 6` 

```
Build:
```

```
```text
Team Builder
Vault
Analyzer
AI Layer
```
```

```
---
```

## `# Mission Statement` 

```
RotoDex is a modern Pokémon save editing ecosystem built around PKHeX as an
upstream engine while remaining architecturally independent.
```

```
It should:
```

- `Remain offline-first` 

- `Avoid database dependencies` 

- `Support desktop, mobile, web, and Discord` 

- `Automatically synchronize with upstream PKHeX changes whenever possible` 

- `Use automated dumping and resource synchronization from PKHeX` 

- `Maintain a stable adapter layer to absorb upstream changes` 

- `Deliver a modern user experience while preserving the power and compatibility of PKHeX` 

```
The final result should be a long-term Pokémon platform capable of inheriting
future game support, legality fixes, encounter updates, and resource
improvements from PKHeX with minimal maintenance effort.
```

