# UI实例根节点设置修改说明

## 修改概述

在去除UIGroup管理后，进一步修改了UI创建流程，确保所有UI界面的父节点都设置为`m_InstanceRoot`，统一管理UI界面的层级结构。

## 主要修改内容

### 1. IUIFormHelper接口修改

#### 新增方法
```csharp
// 新的CreateUIForm方法，使用parentTransform参数
IUIForm CreateUIForm(object uiFormInstance, UnityEngine.Transform parentTransform, object userData);

// 废弃的旧方法，保持向后兼容
[System.Obsolete("请使用CreateUIForm(object uiFormInstance, Transform parentTransform, object userData)方法")]
IUIForm CreateUIForm(object uiFormInstance, IUIGroup uiGroup, object userData);
```

### 2. UIFormHelperBase基类修改

#### 方法更新
- 新增抽象方法：`CreateUIForm(object uiFormInstance, Transform parentTransform, object userData)`
- 保留废弃方法：提供向后兼容的虚方法实现

### 3. DefaultUIFormHelper实现修改

#### 核心变更
```csharp
// 新实现：直接使用parentTransform作为父节点
public override IUIForm CreateUIForm(object uiFormInstance, Transform parentTransform, object userData)
{
    GameObject gameObject = uiFormInstance as GameObject;
    if (gameObject == null)
    {
        Log.Error("UI form instance is invalid.");
        return null;
    }

    Transform transform = gameObject.transform;
    transform.SetParent(parentTransform);  // 直接设置传入的父节点
    transform.localScale = Vector3.one;

    return gameObject.GetOrAddComponent<UIForm>();
}
```

### 4. IUIManager接口扩展

#### 新增方法
```csharp
/// <summary>
/// 设置界面实例根节点。
/// </summary>
/// <param name="instanceRoot">界面实例根节点Transform。</param>
void SetInstanceRoot(UnityEngine.Transform instanceRoot);
```

### 5. UIManager实现更新

#### 新增字段
```csharp
private UnityEngine.Transform m_InstanceRoot;
```

#### 新增方法
```csharp
public void SetInstanceRoot(UnityEngine.Transform instanceRoot)
{
    if (instanceRoot == null)
    {
        throw new GameFrameworkException("Instance root is invalid.");
    }

    m_InstanceRoot = instanceRoot;
}
```

#### InternalOpenUIForm修改
```csharp
// 将m_InstanceRoot传递给CreateUIForm方法
IUIForm uiForm = m_UIFormHelper.CreateUIForm(uiFormInstance, m_InstanceRoot, userData);
```

### 6. UIComponent初始化更新

#### 设置实例根节点
```csharp
// 在Start方法中添加
m_UIManager.SetInstanceRoot(m_InstanceRoot);
```

## 工作流程

### UI创建流程
1. `UIComponent`在Start方法中将`m_InstanceRoot`传递给`UIManager`
2. `UIManager`在`InternalOpenUIForm`中创建UI时，将`m_InstanceRoot`传递给`UIFormHelper`
3. `DefaultUIFormHelper`的`CreateUIForm`方法将UI实例的父节点设置为`m_InstanceRoot`
4. 所有UI界面统一在`m_InstanceRoot`下管理

### 层级管理
- **统一父节点**: 所有UI界面都以`m_InstanceRoot`为父节点
- **Order排序**: 通过Order值控制UI在列表中的顺序
- **深度计算**: 根据在列表中的位置自动计算显示深度

## 优势

1. **统一管理**: 所有UI界面都在同一个父节点下，便于管理和调试
2. **清晰的层级**: 不再依赖UIGroup的复杂层级结构
3. **灵活的排序**: 通过Order值可以灵活调整UI显示顺序
4. **向后兼容**: 保留了旧的接口方法，确保现有代码可以正常工作

## 注意事项

1. **初始化顺序**: 确保在使用UIManager创建UI之前，已经调用了`SetInstanceRoot`方法
2. **Transform层级**: 所有UI都会成为`m_InstanceRoot`的子对象
3. **性能考虑**: 大量UI在同一父节点下时，注意Transform操作的性能影响

## 后续可能的优化

1. 可以考虑根据UI类型创建不同的子节点分组
2. 可以添加UI池化机制，复用UI实例
3. 可以实现更复杂的层级管理策略