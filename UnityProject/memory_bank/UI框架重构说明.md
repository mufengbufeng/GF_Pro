# UI框架重构说明 - 去除UIGroup管理，改用Order管理层级

## 修改概述

本次重构将Unity游戏框架中的UI管理系统从基于UIGroup的层级管理改为基于order的直接层级管理，简化了UI系统的复杂度，提高了使用效率。

## 主要修改内容

### 1. UIManager.cs 主要修改

#### 数据结构变更
```csharp
// 原有的UIGroup字典管理
// private readonly Dictionary<string, UIGroup> m_UIGroups;

// 改为直接的UI列表管理
private readonly List<IUIForm> m_AllUIForms;
```

#### 新增方法
- `AddUIFormToList(IUIForm uiForm, int order)`: 将UI添加到列表并按order排序
- `RefreshAllUIFormDepth()`: 刷新所有UI的深度

#### OpenUIForm方法签名变更
```csharp
// 原方法签名
public int OpenUIForm(string uiFormAssetName, string uiGroupName, int priority, bool pauseCoveredUIForm, object userData)

// 新方法签名  
public int OpenUIForm(string uiFormAssetName, int order, int priority, bool pauseCoveredUIForm, object userData)
```

#### InternalOpenUIForm方法变更
```csharp
// 原方法签名
private void InternalOpenUIForm(int serialId, string uiFormAssetName, UIGroup uiGroup, object uiFormInstance, bool pauseCoveredUIForm, bool isNewInstance, float duration, object userData)

// 新方法签名
private void InternalOpenUIForm(int serialId, string uiFormAssetName, int order, object uiFormInstance, bool pauseCoveredUIForm, bool isNewInstance, float duration, object userData)
```

### 2. UIManager.OpenUIFormInfo.cs 修改

#### 属性变更
```csharp
// 原属性
public UIGroup UIGroup { get; }

// 新属性
public int Order { get; }
```

#### Create方法变更
```csharp
// 原方法
public static OpenUIFormInfo Create(int serialId, UIGroup uiGroup, bool pauseCoveredUIForm, object userData)

// 新方法
public static OpenUIFormInfo Create(int serialId, int order, bool pauseCoveredUIForm, object userData)
```

### 3. IUIManager.cs 接口修改

#### 新增属性
```csharp
// 新增UI数量属性
int UIFormCount { get; }

// 原UIGroupCount属性标记为废弃
[System.Obsolete("UIGroup管理已移除，请使用UIFormCount属性")]
int UIGroupCount { get; }
```

#### OpenUIForm方法更新
所有OpenUIForm方法的第二个参数从`string uiGroupName`改为`int order`，原方法标记为废弃并提供向后兼容。

### 4. UIComponent.cs 修改

#### 对应的方法签名更新
所有OpenUIForm相关方法都更新为使用order参数，并提供废弃的向后兼容方法。

#### 属性更新
```csharp
// 新增
public int UIFormCount { get; }

// 废弃
[System.Obsolete("UIGroup管理已移除，请使用UIFormCount属性")]
public int UIGroupCount { get; }
```

## 核心设计理念

### Order管理系统
- **Order值**: 数值越大的UI显示层级越靠前
- **自动排序**: UI按order值自动排序，无需手动管理UIGroup
- **深度计算**: 系统自动根据UI在列表中的位置计算显示深度

### 简化的层级管理
1. **直接管理**: 不再需要创建和管理UIGroup，直接通过order值控制层级
2. **动态排序**: 新添加的UI会根据order值自动插入到正确位置
3. **灵活调整**: 可以通过RefocusUIForm将UI移动到最前面

## 向后兼容性

### 废弃方法
所有原有的基于UIGroup的方法都标记为`[System.Obsolete]`，但仍然可用，会自动转换为order=0的调用。

### 渐进式迁移
现有代码可以继续使用，但会收到编译器警告，建议逐步迁移到新的API。

## 使用示例

### 原用法
```csharp
// 原来需要指定UIGroup
uiComponent.OpenUIForm("MainMenu", "UI", priority, false, null);
```

### 新用法
```csharp
// 现在直接指定order，数值越大越靠前显示
uiComponent.OpenUIForm("MainMenu", 100, priority, false, null);

// 背景UI使用较小的order值
uiComponent.OpenUIForm("Background", 0, priority, false, null);

// 弹窗UI使用较大的order值
uiComponent.OpenUIForm("Dialog", 1000, priority, false, null);
```

## 注意事项

1. **Order值规划**: 建议为不同类型的UI预留order值范围，如背景UI(0-99)、普通UI(100-999)、弹窗UI(1000+)
2. **深度刷新**: 每次添加或移除UI都会自动刷新所有UI的深度，确保显示正确
3. **性能考虑**: 大量UI频繁添加删除时，排序操作可能有性能影响，建议合理规划UI的生命周期

## 优势

1. **简化架构**: 去除了UIGroup的复杂层级，使用更直观的order数值管理
2. **易于理解**: order值直接对应显示优先级，更容易理解和使用
3. **灵活性**: 可以随时调整UI的显示顺序，无需重新配置UIGroup
4. **减少配置**: 不再需要预先创建和配置UIGroup

## 后续建议

1. 可以考虑为IUIForm接口添加Order属性，实现更完整的order管理
2. 可以添加order值的常量定义，方便团队协作
3. 考虑添加order值的验证和冲突检测机制