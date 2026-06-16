# Changelog

All notable changes to RotoDex are documented here.

Format follows [Keep a Changelog](https://keepachangelog.com/en/1.0.0/).
RotoDex uses [Semantic Versioning](https://semver.org/).

Each entry also notes the upstream PKHeX version it was built against.

---

## [Unreleased]

### Architecture & Scaffolding
- Initialized core repository structure (`src`, `tests`, `tools`, `resources`).
- Cloned upstream Roto.Core engine and completed the integration/renaming process.
- Scaffolded `RotoDex.Adapter` and `RotoDex.Core` layer frameworks.
- Scaffolded `RotoDex.Desktop` UI shell.
- Established `xUnit` test suite and Architecture enforcement logic.
- Established GitHub Actions CI workflow.

### Resource Extraction
- Scaffolded `Roto.Dumper` tool to extract engine resources.
- Successfully verified pipeline execution and JSON dumping into the `resources/` directory.

### Ecosystem
- Upgraded the entire RotoDex ecosystem to `.NET 10` to successfully compile upstream C# 13 preview syntax.

### RotoDex.Adapter
- Initial Roto.Core integration

### RotoDex.Desktop
- Open save file
- View Pokémon in boxes
- Edit Pokémon fields
- Write save file
- Basic legality indicator

---

<!-- Template for future entries:

## [x.y.z] — YYYY-MM-DD
Roto.Core: xx.xx.xx

### Added
-

### Changed
- Initialized core repository structure (`src`, `tests`, `tools`, `resources`).
- Cloned PKHeX upstream repository.
- Successfully applied renaming process: adapted `PKHeX.Core` into `Roto.Core` locally.

### Fixed
-

### Removed
-

### Upstream Sync
- Cloned initial PKHeX upstream commit.
- Adapted `PKHeX.Core` -> `Roto.Core` via `RenameAndIntegrate.ps1`.

-->
