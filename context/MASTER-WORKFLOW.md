# Master Workflow & Central Thread

This document serves as the central thread connecting the entire RotoDex ecosystem. It outlines the complete lifecycle—from initializing the repository, integrating the core engine, to developing and deploying the various applications.

## 1. Initialization & Upstream Sync
The very first step in the RotoDex lifecycle is acquiring the required data engines. We use a dual-upstream model.
- **Action**: Clone the official upstream repositories: 
  ```bash
  # Core Engine
  git clone https://github.com/kwsch/PKHeX.git upstream_pkhex
  
  # Lore / Pokedex Encyclopedia Engine (PokeAPI)
  git clone https://github.com/PokeAPI/pokeapi.git upstream_lore
  ```
- **Related Docs**: 
  - [UPSTREAM.md](./UPSTREAM.md): Explains how we track and sync upstream commits.
  - [TECH-STACK.md](./TECH-STACK.md): Details the technologies driving the cloned source.

## 2. The Renaming & Integration Process
RotoDex does not use PKHeX as a standard pre-built package. We mold it to fit our ecosystem.
- **Action**: Run the renaming script to convert `upstream_pkhex` source into `Roto.Core` and seamlessly add the restructured projects into `RotoDex.sln`.
- **Related Docs**:
  - [ARCHITECTURE.md](./ARCHITECTURE.md): Shows where this newly integrated code fits (specifically below the Adapter layer).
  - [CORE-PHILOSOPHY.md](./CORE-PHILOSOPHY.md): Explains why we maintain architectural independence despite relying on the upstream source.

## 3. Resource Extraction
Once the upstream sources are integrated, we extract their data for offline use.
- **Action**: 
  - Run the `Roto.Dumper` tool to pull encounter tables, learnsets, and event data into `resources/Core/`.
  - Run the `Lore.Dumper` tool to parse `upstream_lore` CSVs and pull rich Pokedex text and sprite mappings into `resources/Lore/`, generating the `link.json` connecting map.
- **Related Docs**:
  - [RESOURCE-SYSTEM.md](./RESOURCE-SYSTEM.md): Explains our strict "No Database" policy, the dual-folder structure, and the Soft Dependency architecture.
  - [POKEAPI-INTEGRATION.md](../docs/POKEAPI-INTEGRATION.md): Details the separation of concerns between Core and Lore resources.

## 4. Application Development
With the Core and Adapter layers fully integrated and resources extracted, the various RotoDex products consume the engine.
- **Action**: Build and run the Desktop, Mobile, Web, or Discord applications.
- **Related Docs**:
  - [APPLICATIONS.md](./APPLICATIONS.md): Details the primary interfaces (Desktop, Mobile, Web, Bot).
  - [BOT_DEPLOYMENT.md](../docs/BOT_DEPLOYMENT.md): Detailed deployment and configuration instructions for the Discord Bot.
  - [ECOSYSTEM.md](./ECOSYSTEM.md): Highlights advanced tools like the Vault, Analyzer, and AI Layer that augment these apps.
  - [FEATURES.md](./FEATURES.md): Provides a comprehensive feature-by-feature breakdown of each application.

## 5. Extensions & Community
Extensibility and open collaboration are built into the workflow.
- **Action**: Develop or load custom plugins to extend functionality without modifying the core engine.
- **Related Docs**:
  - [PLUGIN-SYSTEM.md](./PLUGIN-SYSTEM.md): A technical guide to authoring and loading plugins.
  - [CONTRIBUTING.md](./CONTRIBUTING.md): Guidelines for submitting pull requests and reporting issues.

## 6. Future Progression
The project advances according to a heavily structured plan to ensure all components evolve in sync.
- **Action**: Monitor and contribute to upcoming phases.
- **Related Docs**:
  - [ROADMAP.md](./ROADMAP.md): Our phased approach to achieving the full ecosystem vision.
  - [CHANGELOG.md](./CHANGELOG.md): The historical record of versions and upstream syncs.

## 7. Workflow State Tracking & Temp MDs
To maintain a robust and proper workflow, we rely on the continuous use of temporary Markdown files (`temp mds`) as scratchpads, task planners, and state trackers.
- **Action**: Dynamically generate `.md` files (like `task.md`, `implementation_plan.md`, `walkthrough.md`, or `scratch.md`) during active development or automation processes.
- **Purpose**: These files track exactly where you are in the master workflow, detail proposed architecture changes (Implementation Plans), summarize delivered functionality (Walkthroughs), keep the context flowing smoothly across multiple sessions, and act as transient connectors for complex, multi-step operations.
