# Tech Stack

This document lists every technology, framework, and tool used in RotoDex, and the reason each was chosen.

---

## Language

### C#

All RotoDex projects are written in C#.

**Why:** PKHeX is written in C#. Using the same language means RotoDex can consume PKHeX as a direct dependency without any FFI layer, marshaling, or translation overhead. It also gives access to the full .NET ecosystem and cross-platform MAUI for mobile.

---

## Runtime & Framework

### .NET 10

**Why:** Upstream PKHeX natively utilizes C# 13 preview features which dictates the use of the `.NET 10 SDK`. `RotoDex.Core` targets .NET 10 and is consumed by all platforms.

---

## Desktop

### WPF or WinUI 3 (Windows)

`RotoDex.Desktop` targets Windows.

- **WPF** — Mature, well-understood, excellent data binding, large control ecosystem. Preferred for Phase 1.
- **WinUI 3** — Modern Windows UI, better visual design system, eventual migration target.

The decision between them is made at the start of Phase 1 and documented in an ADR (Architecture Decision Record).

---

## Mobile

### .NET MAUI

`RotoDex.Mobile` targets Android via .NET MAUI.

**Why:** MAUI allows sharing `RotoDex.Core` directly with no adaptation layer. Write-once logic, platform-specific UI shell.

---

## Web

### Blazor WebAssembly

`RotoDex.Web` targets browsers via Blazor WASM.

**Why:** Blazor WASM runs .NET in the browser directly, meaning `RotoDex.Core` can be consumed without a server. This preserves offline-first operation — the web app can run without any backend after initial load.

---

## Discord Bot

### Discord.NET

`RotoDex.Bot` is built on Discord.NET.

**Why:** Discord.NET is the standard C# Discord library with full slash command, attachment, and interaction support.

---

## PKHeX Engine

### PKHeX Core (Cloned & Renamed)

Referenced directly from source after cloning and applying the renaming process in `RotoDex.Adapter`.

**Why Cloned & Renamed:** Ensures full control over the upstream source code, immediate access to updates without waiting for package publishes, and seamless integration into the RotoDex namespace ecosystem.

---

## CI/CD

### GitHub Actions

All automation runs on GitHub Actions.

Workflows:

| Workflow | Trigger | Purpose |
|---|---|---|
| `build.yml` | Push / PR | Build and test |
| `upstream-sync.yml` | Daily cron | Check PKHeX for updates |
| `release.yml` | Tag push | Build release artifacts |

---

## Version Control

### Git + GitHub

Standard Git workflow. Feature branches, PRs, protected main branch.

---

## Testing

### xUnit

Unit and integration tests use xUnit.

Test projects:

| Project | Tests |
|---|---|
| `RotoDex.Adapter.Tests` | Adapter wrapping, model conversion |
| `RotoDex.Core.Tests` | Manager logic, business rules |
| `RotoDex.Integration.Tests` | Architecture enforcement, dependency rules |

### Architecture Tests

`RotoDex.Integration.Tests` uses [NetArchTest](https://github.com/BenMorris/NetArchTest) to enforce the dependency rules:

- Applications may not reference `RotoDex.Adapter`
- Applications may not reference `Roto.Core`
- Only `RotoDex.Adapter` may reference `Roto.Core`

These tests fail the build if dependency rules are violated.

---

## Resource Format

### Flat files (binary / JSON / custom PKHeX format)

Resources are stored as files, not in a database. See [RESOURCE-SYSTEM.md](./RESOURCE-SYSTEM.md).

---

## No External Dependencies Summary

RotoDex deliberately avoids:

| Technology | Reason avoided |
|---|---|
| SQL / SQLite | Database dependency, conflicts with offline-first |
| PostgreSQL | Server dependency |
| MongoDB | Database dependency |
| Redis | Server dependency |
| REST API backend | Breaks offline-first |
| Any cloud SDK | Breaks offline-first |

The only external dependency is Roto.Core.
