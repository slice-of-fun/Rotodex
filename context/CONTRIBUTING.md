# Contributing to RotoDex

Thank you for your interest in contributing. This document covers how to set up your development environment, the rules for contributions, and the workflow for submitting changes.

---

## Before You Start

Read these first:

- [ARCHITECTURE.md](./ARCHITECTURE.md) — understand the layer structure before touching code
- [ROADMAP.md](./ROADMAP.md) — understand what's planned and what phase we're in
- [UPSTREAM.md](./UPSTREAM.md) — understand the PKHeX relationship

---

## Development Environment

### Requirements

| Tool | Version |
|---|---|
| .NET SDK | 8.0+ |
| Git | Any recent |
| IDE | Visual Studio 2022, Rider, or VS Code with C# extension |

### Setup

```bash
# Clone the repository
git clone https://github.com/your-org/RotoDex.git
cd RotoDex

# Restore dependencies
dotnet restore

# Build
dotnet build

# Run tests
dotnet test
```

---

## Project Structure

```
src/
├── RotoDex.Adapter/       ← PKHeX wrapper. Touch only if PKHeX API changed.
├── RotoDex.Core/          ← Business logic. Most contributions land here.
├── RotoDex.Desktop/       ← Windows UI. UI contributions land here.
├── RotoDex.Mobile/        ← Android UI. Phase 4+.
├── RotoDex.Web/           ← Blazor WASM. Phase 5+.
└── RotoDex.Bot/           ← Discord Bot. Phase 3+.

tests/
├── RotoDex.Adapter.Tests/
├── RotoDex.Core.Tests/
└── RotoDex.Integration.Tests/   ← Architecture enforcement tests. Do not break these.

tools/
└── UpstreamDumper/        ← Resource sync tool.
```

---

## Dependency Rules — Non-Negotiable

Before submitting a PR, verify:

1. No project other than `RotoDex.Adapter` references `Roto.Core`
2. No application project references `RotoDex.Adapter`
3. `RotoDex.Core` only references `RotoDex.Adapter`

The architecture tests in `RotoDex.Integration.Tests` enforce this and will fail the build if violated.

---

## Branching

| Branch | Purpose |
|---|---|
| `main` | Stable. Protected. No direct pushes. |
| `develop` | Integration branch. PRs target here. |
| `feature/your-feature-name` | Your work |
| `fix/short-description` | Bug fixes |
| `upstream/sync-YYYY-MM-DD` | Upstream PKHeX sync (automated) |

---

## Making a Change

```bash
# Create your branch from develop
git checkout develop
git pull
git checkout -b feature/my-feature

# Make your changes

# Run tests before pushing
dotnet test

# Push and open a PR targeting develop
git push origin feature/my-feature
```

---

## Pull Request Checklist

Before opening a PR:

- [ ] Code builds cleanly (`dotnet build`)
- [ ] All existing tests pass (`dotnet test`)
- [ ] New tests written for new logic
- [ ] Architecture tests pass (no dependency rule violations)
- [ ] No direct PKHeX references outside `RotoDex.Adapter`
- [ ] PR description explains *what* changed and *why*
- [ ] If UI change: screenshot or recording attached

---

## What Belongs Where

| Change type | Where it goes |
|---|---|
| New PKHeX wrapping | `RotoDex.Adapter` |
| New business logic | `RotoDex.Core` |
| New UI element (Desktop) | `RotoDex.Desktop` |
| New Discord command | `RotoDex.Bot` |
| New resource type | `RotoDex.Core/Resources` + `tools/UpstreamDumper` |
| New plugin API surface | `RotoDex.Core/Plugins` |

---

## Code Style

- Follow standard C# conventions (PascalCase types, camelCase fields with `_` prefix)
- Use `var` when type is obvious from the right-hand side
- Prefer expression bodies for simple properties
- XML doc comments on all public APIs in `RotoDex.Core` and `RotoDex.Adapter`
- No nullable suppressions (`!`) without a comment explaining why

---

## Reporting Issues

Open an issue on GitHub with:

- RotoDex version
- PKHeX version (shown in Help → About)
- Steps to reproduce
- Expected behaviour
- Actual behaviour
- Save file (if relevant and shareable — you can DM maintainers for sensitive files)

---

## Upstream PKHeX Issues

If the issue is in PKHeX itself (save parsing, legality logic, encounter data), report it to [PKHeX Issues](https://github.com/kwsch/PKHeX/issues) — not here. RotoDex will inherit the fix on the next upstream sync.

---

## Questions

Open a Discussion on GitHub rather than an Issue if you have a question about architecture, design decisions, or planned features.
