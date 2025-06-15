# Memory Bank 文档索引

这个文件用于快速索引和查阅相关文档。

## UI框架相关文档

### [UI框架重构说明.md](./UI框架重构说明.md)
- **内容**: 去除UIGroup管理，改用Order管理层级的完整重构说明
- **关键词**: UIGroup, Order, 层级管理, 重构
- **修改范围**: UIManager, IUIManager, UIComponent, OpenUIFormInfo
- **使用场景**: 了解UI框架的整体重构思路和实现细节

### [UI实例根节点设置修改说明.md](./UI实例根节点设置修改说明.md)
- **内容**: 确保UI界面父节点设置为m_InstanceRoot的修改说明
- **关键词**: m_InstanceRoot, 父节点, CreateUIForm, UIFormHelper
- **修改范围**: IUIFormHelper, UIFormHelperBase, DefaultUIFormHelper, UIManager, UIComponent
- **使用场景**: 了解UI实例创建时父节点设置的实现细节

## 快速查阅指南

### 如果你想了解：

#### UI层级管理
- 查阅: [UI框架重构说明.md](./UI框架重构说明.md)
- 关注: Order系统、深度管理、UIGroup替换

#### UI创建流程
- 查阅: [UI实例根节点设置修改说明.md](./UI实例根节点设置修改说明.md)
- 关注: CreateUIForm方法、父节点设置、实例根节点

#### 向后兼容性
- 查阅: 两个文档都有相关说明
- 关注: 废弃方法标记、兼容性实现

#### 使用示例
- 查阅: [UI框架重构说明.md](./UI框架重构说明.md) 的使用示例部分
- 关注: 新旧API对比、迁移指南

## 文档更新记录

- 2025/6/15: 创建UI框架重构说明文档
- 2025/6/15: 创建UI实例根节点设置修改说明文档
- 2025/6/15: 创建memoryMap.md索引文档