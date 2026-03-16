# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project Overview

**Epic Merge** is a 2D tile-based merge puzzle game built with Unity 6.0.0 (6000.0.62f1). Players drag tiles with matching IDs/levels; when 3+ matching tiles are adjacent they merge into a higher-level tile. The graph system supports non-rectangular grids.

Main scene: `Assets/_Game/Scenes/GameScene.unity`

## Build & Development

This is a Unity project — there are no CLI build/test commands. Open the project in Unity 6.0.0 and use the Unity Editor to build and run. Unit tests can be run via **Window > General > Test Runner** in the editor.

## Architecture

### Code Organization

| Path | Purpose |
|------|---------|
| `Assets/_Game/Scripts/` | Game-specific code (tile system, drag-drop, indicators, camera) |
| `Assets/Scripts/` | Shared base systems (graph, pooling, panel, move) |
| `Assets/Plugins/Zenject/` | Dependency injection framework |
| `Assets/ThirdPartyAssets/` | UpdateManager, NaughtyAttributes |

### Assembly Definitions

The project uses modular `.asmdef` files:
- `Scripts.BaseSystems` — shared graph, pooling, panel, move systems
- `Scripts.GameSystems` — top-level game assembly
- `Scripts.Systems.TileSystem` — tile, node, detection, merge (core gameplay)
- `Scripts.Systems.DragDropSystem`, `Scripts.Systems.IndicatorSystem`, `Scripts.Systems.CameraSystem`
- `Scripts.GameDepends` — game initialization & Zenject installers

### Dependency Injection (Zenject)

All major services are wired in `Assets/_Game/Scripts/GameDepend/Zenject/ProjectInstaller.cs`. Tile objects are created via `TileObjectFactory`. Never instantiate game objects directly with `new` or `Instantiate` for systems managed by Zenject — use factories or the container.

### Core Systems

**Graph / Tile Node System** (`Assets/Scripts/Systems/GraphSystem/`, `Assets/_Game/Scripts/Systems/TileSystem/TileNodeSystem/`)
- Generic `Graph<T, TF>` and `Node<T, TF>` base classes with BFS traversal
- `TileGraph` extends the generic graph; `TileNode` holds a `TileObjectValue` (ID + Level)
- `TileGraphExtensions.FindEdgesWithNodeDistance()` auto-populates neighbor lists at generation time
- `TileGraph.FindWantedNodesWithBfs()` is the core merge-detection query

**Merge System** (`Assets/_Game/Scripts/Systems/TileSystem/TileMergeSystem/TileObjectMergeHelper.cs`)
- Static helpers `CanMerge()` and `TryMerge()` drive all merge logic
- Minimum tiles required to merge is configurable via `GameConfigurations.mergeRequiredObject` (default 3)

**Event System** (`Assets/_Game/Scripts/Systems/TileSystem/EventSystem/EventService.cs`)
- Observer pattern. Tile drag, placement, and merge events flow through `EventService`.

**Panel System** (`Assets/Scripts/Systems/PanelSystem/`)
- `PanelManager` singleton manages UI panels via `PopupType` enum
- New panels inherit `BasePanel` and are registered in `PopupType`

**Object Pooling** (`Assets/Scripts/Systems/PoolingSystem/`)
- `PoolingSystem` / `PoolGroup` / `PoolCollection` for reusable tile objects
- Pool configuration lives in `Assets/_Game/Resources/Configurations/PoolCollection.asset`

**UpdateManager** (ThirdParty)
- Centralized Update() dispatch — subscribe here instead of using MonoBehaviour.Update() directly for performance

### Game Initialization Flow

```
GameStarter.Start()
  → ShowGameplayPanel()
  → TileGraphGeneratorManager.RecreateAllGraphs()
      → TileGraphGenerator.CreateGraph()
          → Builds TileGraph, sets neighbors via FindEdgesWithNodeDistance()
          → Spawns random TileObjects on nodes via TileObjectFactory
```

### Configuration

All runtime tunables are in `Assets/_Game/Resources/Configurations/GameConfigurations.asset` (ScriptableObject). Access them via `ConfigurationService` (injected), not direct Resources.Load.

### Key Entry Points

| File | Role |
|------|------|
| `Assets/_Game/Scripts/GameDepend/GameStarter.cs` | Boot sequence |
| `Assets/_Game/Scripts/GameDepend/Zenject/ProjectInstaller.cs` | DI bindings |
| `Assets/_Game/Scripts/Systems/TileSystem/TileMergeSystem/TileObjectMergeHelper.cs` | Merge logic |
| `Assets/_Game/Scripts/Systems/TileSystem/TileNodeSystem/Graph/TileGraph.cs` | Tile graph |
| `Assets/Scripts/Systems/GraphSystem/Graph.cs` | Generic BFS graph |