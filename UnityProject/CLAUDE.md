# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project Overview

GameFramework-Next is a Unity game framework built for commercial games with hot update capabilities. It integrates HybridCLR for C# hot updates, YooAsset for asset management, Luban for configuration, and UniTask for async operations.

## Essential Commands

### Configuration System (Luban)
```bash
# Build Luban tool (run once or when Luban updates)
cd Tools && ./build-luban.sh  # Linux/Mac
cd Tools && build-luban.bat   # Windows

# Generate configuration code and data (after Excel changes)
cd Configs/GameConfig && ./gen_code_bin_to_project.bat
```

### Development Workflow
- Configuration changes require running the Luban generation scripts
- Hot update assemblies must be built and deployed to `HybridCLRData/HotUpdateDlls/`
- Asset bundles are managed through YooAsset's build system

## Architecture

### Assembly Division
The project uses a 3-tier hot update architecture:

1. **Runtime Assembly** (`GameMain.Runtime`): Startup procedures and core systems (non-hot-updatable)
2. **GameBase**: Core utilities, singletons, UI extensions (hot-updatable)
3. **GameProto**: Configuration system and data protocols (hot-updatable) 
4. **GameLogic**: Main game business logic (hot-updatable)

### Startup Flow
Applications follow this procedure chain:
`ProcedureLaunch` → `ProcedureSplash` → `ProcedureInitResources` → `ProcedureUpdateVersion` → `ProcedureUpdateManifest` → `ProcedureCreateDownloader` → `ProcedureDownloadFile` → `ProcedureLoadAssembly` → `ProcedureStartGame`

Hot update logic begins at `GameApp.cs` in the GameLogic assembly.

### UI System
- Uses order-based layer management instead of traditional UIGroup system
- UI windows managed in a stack with automatic depth sorting
- Event system based on interfaces with code generation
- Resources loaded through YooAsset integration

### Configuration System
- Excel files in `Configs/GameConfig/Datas/` define game data
- Luban generates C# classes and binary data automatically
- Configuration loaded through `ConfigSystem.cs` at runtime
- Templates in `CustomTemplate/` folder define output structure

## Key Patterns

### System Architecture
- Systems implement `ILogicSys` interface with `DoInit()`, `DoStart()`, `DoUpdate()`, `DoLateUpdate()`, `DoFixedUpdate()` lifecycle methods
- Register systems in `GameApp_RegisterSystem.cs`

### Resource Management
- All resources loaded through `GameModule.Resource` (YooAsset wrapper)
- Asset addresses defined in YooAsset's AssetBundleCollectorSetting
- Support for both addressable and raw file loading

### Event System
- Interface-based events defined in `Assets/GameScripts/HotFix/GameLogic/Event/Interface/`
- Code generation creates helper classes in `Gen/` folders
- Register event interfaces in `RegisterEventInterface_*.cs` files

## File Locations

### Configuration
- Excel data files: `Configs/GameConfig/Datas/`
- Luban config: `Configs/GameConfig/luban.conf`
- Generated C# classes: `Assets/GameScripts/HotFix/GameProto/GameConfig/`

### Core Systems
- Startup procedures: `Assets/GameScripts/Runtime/Procedure/`
- Hot update entry: `Assets/GameScripts/HotFix/GameLogic/GameApp.cs`
- UI system: `Assets/GameScripts/HotFix/GameLogic/System/UISystem/`

### Asset Management
- Raw assets: `Assets/AssetRaw/`
- YooAsset settings: `Assets/UnityGameFramework/ResRaw/YooAssetSettings/`

## Development Notes

- Follow low GC allocation patterns, especially in Update loops
- UI prefabs should be placed in appropriate AssetRaw directories for YooAsset bundling
- Configuration changes require regenerating code through Luban scripts
- Hot update assemblies require special build and deployment steps through HybridCLR