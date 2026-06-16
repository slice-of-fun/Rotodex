# RotoDex

> A modern Pokémon save editing ecosystem built around PKHeX as an upstream engine.

---

## What is RotoDex?

RotoDex is a cross-platform Pokémon save editing platform. It wraps [PKHeX](https://github.com/kwsch/PKHeX) as its authoritative upstream Pokémon engine while remaining architecturally independent — inheriting PKHeX's power without being locked to its UI or structure.

RotoDex is **offline-first**. No account. No cloud. No database. Just open your save, edit it, and save it.

---

## Where to Start

The main part and foundation of RotoDex's core engine relies entirely on the upstream PKHeX project. If you are looking for the official upstream source where all the core Pokémon logic (parsing saves, legality, encountering data) begins, start here:

**[PKHeX GitHub Repository](https://github.com/kwsch/PKHeX)**

---

## Product Lineup

| Product | Platform | Status |
|---|---|---|
| **RotoDex Desktop** | Windows | Phase 1 |
| **RotoDex Mobile** | Android | Phase 4 |
| **RotoDex Web** | Browser | Phase 5 |
| **RotoDex Bot** | Discord | Phase 3 |
| **RotoDex Core** | Shared Library | Phase 1 |

---

## Core Philosophy

```
Open Save
    ↓
Edit Save
    ↓
Save File
```

No internet required. No account required. No database required.

---

## Architecture at a Glance

```
RotoDex.Desktop
RotoDex.Mobile
RotoDex.Web
RotoDex.Bot
        ↓
   RotoDex.Core
        ↓
  RotoDex.Adapter
        ↓
    Roto.Core
```

PKHeX is the engine. The Adapter shields everything above it from upstream changes.

---

## Quick Navigation

| Document | Purpose |
|---|---|
| [MASTER-WORKFLOW.md](./MASTER-WORKFLOW.md) | Central thread connecting setup, renaming, and the entire ecosystem |
| [CORE-PHILOSOPHY.md](./CORE-PHILOSOPHY.md) | Fundamental principles and mission statement |
| [ARCHITECTURE.md](./ARCHITECTURE.md) | Full system design and layer breakdown |
| [APPLICATIONS.md](./APPLICATIONS.md) | Overview of Desktop, Mobile, Web, and Bot apps |
| [ECOSYSTEM.md](./ECOSYSTEM.md) | Vault, Events, Analyzer, Team Builder, Backup, and AI |
| [ROADMAP.md](./ROADMAP.md) | Phase-by-phase development plan |
| [UPSTREAM.md](./UPSTREAM.md) | PKHeX sync strategy and automation |
| [CONTRIBUTING.md](./CONTRIBUTING.md) | How to contribute |
| [TECH-STACK.md](./TECH-STACK.md) | Languages, frameworks, and tooling |
| [FEATURES.md](./FEATURES.md) | Full feature list per product |
| [PLUGIN-SYSTEM.md](./PLUGIN-SYSTEM.md) | Plugin architecture guide |
| [RESOURCE-SYSTEM.md](./RESOURCE-SYSTEM.md) | File-based resource design |
| [CHANGELOG.md](./CHANGELOG.md) | Version history |

---

## Technology

- **Language:** C#
- **Framework:** .NET 8+
- **CI/CD:** GitHub Actions
- **Discord:** Discord.NET

---

## License

See [LICENSE](./LICENSE).
