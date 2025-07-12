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

## 编程规范说明

1. 在编写代码完成时，需要检查代码是否报错，如果不是预期内的报错，需要修复报错。
2. 需要保证代码低 GC，特别需要注意在(循环、Update、输入检测)中是否有频繁"new"新的实例。
3. 在编写代码时，需要充分考虑文件和脚本的单一职责性。
4. 在 Agent 模式，请直接修改代码，不需要询问用户是否需要修改。
5. md 文件请放入"memory_md"文件夹下。
6. 请只完成特定任务，不要偏离需求。

## mcp 工具使用

### repomix

**repomix** : Repomix is a powerful tool that packs your entire repository into a single, AI-friendly file.
It is perfect for when you need to feed your codebase to Large Language Models (LLMs) or other AI tools like Claude, ChatGPT, DeepSeek, Perplexity, Gemini, Gemma, Llama, Grok, and more.

repomix MCP 工具的主要功能：

1. 本地代码库打包功能 ✅

- 成功打包了当前 Unity 项目
- 支持忽略模式过滤文件
- 提供压缩选项减少 token 使用
- 生成了 308 万 token 的代码分析文件

1. 远程仓库打包功能 ✅

- 成功克隆并打包了 Microsoft VSCode 仓库
- 支持包含模式只提取特定文件
- 自动下载并处理远程仓库

1. 内容搜索功能 ✅

- grep 功能可以在打包结果中搜索特定模式
- 支持上下文行显示
- 成功找到了 GameApp 类定义

1. 内容读取功能 ✅

- 可以按行范围读取大文件的特定部分
- 避免一次性加载过大内容
- 支持分页浏览

主要特性总结：

1. 代码库分析：将整个代码库合并为单个 AI 友好的 XML 文件
2. 智能过滤：支持 gitignore、自定义忽略模式和包含模式
3. 压缩选项：Tree-sitter 压缩可减少约 70% 的 token 使用
4. 远程支持：直接处理 GitHub 仓库 URL
5. 增量访问：grep 搜索和分页读取避免处理过大内容
6. 安全性：自动检测并阻止敏感信息

### memory

**memory** : Memory 是一个外部记忆图谱工具，可以帮助你在不同的上下文之间保持信息的连贯性。它允许你存储和检索重要的上下文信息，以便在与 AI 交互时提供更丰富的背景。但是它需要你在每次对话完成后手动更新记忆。
