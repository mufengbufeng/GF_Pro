# GameFramework-Next 项目架构分析

## 项目概述
- **项目名称**: GameFramework-Next
- **项目类型**: Unity商业游戏开发框架
- **核心特性**: 支持C#热更新的完整游戏框架
- **分析日期**: 2025-06-16

## 核心技术栈
- **Unity版本**: 支持Unity 2022.3+
- **热更新方案**: HybridCLR
- **资源管理**: YooAsset
- **配置系统**: Luban
- **异步库**: UniTask
- **UI动画**: DOTween
- **本地化**: I2Localization

## 程序集架构

### 非热更新程序集
1. **GameMain.Runtime** (`Assets/GameScripts/Runtime/`)
   - 启动流程管理
   - 核心系统封装
   - 平台相关功能
   - 关键文件: `ProcedureLaunch.cs:12`, `ProcedureLoadAssembly.cs:21`

### 热更新程序集
1. **GameBase** (`Assets/GameScripts/HotFix/GameBase/`)
   - 基础工具类和扩展
   - 单例模式实现
   - UI组件扩展
   - 关键文件: `BaseLogicSys.cs`, `Singleton.cs`

2. **GameProto** (`Assets/GameScripts/HotFix/GameProto/`)
   - Luban配置系统
   - 数据结构定义
   - 配置加载器
   - 关键文件: `ConfigSystem.cs:13`, `Tables.cs`

3. **GameLogic** (`Assets/GameScripts/HotFix/GameLogic/`)
   - 主要业务逻辑
   - UI系统实现
   - 事件系统
   - 关键文件: `GameApp.cs:7`, `UISystem.cs:13`

## 启动流程

### 完整启动链条
```
ProcedureLaunch (语言/声音初始化)
↓
ProcedureSplash (启动画面)
↓
ProcedureInitResources (资源系统初始化)
↓
ProcedureUpdateVersion (版本检查)
↓
ProcedureUpdateManifest (清单更新)
↓
ProcedureCreateDownloader (创建下载器)
↓
ProcedureDownloadFile (文件下载)
↓
ProcedureLoadAssembly (程序集加载)
↓
ProcedureStartGame (游戏启动)
```

### 热更新入口
- **入口方法**: `GameApp.Entrance()` (`GameApp.cs:15`)
- **系统注册**: `GameApp_RegisterSystem.cs`
- **生命周期**: Init → Start → Update → LateUpdate → FixedUpdate → Destroy

## UI系统架构

### 核心特性
- **窗口管理**: 基于栈的窗口管理 (`UISystem.cs:17`)
- **层级系统**: Order-based深度排序
- **加载方式**: 支持异步/同步加载
- **可见性管理**: 全屏窗口自动遮挡

### 关键常量
```csharp
internal const int LAYER_DEEP = 2000;     // 层级深度间隔
internal const int WINDOW_DEEP = 100;     // 窗口深度间隔
internal const int WINDOW_HIDE_LAYER = 2; // 隐藏层
internal const int WINDOW_SHOW_LAYER = 5; // 显示层
```

### UI根节点结构
- **UIRoot**: 主根节点
- **UICanvas**: UI画布 (`UISystem.cs:47`)
- **UICamera**: UI相机 (`UISystem.cs:51`)

## 配置系统

### Luban配置
- **配置路径**: `Configs/GameConfig/`
- **Excel数据**: `Configs/GameConfig/Datas/`
- **生成脚本**: `gen_code_bin_to_project.bat`
- **配置加载**: `ConfigSystem.cs:35` - Load()方法
- **二进制加载**: `LoadByteBuf()` (`ConfigSystem.cs:46`)

### 配置文件结构
- **__beans__.xlsx**: Bean定义
- **__enums__.xlsx**: 枚举定义  
- **__tables__.xlsx**: 表格定义
- **item.xlsx**: 道具配置示例

## 资源管理

### YooAsset配置
- **配置文件**: `AssetBundleCollectorSetting.asset`
- **默认包**: DefaultPackage
- **其他包**: OtherPackage, Dlc1Package, Dlc2Package

### 资源分组策略
- **UI资源**: 单独打包 (`PackSeparately`)
- **配置文件**: 预加载标签 (`PRELOAD`)
- **其他资源**: 按目录打包 (`PackDirectory`)

### 资源路径映射
```
Assets/AssetRaw/Actor     → Actor Group
Assets/AssetRaw/Audios    → Audios Group  
Assets/AssetRaw/Configs   → Configs Group (PRELOAD)
Assets/AssetRaw/UI        → UI Group (PackSeparately)
Assets/AssetRaw/DLL       → DLL Group
```

## 事件系统

### 接口驱动设计
- **UI事件接口**: `Assets/GameScripts/HotFix/GameLogic/Event/Interface/UIEventInterface/`
- **逻辑事件接口**: `Assets/GameScripts/HotFix/GameLogic/Event/Interface/LogicEventInterface/`
- **代码生成**: `Assets/GameScripts/HotFix/GameLogic/Event/Gen/`

### 事件注册
- **UI事件注册**: `RegisterEventInterface_UI.cs`
- **逻辑事件注册**: `RegisterEventInterface_Logic.cs`

## 关键系统常量

### 目录结构
```
Assets/
├── AssetRaw/           # 原始资源
├── GameScripts/        # 游戏脚本
│   ├── Runtime/        # 非热更代码
│   └── HotFix/         # 热更新代码
├── UnityGameFramework/ # 框架核心
└── Scenes/             # 场景文件
```

### 构建输出
```
Bundles/                # 资源包输出
HybridCLRData/         # 热更新数据
└── HotUpdateDlls/     # 热更新程序集
```

## 性能优化要点

### 低GC设计
- 循环中避免new操作
- 预分配集合容器
- 对象池模式应用
- TProfiler性能分析集成

### 关键性能代码位置
- **Update循环**: `GameApp.cs:82-95`
- **UI更新**: `UISystem.cs:470-488`
- **窗口栈管理**: `UISystem.cs:426-468`

## 工具链配置

### Luban工具
- **构建脚本**: `Tools/build-luban.sh` / `Tools/build-luban.bat`
- **配置生成**: `Configs/GameConfig/gen_code_bin_to_project.bat`

### HybridCLR配置
- **设置文件**: `ProjectSettings/HybridCLRSettings.asset`
- **程序集路径**: 可配置的热更新程序集列表

### YooAsset设置
- **全局设置**: `Assets/UnityGameFramework/ResRaw/YooAssetSettings/`
- **编辑器模式**: 支持编辑器仿真模式

## 扩展指南

### 添加新UI窗口
1. 继承 `UIWindow` 类
2. 添加 `WindowAttribute` 特性
3. 在 `UIController` 中注册
4. 放置Prefab到 `Assets/AssetRaw/UI/`

### 添加新系统
1. 继承 `BaseLogicSys<T>` 
2. 实现生命周期方法
3. 在 `GameApp_RegisterSystem.cs` 中注册

### 添加新配置
1. 在Excel中定义数据结构
2. 运行Luban生成脚本
3. 通过 `ConfigSystem.Tables` 访问