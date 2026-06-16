# Core Philosophy

RotoDex follows the same fundamental philosophy as PKHeX, with a focus on a clean, modern, offline-first experience.

## Key Principles

- **Offline-first**: Every component must work without a network connection.
- **No mandatory database**: We avoid SQL, PostgreSQL, MongoDB, Redis, etc.
- **No mandatory account system**: Users own their data entirely locally.
- **No mandatory cloud service**: There are no cloud SDKs or remote syncing requirements by default.
- **No mandatory internet connection**: Once downloaded, it works indefinitely.
- **File-based save editing**: The application reads and writes standard save files directly.

## Workflow

The user experience should be seamless and linear:

1. **Open Save**
2. **Edit Save**
3. **Save File**

This is achieved without requiring any online service.

## Mission Statement

RotoDex is a modern Pokémon save editing ecosystem built around PKHeX as an upstream engine while remaining architecturally independent.

It should:
- Remain offline-first.
- Avoid database dependencies.
- Support desktop, mobile, web, and Discord.
- Automatically synchronize with upstream PKHeX changes whenever possible.
- Use automated dumping and resource synchronization from PKHeX.
- Maintain a stable adapter layer to absorb upstream changes.
- Deliver a modern user experience while preserving the power and compatibility of PKHeX.

The final result should be a long-term Pokémon platform capable of inheriting future game support, legality fixes, encounter updates, and resource improvements from PKHeX with minimal maintenance effort.
