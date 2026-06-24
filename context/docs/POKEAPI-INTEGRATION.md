# PokeAPI (Lore) Integration

This document outlines how RotoDex utilizes data from [PokeAPI](https://github.com/PokeAPI/pokeapi) while maintaining a strict, 100% offline-first architecture with pure separation of concerns.

## The "Separated Upstream" Architecture

RotoDex relies on PKHeX (`Roto.Core`) for all save editing, legality, and battle mechanics. However, PKHeX is not an encyclopedia; it lacks rich user interface data like high-quality sprites, Pokédex flavor text (lore), and complex evolutionary chains.

To provide this rich data without violating the offline-first rule, we utilize PokeAPI's raw data repository as a secondary, entirely separate upstream source.

### 1. Dual Upstream Clones
Just as the PKHeX engine is cloned into `upstream_pkhex/`, the raw database of PokeAPI (which consists of raw `.csv` files) is cloned into `upstream_lore/`.

```bash
git clone https://github.com/PokeAPI/pokeapi.git upstream_lore
```
*(Note: We specifically target the `data/v2/csv/` directory inside the repository).*

### 2. Separated Extraction Tools
We maintain two completely isolated dumper tools:
- `tools/Roto.Dumper`: Extracts core data from `upstream_pkhex` and outputs JSONs to `resources/Core/`.
- `tools/Lore.Dumper`: Parses the CSVs from `upstream_lore`, extracts flavor text, names, and assets, and outputs pure JSONs to `resources/Lore/`.

### 3. The Connecting Point
Because the data remains completely isolated, `Lore.Dumper` generates a mapping file called `resources/link.json` (or `map.json`).
This file acts as a bridge. For example, it dictates that PKHeX Species ID `25` maps directly to PokeAPI ID `25`.

### 4. Soft Dependency (Core) vs Strict Requirement (Bot)
While the `ResourceManager` in `RotoDex.Core` treats the PokeAPI data as a **soft dependency** (it will not crash if the data is missing, it will just gracefully hide the lore elements in the Desktop/Mobile UI), certain applications like the **Discord Bot strictly require it**.
- Upon startup, the core engine checks if `resources/Lore/` exists.
- If it does, the `HasLore` flag is set to true and the data is loaded.
- For the Discord Bot, this data is mandatory to render the rich embeds (high-quality sprites and flavor text) that users expect.

## Updating the Data
When a new Pokémon generation is released:
1. `git pull` in `upstream_pkhex/`
2. `git pull` in `upstream_lore/`
3. Run `Roto.Dumper`
4. Run `Lore.Dumper`

This process ensures that the RotoDex ecosystem is instantly updated with both the latest game mechanics and the latest rich encyclopedic data, entirely offline.
