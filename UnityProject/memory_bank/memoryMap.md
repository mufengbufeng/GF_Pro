# Memory Map - 文档索引

## 项目文档索引

### MCP配置相关
- [MCP服务器配置说明.md](./MCP服务器配置说明.md) - Claude Code MCP服务器配置指南

### UI系统相关  
- [UI实例根节点设置修改说明.md](./UI实例根节点设置修改说明.md) - UI实例根节点设置修改说明
- [UI框架重构说明.md](./UI框架重构说明.md) - UI框架重构说明文档

### 项目架构分析
- [GameFramework-Next项目架构分析.md](./GameFramework-Next项目架构分析.md) - 完整的项目架构分析和关键数据记录
- [GameFramework-Next深度项目分析报告.md](./GameFramework-Next深度项目分析报告.md) - 基于repomix深度分析的完整项目技术报告，涵盖架构设计、核心组件、热更新机制等

### RPG项目规划
- [RPG项目看板.md](./RPG项目看板.md) - RPG项目的任务看板和进度跟踪
- [RPG项目计划.md](./RPG项目计划.md) - RPG项目的详细开发计划和技术架构

### 游戏开发指南
- [Claude Code独立游戏美术解决方案.md](./Claude Code独立游戏美术解决方案.md) - 2024年程序员独立游戏开发美术完整解决方案，包含AI工具、免费资源、外包策略、成本分析和实施指南

## 快速导航

### 配置管理
- MCP服务器配置：参考 MCP服务器配置说明.md
- Luban配置系统：查看项目根目录 Configs/GameConfig/

### 框架核心
- 启动流程：UnityProject/Assets/GameScripts/Runtime/Procedure/
- 热更新逻辑：UnityProject/Assets/GameScripts/HotFix/GameLogic/
- UI系统：UnityProject/Assets/GameScripts/HotFix/GameLogic/System/UISystem/
- 项目架构概览：参考 GameFramework-Next项目架构分析.md

### 开发工具
- 构建脚本：Tools/
- 编辑器工具：UnityProject/Assets/GameScripts/Editor/

## FSM (有限状态机) 模块

### 核心分析
- **[GameFramework-FSM模块分析.md](GameFramework-FSM模块分析.md)** - FSM模块的完整架构分析和设计模式解读
- **[GameFramework-FSM实现方式深度分析.md](GameFramework-FSM实现方式深度分析.md)** - FSM模块实现方式的深度技术分析，包含mermaid架构图和设计模式详解

### 补充内容
本次为FSM模块补充了以下实用代码:

#### 1. 扩展工具类
- **FsmExtensions.cs** - 状态机扩展方法集合
  - 状态检查方法 (IsCurrentState)
  - 安全状态切换 (TryChangeState)
  - 临时数据管理 (SetTemporaryData/GetTemporaryData)

#### 2. 增强状态基类
- **FsmStateBase.cs** - 提供更便捷的状态基类
  - 内置状态时间追踪
  - 简化的状态切换方法
  - 数据管理便捷方法

#### 3. 条件转换系统
- **FsmCondition.cs** - 基于条件的状态转换系统
  - 数据条件检查器 (DataCondition)
  - 时间条件检查器 (TimeCondition)
  - 复合条件检查器 (CompositeCondition)
  - 委托条件检查器 (DelegateCondition)
  - 条件状态基类 (ConditionalFsmState)

#### 4. 调试工具
- **FsmDebugger.cs** - 状态机调试和监控工具
  - 状态转换历史记录
  - 调试报告生成
  - 可调试状态基类 (DebuggableFsmState)

#### 5. 使用示例
- **FsmExample.cs** - 基础FSM使用示例
  - 敌人AI状态机实现 (待机、巡逻、追击、攻击、返回)
  - 完整的游戏场景应用

- **FsmAdvancedExample.cs** - 高级FSM使用示例  
  - 基于条件的状态转换
  - 角色状态管理 (正常、低血量、危险、恢复、强化)
  - 复杂状态逻辑演示

#### 6. 单元测试
- **FsmTest.cs** - FSM模块完整测试套件
  - 基础功能测试 (创建、启动、销毁)
  - 状态管理测试 (状态检查、获取、转换)
  - 数据管理测试 (设置、获取、删除)
  - 异常处理测试 (边界情况)

### 特点总结

1. **完整性**: 从基础使用到高级特性的全覆盖
2. **实用性**: 提供了实际项目中常用的扩展功能
3. **调试友好**: 内置完善的调试和监控工具
4. **类型安全**: 充分利用泛型和约束保证类型安全
5. **性能优化**: 考虑了GC和内存分配优化
6. **测试覆盖**: 提供完整的单元测试保证代码质量

这些补充代码大大增强了FSM模块的易用性和功能性，使其更适合在实际游戏项目中使用。