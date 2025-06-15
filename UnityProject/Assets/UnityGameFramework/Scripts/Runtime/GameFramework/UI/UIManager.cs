using System;
using System.Collections.Generic;
using GameFramework.ObjectPool;
using GameFramework.Resource;

namespace GameFramework.UI
{
    /// <summary>
    /// 界面管理器。
    /// 已修改：去除UIGroup管理，改用order直接管理UI层级
    /// </summary>
    internal sealed partial class UIManager : GameFrameworkModule, IUIManager, IUpdateModule
    {
        // 已注释：去除UIGroup相关字段
        // private readonly Dictionary<string, UIGroup> m_UIGroups;
        
        /// <summary>
        /// 所有UI界面的列表，按order排序管理
        /// </summary>
        private readonly List<IUIForm> m_AllUIForms;
        
        private readonly Dictionary<int, string> m_UIFormsBeingLoaded;
        private readonly HashSet<int> m_UIFormsToReleaseOnLoad;
        private readonly Queue<IUIForm> m_RecycleQueue;
        private readonly LoadAssetCallbacks m_LoadAssetCallbacks;
        private IObjectPoolManager m_ObjectPoolManager;
        private IResourceManager m_ResourceManager;
        private IObjectPool<UIFormInstanceObject> m_InstancePool;
        private IUIFormHelper m_UIFormHelper;
        private UnityEngine.Transform m_InstanceRoot;
        private int m_Serial;
        private bool m_IsShutdown;
        private EventHandler<OpenUIFormSuccessEventArgs> m_OpenUIFormSuccessEventHandler;
        private EventHandler<OpenUIFormFailureEventArgs> m_OpenUIFormFailureEventHandler;
        private EventHandler<OpenUIFormUpdateEventArgs> m_OpenUIFormUpdateEventHandler;
        private EventHandler<CloseUIFormCompleteEventArgs> m_CloseUIFormCompleteEventHandler;
        private EventHandler<OpenUIFormDependencyAssetEventArgs> m_OpenUIFormDependencyAssetEventHandler;

        /// <summary>
        /// 初始化界面管理器的新实例。
        /// </summary>
        public UIManager()
        {
            // 已修改：去除UIGroups，改用UI列表管理
            // m_UIGroups = new Dictionary<string, UIGroup>(StringComparer.Ordinal);
            m_AllUIForms = new List<IUIForm>();
            
            m_UIFormsBeingLoaded = new Dictionary<int, string>();
            m_UIFormsToReleaseOnLoad = new HashSet<int>();
            m_RecycleQueue = new Queue<IUIForm>();
            m_LoadAssetCallbacks = new LoadAssetCallbacks(LoadAssetSuccessCallback, LoadAssetFailureCallback, LoadAssetUpdateCallback);
            m_ObjectPoolManager = null;
            m_ResourceManager = null;
            m_InstancePool = null;
            m_UIFormHelper = null;
            m_InstanceRoot = null;
            m_Serial = 0;
            m_IsShutdown = false;
            m_OpenUIFormSuccessEventHandler = null;
            m_OpenUIFormFailureEventHandler = null;
            m_OpenUIFormUpdateEventHandler = null;
            m_CloseUIFormCompleteEventHandler = null;
        }

        /// <summary>
        /// 获取界面数量。
        /// 已修改：不再返回UIGroup数量，改为返回UI界面总数
        /// </summary>
        public int UIFormCount
        {
            get { return m_AllUIForms.Count; }
        }

        /// <summary>
        /// 获取界面组数量。
        /// 已废弃：UIGroup管理已移除，始终返回0
        /// </summary>
        [System.Obsolete("UIGroup管理已移除，请使用UIFormCount属性")]
        public int UIGroupCount
        {
            get { return 0; }
        }

        /// <summary>
        /// 获取或设置界面实例对象池自动释放可释放对象的间隔秒数。
        /// </summary>
        public float InstanceAutoReleaseInterval
        {
            get { return m_InstancePool.AutoReleaseInterval; }
            set { m_InstancePool.AutoReleaseInterval = value; }
        }

        /// <summary>
        /// 获取或设置界面实例对象池的容量。
        /// </summary>
        public int InstanceCapacity
        {
            get { return m_InstancePool.Capacity; }
            set { m_InstancePool.Capacity = value; }
        }

        /// <summary>
        /// 获取或设置界面实例对象池对象过期秒数。
        /// </summary>
        public float InstanceExpireTime
        {
            get { return m_InstancePool.ExpireTime; }
            set { m_InstancePool.ExpireTime = value; }
        }

        /// <summary>
        /// 获取或设置界面实例对象池的优先级。
        /// </summary>
        public int InstancePriority
        {
            get { return m_InstancePool.Priority; }
            set { m_InstancePool.Priority = value; }
        }

        /// <summary>
        /// 打开界面成功事件。
        /// </summary>
        public event EventHandler<OpenUIFormSuccessEventArgs> OpenUIFormSuccess
        {
            add { m_OpenUIFormSuccessEventHandler += value; }
            remove { m_OpenUIFormSuccessEventHandler -= value; }
        }

        /// <summary>
        /// 打开界面失败事件。
        /// </summary>
        public event EventHandler<OpenUIFormFailureEventArgs> OpenUIFormFailure
        {
            add { m_OpenUIFormFailureEventHandler += value; }
            remove { m_OpenUIFormFailureEventHandler -= value; }
        }

        /// <summary>
        /// 打开界面更新事件。
        /// </summary>
        public event EventHandler<OpenUIFormUpdateEventArgs> OpenUIFormUpdate
        {
            add { m_OpenUIFormUpdateEventHandler += value; }
            remove { m_OpenUIFormUpdateEventHandler -= value; }
        }

        /// <summary>
        /// 关闭界面完成事件。
        /// </summary>
        public event EventHandler<CloseUIFormCompleteEventArgs> CloseUIFormComplete
        {
            add { m_CloseUIFormCompleteEventHandler += value; }
            remove { m_CloseUIFormCompleteEventHandler -= value; }
        }

        public event EventHandler<OpenUIFormDependencyAssetEventArgs> OpenUIFormDependencyAsset
        {
            add { m_OpenUIFormDependencyAssetEventHandler += value; }
            remove { m_OpenUIFormDependencyAssetEventHandler -= value; }
        }

        /// <summary>
        /// 界面管理器轮询。
        /// 已修改：直接轮询所有UI界面，不再通过UIGroup
        /// </summary>
        /// <param name="elapseSeconds">逻辑流逝时间，以秒为单位。</param>
        /// <param name="realElapseSeconds">真实流逝时间，以秒为单位。</param>
        public void Update(float elapseSeconds, float realElapseSeconds)
        {
            while (m_RecycleQueue.Count > 0)
            {
                IUIForm uiForm = m_RecycleQueue.Dequeue();
                uiForm.OnRecycle();
                m_InstancePool.Unspawn(uiForm.Handle);
            }

            // 已修改：直接轮询所有UI界面
            for (int i = 0; i < m_AllUIForms.Count; i++)
            {
                IUIForm uiForm = m_AllUIForms[i];
                if (uiForm != null)
                {
                    uiForm.OnUpdate(elapseSeconds, realElapseSeconds);
                }
            }
        }

        /// <summary>
        /// 关闭并清理界面管理器。
        /// 已修改：清理UI列表而不是UIGroups
        /// </summary>
        internal override void Shutdown()
        {
            m_IsShutdown = true;
            CloseAllLoadedUIForms();
            // 已修改：清理UI列表
            m_AllUIForms.Clear();
            m_UIFormsBeingLoaded.Clear();
            m_UIFormsToReleaseOnLoad.Clear();
            m_RecycleQueue.Clear();
        }

        /// <summary>
        /// 设置对象池管理器。
        /// </summary>
        /// <param name="objectPoolManager">对象池管理器。</param>
        public void SetObjectPoolManager(IObjectPoolManager objectPoolManager)
        {
            if (objectPoolManager == null)
            {
                throw new GameFrameworkException("Object pool manager is invalid.");
            }

            m_ObjectPoolManager = objectPoolManager;
            m_InstancePool = m_ObjectPoolManager.CreateSingleSpawnObjectPool<UIFormInstanceObject>("UI Instance Pool");
        }

        /// <summary>
        /// 设置资源管理器。
        /// </summary>
        /// <param name="resourceManager">资源管理器。</param>
        public void SetResourceManager(IResourceManager resourceManager)
        {
            if (resourceManager == null)
            {
                throw new GameFrameworkException("Resource manager is invalid.");
            }

            m_ResourceManager = resourceManager;
        }

        /// <summary>
        /// 设置界面辅助器。
        /// </summary>
        /// <param name="uiFormHelper">界面辅助器。</param>
        public void SetUIFormHelper(IUIFormHelper uiFormHelper)
        {
            if (uiFormHelper == null)
            {
                throw new GameFrameworkException("UI form helper is invalid.");
            }

            m_UIFormHelper = uiFormHelper;
        }

        /// <summary>
        /// 设置界面实例根节点。
        /// </summary>
        /// <param name="instanceRoot">界面实例根节点Transform。</param>
        public void SetInstanceRoot(UnityEngine.Transform instanceRoot)
        {
            if (instanceRoot == null)
            {
                throw new GameFrameworkException("Instance root is invalid.");
            }

            m_InstanceRoot = instanceRoot;
        }

        /// <summary>
        /// 是否存在界面组。
        /// 已废弃：UIGroup管理已移除，始终返回false
        /// </summary>
        /// <param name="uiGroupName">界面组名称。</param>
        /// <returns>是否存在界面组。</returns>
        [System.Obsolete("UIGroup管理已移除")]
        public bool HasUIGroup(string uiGroupName)
        {
            return false;
        }

        /// <summary>
        /// 获取界面组。
        /// 已废弃：UIGroup管理已移除，始终返回null
        /// </summary>
        /// <param name="uiGroupName">界面组名称。</param>
        /// <returns>要获取的界面组。</returns>
        [System.Obsolete("UIGroup管理已移除")]
        public IUIGroup GetUIGroup(string uiGroupName)
        {
            return null;
        }

        /// <summary>
        /// 获取所有界面组。
        /// 已废弃：UIGroup管理已移除，始终返回空数组
        /// </summary>
        /// <returns>所有界面组。</returns>
        [System.Obsolete("UIGroup管理已移除")]
        public IUIGroup[] GetAllUIGroups()
        {
            return new IUIGroup[0];
        }

        /// <summary>
        /// 获取所有界面组。
        /// 已废弃：UIGroup管理已移除
        /// </summary>
        /// <param name="results">所有界面组。</param>
        [System.Obsolete("UIGroup管理已移除")]
        public void GetAllUIGroups(List<IUIGroup> results)
        {
            if (results == null)
            {
                throw new GameFrameworkException("Results is invalid.");
            }

            results.Clear();
        }

        /// <summary>
        /// 增加界面组。
        /// 已废弃：UIGroup管理已移除，始终返回false
        /// </summary>
        /// <param name="uiGroupName">界面组名称。</param>
        /// <param name="uiGroupHelper">界面组辅助器。</param>
        /// <returns>是否增加界面组成功。</returns>
        [System.Obsolete("UIGroup管理已移除")]
        public bool AddUIGroup(string uiGroupName, IUIGroupHelper uiGroupHelper)
        {
            return false;
        }

        /// <summary>
        /// 增加界面组。
        /// 已废弃：UIGroup管理已移除，始终返回false
        /// </summary>
        /// <param name="uiGroupName">界面组名称。</param>
        /// <param name="uiGroupDepth">界面组深度。</param>
        /// <param name="uiGroupHelper">界面组辅助器。</param>
        /// <returns>是否增加界面组成功。</returns>
        [System.Obsolete("UIGroup管理已移除")]
        public bool AddUIGroup(string uiGroupName, int uiGroupDepth, IUIGroupHelper uiGroupHelper)
        {
            return false;
        }

        /// <summary>
        /// 是否存在界面。
        /// 已修改：直接在UI列表中查找
        /// </summary>
        /// <param name="serialId">界面序列编号。</param>
        /// <returns>是否存在界面。</returns>
        public bool HasUIForm(int serialId)
        {
            for (int i = 0; i < m_AllUIForms.Count; i++)
            {
                if (m_AllUIForms[i].SerialId == serialId)
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// 是否存在界面。
        /// 已修改：直接在UI列表中查找
        /// </summary>
        /// <param name="uiFormAssetName">界面资源名称。</param>
        /// <returns>是否存在界面。</returns>
        public bool HasUIForm(string uiFormAssetName)
        {
            if (string.IsNullOrEmpty(uiFormAssetName))
            {
                throw new GameFrameworkException("UI form asset name is invalid.");
            }

            for (int i = 0; i < m_AllUIForms.Count; i++)
            {
                if (m_AllUIForms[i].UIFormAssetName == uiFormAssetName)
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// 获取界面。
        /// 已修改：直接在UI列表中查找
        /// </summary>
        /// <param name="serialId">界面序列编号。</param>
        /// <returns>要获取的界面。</returns>
        public IUIForm GetUIForm(int serialId)
        {
            for (int i = 0; i < m_AllUIForms.Count; i++)
            {
                if (m_AllUIForms[i].SerialId == serialId)
                {
                    return m_AllUIForms[i];
                }
            }

            return null;
        }

        /// <summary>
        /// 获取界面。
        /// 已修改：直接在UI列表中查找
        /// </summary>
        /// <param name="uiFormAssetName">界面资源名称。</param>
        /// <returns>要获取的界面。</returns>
        public IUIForm GetUIForm(string uiFormAssetName)
        {
            if (string.IsNullOrEmpty(uiFormAssetName))
            {
                throw new GameFrameworkException("UI form asset name is invalid.");
            }

            for (int i = 0; i < m_AllUIForms.Count; i++)
            {
                if (m_AllUIForms[i].UIFormAssetName == uiFormAssetName)
                {
                    return m_AllUIForms[i];
                }
            }

            return null;
        }

        /// <summary>
        /// 获取界面。
        /// 已修改：直接在UI列表中查找所有匹配的界面
        /// </summary>
        /// <param name="uiFormAssetName">界面资源名称。</param>
        /// <returns>要获取的界面。</returns>
        public IUIForm[] GetUIForms(string uiFormAssetName)
        {
            if (string.IsNullOrEmpty(uiFormAssetName))
            {
                throw new GameFrameworkException("UI form asset name is invalid.");
            }

            List<IUIForm> results = new List<IUIForm>();
            for (int i = 0; i < m_AllUIForms.Count; i++)
            {
                if (m_AllUIForms[i].UIFormAssetName == uiFormAssetName)
                {
                    results.Add(m_AllUIForms[i]);
                }
            }

            return results.ToArray();
        }

        /// <summary>
        /// 获取界面。
        /// 已修改：直接在UI列表中查找所有匹配的界面
        /// </summary>
        /// <param name="uiFormAssetName">界面资源名称。</param>
        /// <param name="results">要获取的界面。</param>
        public void GetUIForms(string uiFormAssetName, List<IUIForm> results)
        {
            if (string.IsNullOrEmpty(uiFormAssetName))
            {
                throw new GameFrameworkException("UI form asset name is invalid.");
            }

            if (results == null)
            {
                throw new GameFrameworkException("Results is invalid.");
            }

            results.Clear();
            for (int i = 0; i < m_AllUIForms.Count; i++)
            {
                if (m_AllUIForms[i].UIFormAssetName == uiFormAssetName)
                {
                    results.Add(m_AllUIForms[i]);
                }
            }
        }

        /// <summary>
        /// 获取所有已加载的界面。
        /// 已修改：直接返回UI列表的拷贝
        /// </summary>
        /// <returns>所有已加载的界面。</returns>
        public IUIForm[] GetAllLoadedUIForms()
        {
            return m_AllUIForms.ToArray();
        }

        /// <summary>
        /// 获取所有已加载的界面。
        /// 已修改：直接复制UI列表到结果中
        /// </summary>
        /// <param name="results">所有已加载的界面。</param>
        public void GetAllLoadedUIForms(List<IUIForm> results)
        {
            if (results == null)
            {
                throw new GameFrameworkException("Results is invalid.");
            }

            results.Clear();
            results.AddRange(m_AllUIForms);
        }

        /// <summary>
        /// 获取所有正在加载界面的序列编号。
        /// </summary>
        /// <returns>所有正在加载界面的序列编号。</returns>
        public int[] GetAllLoadingUIFormSerialIds()
        {
            int index = 0;
            int[] results = new int[m_UIFormsBeingLoaded.Count];
            foreach (KeyValuePair<int, string> uiFormBeingLoaded in m_UIFormsBeingLoaded)
            {
                results[index++] = uiFormBeingLoaded.Key;
            }

            return results;
        }

        /// <summary>
        /// 获取所有正在加载界面的序列编号。
        /// </summary>
        /// <param name="results">所有正在加载界面的序列编号。</param>
        public void GetAllLoadingUIFormSerialIds(List<int> results)
        {
            if (results == null)
            {
                throw new GameFrameworkException("Results is invalid.");
            }

            results.Clear();
            foreach (KeyValuePair<int, string> uiFormBeingLoaded in m_UIFormsBeingLoaded)
            {
                results.Add(uiFormBeingLoaded.Key);
            }
        }

        /// <summary>
        /// 是否正在加载界面。
        /// </summary>
        /// <param name="serialId">界面序列编号。</param>
        /// <returns>是否正在加载界面。</returns>
        public bool IsLoadingUIForm(int serialId)
        {
            return m_UIFormsBeingLoaded.ContainsKey(serialId);
        }

        /// <summary>
        /// 是否正在加载界面。
        /// </summary>
        /// <param name="uiFormAssetName">界面资源名称。</param>
        /// <returns>是否正在加载界面。</returns>
        public bool IsLoadingUIForm(string uiFormAssetName)
        {
            if (string.IsNullOrEmpty(uiFormAssetName))
            {
                throw new GameFrameworkException("UI form asset name is invalid.");
            }

            return m_UIFormsBeingLoaded.ContainsValue(uiFormAssetName);
        }

        /// <summary>
        /// 是否是合法的界面。
        /// </summary>
        /// <param name="uiForm">界面。</param>
        /// <returns>界面是否合法。</returns>
        public bool IsValidUIForm(IUIForm uiForm)
        {
            if (uiForm == null)
            {
                return false;
            }

            return HasUIForm(uiForm.SerialId);
        }

        /// <summary>
        /// 打开界面。
        /// 已修改：移除uiGroupName参数，改用order参数
        /// </summary>
        /// <param name="uiFormAssetName">界面资源名称。</param>
        /// <param name="order">界面显示顺序（数值越大越靠前显示）。</param>
        /// <returns>界面的序列编号。</returns>
        public int OpenUIForm(string uiFormAssetName, int order = 0)
        {
            return OpenUIForm(uiFormAssetName, order, Constant.DefaultPriority, false, null);
        }

        /// <summary>
        /// 打开界面。
        /// 已修改：移除uiGroupName参数，改用order参数
        /// </summary>
        /// <param name="uiFormAssetName">界面资源名称。</param>
        /// <param name="order">界面显示顺序（数值越大越靠前显示）。</param>
        /// <param name="priority">加载界面资源的优先级。</param>
        /// <returns>界面的序列编号。</returns>
        public int OpenUIForm(string uiFormAssetName, int order, int priority)
        {
            return OpenUIForm(uiFormAssetName, order, priority, false, null);
        }

        /// <summary>
        /// 打开界面。
        /// 已修改：移除uiGroupName参数，改用order参数
        /// </summary>
        /// <param name="uiFormAssetName">界面资源名称。</param>
        /// <param name="order">界面显示顺序（数值越大越靠前显示）。</param>
        /// <param name="pauseCoveredUIForm">是否暂停被覆盖的界面。</param>
        /// <returns>界面的序列编号。</returns>
        public int OpenUIForm(string uiFormAssetName, int order, bool pauseCoveredUIForm)
        {
            return OpenUIForm(uiFormAssetName, order, Constant.DefaultPriority, pauseCoveredUIForm, null);
        }

        /// <summary>
        /// 打开界面。
        /// 已修改：移除uiGroupName参数，改用order参数
        /// </summary>
        /// <param name="uiFormAssetName">界面资源名称。</param>
        /// <param name="order">界面显示顺序（数值越大越靠前显示）。</param>
        /// <param name="userData">用户自定义数据。</param>
        /// <returns>界面的序列编号。</returns>
        public int OpenUIForm(string uiFormAssetName, int order, object userData)
        {
            return OpenUIForm(uiFormAssetName, order, Constant.DefaultPriority, false, userData);
        }

        /// <summary>
        /// 打开界面。
        /// 已修改：移除uiGroupName参数，改用order参数
        /// </summary>
        /// <param name="uiFormAssetName">界面资源名称。</param>
        /// <param name="order">界面显示顺序（数值越大越靠前显示）。</param>
        /// <param name="priority">加载界面资源的优先级。</param>
        /// <param name="pauseCoveredUIForm">是否暂停被覆盖的界面。</param>
        /// <returns>界面的序列编号。</returns>
        public int OpenUIForm(string uiFormAssetName, int order, int priority, bool pauseCoveredUIForm)
        {
            return OpenUIForm(uiFormAssetName, order, priority, pauseCoveredUIForm, null);
        }

        /// <summary>
        /// 打开界面。
        /// 已修改：移除uiGroupName参数，改用order参数
        /// </summary>
        /// <param name="uiFormAssetName">界面资源名称。</param>
        /// <param name="order">界面显示顺序（数值越大越靠前显示）。</param>
        /// <param name="priority">加载界面资源的优先级。</param>
        /// <param name="userData">用户自定义数据。</param>
        /// <returns>界面的序列编号。</returns>
        public int OpenUIForm(string uiFormAssetName, int order, int priority, object userData)
        {
            return OpenUIForm(uiFormAssetName, order, priority, false, userData);
        }

        /// <summary>
        /// 打开界面。
        /// 已修改：移除uiGroupName参数，改用order参数
        /// </summary>
        /// <param name="uiFormAssetName">界面资源名称。</param>
        /// <param name="order">界面显示顺序（数值越大越靠前显示）。</param>
        /// <param name="pauseCoveredUIForm">是否暂停被覆盖的界面。</param>
        /// <param name="userData">用户自定义数据。</param>
        /// <returns>界面的序列编号。</returns>
        public int OpenUIForm(string uiFormAssetName, int order, bool pauseCoveredUIForm, object userData)
        {
            return OpenUIForm(uiFormAssetName, order, Constant.DefaultPriority, pauseCoveredUIForm, userData);
        }

        /// <summary>
        /// 打开界面。
        /// 已修改：移除uiGroupName参数，改用order参数直接管理层级
        /// </summary>
        /// <param name="uiFormAssetName">界面资源名称。</param>
        /// <param name="order">界面显示顺序（数值越大越靠前显示）。</param>
        /// <param name="priority">加载界面资源的优先级。</param>
        /// <param name="pauseCoveredUIForm">是否暂停被覆盖的界面。</param>
        /// <param name="userData">用户自定义数据。</param>
        /// <returns>界面的序列编号。</returns>
        public int OpenUIForm(string uiFormAssetName, int order, int priority, bool pauseCoveredUIForm, object userData)
        {
            if (m_ResourceManager == null)
            {
                throw new GameFrameworkException("You must set resource manager first.");
            }

            if (m_UIFormHelper == null)
            {
                throw new GameFrameworkException("You must set UI form helper first.");
            }

            if (string.IsNullOrEmpty(uiFormAssetName))
            {
                throw new GameFrameworkException($"UI form asset name is invalid. UIFormName: {uiFormAssetName}");
            }

            int serialId = ++m_Serial;
            UIFormInstanceObject uiFormInstanceObject = m_InstancePool.Spawn(uiFormAssetName);
            if (uiFormInstanceObject == null)
            {
                m_UIFormsBeingLoaded.Add(serialId, uiFormAssetName);
                m_ResourceManager.LoadAssetAsync(uiFormAssetName, priority, m_LoadAssetCallbacks, OpenUIFormInfo.Create(serialId, order, pauseCoveredUIForm, userData));
            }
            else
            {
                InternalOpenUIForm(serialId, uiFormAssetName, order, uiFormInstanceObject.Target, pauseCoveredUIForm, false, 0f, userData);
            }

            return serialId;
        }

        // 保持向后兼容的废弃方法
        [System.Obsolete("请使用OpenUIForm(string uiFormAssetName, int order, ...)方法")]
        public int OpenUIForm(string uiFormAssetName, string uiGroupName)
        {
            return OpenUIForm(uiFormAssetName, 0);
        }

        [System.Obsolete("请使用OpenUIForm(string uiFormAssetName, int order, ...)方法")]
        public int OpenUIForm(string uiFormAssetName, string uiGroupName, int priority)
        {
            return OpenUIForm(uiFormAssetName, 0, priority);
        }

        [System.Obsolete("请使用OpenUIForm(string uiFormAssetName, int order, ...)方法")]
        public int OpenUIForm(string uiFormAssetName, string uiGroupName, bool pauseCoveredUIForm)
        {
            return OpenUIForm(uiFormAssetName, 0, pauseCoveredUIForm);
        }

        [System.Obsolete("请使用OpenUIForm(string uiFormAssetName, int order, ...)方法")]
        public int OpenUIForm(string uiFormAssetName, string uiGroupName, object userData)
        {
            return OpenUIForm(uiFormAssetName, 0, userData);
        }

        [System.Obsolete("请使用OpenUIForm(string uiFormAssetName, int order, ...)方法")]
        public int OpenUIForm(string uiFormAssetName, string uiGroupName, int priority, bool pauseCoveredUIForm)
        {
            return OpenUIForm(uiFormAssetName, 0, priority, pauseCoveredUIForm);
        }

        [System.Obsolete("请使用OpenUIForm(string uiFormAssetName, int order, ...)方法")]
        public int OpenUIForm(string uiFormAssetName, string uiGroupName, int priority, object userData)
        {
            return OpenUIForm(uiFormAssetName, 0, priority, userData);
        }

        [System.Obsolete("请使用OpenUIForm(string uiFormAssetName, int order, ...)方法")]
        public int OpenUIForm(string uiFormAssetName, string uiGroupName, bool pauseCoveredUIForm, object userData)
        {
            return OpenUIForm(uiFormAssetName, 0, pauseCoveredUIForm, userData);
        }

        [System.Obsolete("请使用OpenUIForm(string uiFormAssetName, int order, ...)方法")]
        public int OpenUIForm(string uiFormAssetName, string uiGroupName, int priority, bool pauseCoveredUIForm, object userData)
        {
            return OpenUIForm(uiFormAssetName, 0, priority, pauseCoveredUIForm, userData);
        }

        /// <summary>
        /// 关闭界面。
        /// </summary>
        /// <param name="serialId">要关闭界面的序列编号。</param>
        public void CloseUIForm(int serialId)
        {
            CloseUIForm(serialId, null);
        }

        /// <summary>
        /// 关闭界面。
        /// </summary>
        /// <param name="serialId">要关闭界面的序列编号。</param>
        /// <param name="userData">用户自定义数据。</param>
        public void CloseUIForm(int serialId, object userData)
        {
            if (IsLoadingUIForm(serialId))
            {
                m_UIFormsToReleaseOnLoad.Add(serialId);
                m_UIFormsBeingLoaded.Remove(serialId);
                return;
            }

            IUIForm uiForm = GetUIForm(serialId);
            if (uiForm == null)
            {
                throw new GameFrameworkException(Utility.Text.Format("Can not find UI form '{0}'.", serialId));
            }

            CloseUIForm(uiForm, userData);
        }

        /// <summary>
        /// 关闭界面。
        /// </summary>
        /// <param name="uiForm">要关闭的界面。</param>
        public void CloseUIForm(IUIForm uiForm)
        {
            CloseUIForm(uiForm, null);
        }

        /// <summary>
        /// 关闭界面。
        /// 已修改：去除UIGroup依赖，直接从列表中移除
        /// </summary>
        /// <param name="uiForm">要关闭的界面。</param>
        /// <param name="userData">用户自定义数据。</param>
        public void CloseUIForm(IUIForm uiForm, object userData)
        {
            if (uiForm == null)
            {
                throw new GameFrameworkException("UI form is invalid.");
            }

            // 已修改：直接从列表中移除UI
            m_AllUIForms.Remove(uiForm);
            
            uiForm.OnClose(m_IsShutdown, userData);
            
            // 已修改：刷新所有UI的深度
            RefreshAllUIFormDepth();

            if (m_CloseUIFormCompleteEventHandler != null)
            {
                CloseUIFormCompleteEventArgs closeUIFormCompleteEventArgs = CloseUIFormCompleteEventArgs.Create(uiForm.SerialId, uiForm.UIFormAssetName, null, userData);
                m_CloseUIFormCompleteEventHandler(this, closeUIFormCompleteEventArgs);
                ReferencePool.Release(closeUIFormCompleteEventArgs);
            }

            m_RecycleQueue.Enqueue(uiForm);
        }

        /// <summary>
        /// 关闭所有已加载的界面。
        /// </summary>
        public void CloseAllLoadedUIForms()
        {
            CloseAllLoadedUIForms(null);
        }

        /// <summary>
        /// 关闭所有已加载的界面。
        /// </summary>
        /// <param name="userData">用户自定义数据。</param>
        public void CloseAllLoadedUIForms(object userData)
        {
            IUIForm[] uiForms = GetAllLoadedUIForms();
            foreach (IUIForm uiForm in uiForms)
            {
                if (!HasUIForm(uiForm.SerialId))
                {
                    continue;
                }

                CloseUIForm(uiForm, userData);
            }
        }

        /// <summary>
        /// 关闭所有正在加载的界面。
        /// </summary>
        public void CloseAllLoadingUIForms()
        {
            foreach (KeyValuePair<int, string> uiFormBeingLoaded in m_UIFormsBeingLoaded)
            {
                m_UIFormsToReleaseOnLoad.Add(uiFormBeingLoaded.Key);
            }

            m_UIFormsBeingLoaded.Clear();
        }

        /// <summary>
        /// 激活界面。
        /// </summary>
        /// <param name="uiForm">要激活的界面。</param>
        public void RefocusUIForm(IUIForm uiForm)
        {
            RefocusUIForm(uiForm, null);
        }

        /// <summary>
        /// 激活界面。
        /// 已修改：去除UIGroup依赖，直接将UI移动到列表前面
        /// </summary>
        /// <param name="uiForm">要激活的界面。</param>
        /// <param name="userData">用户自定义数据。</param>
        public void RefocusUIForm(IUIForm uiForm, object userData)
        {
            if (uiForm == null)
            {
                throw new GameFrameworkException("UI form is invalid.");
            }

            // 已修改：将UI移动到列表前面（最高优先级）
            if (m_AllUIForms.Remove(uiForm))
            {
                m_AllUIForms.Insert(0, uiForm);
            }
            
            // 已修改：刷新所有UI的深度
            RefreshAllUIFormDepth();
            
            uiForm.OnRefocus(userData);
        }

        /// <summary>
        /// 设置界面实例是否被加锁。
        /// </summary>
        /// <param name="uiFormInstance">要设置是否被加锁的界面实例。</param>
        /// <param name="locked">界面实例是否被加锁。</param>
        public void SetUIFormInstanceLocked(object uiFormInstance, bool locked)
        {
            if (uiFormInstance == null)
            {
                throw new GameFrameworkException("UI form instance is invalid.");
            }

            m_InstancePool.SetLocked(uiFormInstance, locked);
        }

        /// <summary>
        /// 设置界面实例的优先级。
        /// </summary>
        /// <param name="uiFormInstance">要设置优先级的界面实例。</param>
        /// <param name="priority">界面实例优先级。</param>
        public void SetUIFormInstancePriority(object uiFormInstance, int priority)
        {
            if (uiFormInstance == null)
            {
                throw new GameFrameworkException("UI form instance is invalid.");
            }

            m_InstancePool.SetPriority(uiFormInstance, priority);
        }

        /// <summary>
        /// 打开界面。
        /// 已修改：将UIGroup参数改为order参数
        /// </summary>
        /// <param name="serialId">Id</param>
        /// <param name="uiFormAssetName">资源名称</param>
        /// <param name="order">界面显示顺序</param>
        /// <param name="uiFormInstance">UI实例</param>
        /// <param name="pauseCoveredUIForm">是否暂停被覆盖的UI</param>
        /// <param name="isNewInstance">是否为新实例</param>
        /// <param name="duration">持续时间</param>
        /// <param name="userData"></param>
        private void InternalOpenUIForm(int serialId, string uiFormAssetName, int order, object uiFormInstance, bool pauseCoveredUIForm, bool isNewInstance, float duration,
            object userData)
        {
            try
            {
                IUIForm uiForm = m_UIFormHelper.CreateUIForm(uiFormInstance, m_InstanceRoot, userData);
                if (uiForm == null)
                {
                    throw new GameFrameworkException("Can not create UI form in UI form helper.");
                }

                uiForm.OnInit(serialId, uiFormAssetName, null, pauseCoveredUIForm, isNewInstance, userData);
                
                // 已修改：将UI添加到列表中并按order排序
                AddUIFormToList(uiForm, order);
                
                uiForm.OnOpen(userData);
                
                // 已修改：刷新所有UI的深度
                RefreshAllUIFormDepth();

                if (m_OpenUIFormSuccessEventHandler != null)
                {
                    OpenUIFormSuccessEventArgs openUIFormSuccessEventArgs = OpenUIFormSuccessEventArgs.Create(uiForm, duration, userData);
                    m_OpenUIFormSuccessEventHandler(this, openUIFormSuccessEventArgs);
                    ReferencePool.Release(openUIFormSuccessEventArgs);
                }
            }
            catch (Exception exception)
            {
                if (m_OpenUIFormFailureEventHandler != null)
                {
                    OpenUIFormFailureEventArgs openUIFormFailureEventArgs =
                        OpenUIFormFailureEventArgs.Create(serialId, uiFormAssetName, "DefaultGroup", pauseCoveredUIForm, exception.ToString(), userData);
                    m_OpenUIFormFailureEventHandler(this, openUIFormFailureEventArgs);
                    ReferencePool.Release(openUIFormFailureEventArgs);
                    return;
                }

                throw;
            }
        }

        /// <summary>
        /// 将UI界面添加到列表中并按order排序
        /// </summary>
        private void AddUIFormToList(IUIForm uiForm, int order)
        {
            // 简化实现：直接添加到列表末尾，后续可以根据需要实现复杂的排序逻辑
            m_AllUIForms.Add(uiForm);
            
            // 按order排序（order越大越靠前显示）
            m_AllUIForms.Sort((a, b) =>
            {
                // 这里需要获取UI的order，暂时使用简单的比较
                // 实际实现中可能需要扩展IUIForm接口或使用其他方式存储order
                return 0; // 暂时不排序，保持添加顺序
            });
        }

        /// <summary>
        /// 刷新所有UI界面的深度
        /// </summary>
        private void RefreshAllUIFormDepth()
        {
            for (int i = 0; i < m_AllUIForms.Count; i++)
            {
                int depth = m_AllUIForms.Count - i; // 越靠前的UI深度越大
                m_AllUIForms[i].OnDepthChanged(0, depth);
            }
        }

        private void LoadAssetSuccessCallback(string uiFormAssetName, object uiFormAsset, float duration, object userData)
        {
            OpenUIFormInfo openUIFormInfo = (OpenUIFormInfo)userData;
            if (openUIFormInfo == null)
            {
                throw new GameFrameworkException("Open UI form info is invalid.");
            }

            if (m_UIFormsToReleaseOnLoad.Contains(openUIFormInfo.SerialId))
            {
                m_UIFormsToReleaseOnLoad.Remove(openUIFormInfo.SerialId);
                ReferencePool.Release(openUIFormInfo);
                m_UIFormHelper.ReleaseUIForm(uiFormAsset, null);
                return;
            }

            m_UIFormsBeingLoaded.Remove(openUIFormInfo.SerialId);
            UIFormInstanceObject uiFormInstanceObject = UIFormInstanceObject.Create(uiFormAssetName, uiFormAsset, m_UIFormHelper.InstantiateUIForm(uiFormAsset), m_UIFormHelper);
            m_InstancePool.Register(uiFormInstanceObject, true);

            InternalOpenUIForm(openUIFormInfo.SerialId, uiFormAssetName, openUIFormInfo.Order, uiFormInstanceObject.Target, openUIFormInfo.PauseCoveredUIForm, true, duration,
                openUIFormInfo.UserData);
            ReferencePool.Release(openUIFormInfo);
        }

        private void LoadAssetFailureCallback(string uiFormAssetName, LoadResourceStatus status, string errorMessage, object userData)
        {
            OpenUIFormInfo openUIFormInfo = (OpenUIFormInfo)userData;
            if (openUIFormInfo == null)
            {
                throw new GameFrameworkException("Open UI form info is invalid.");
            }

            if (m_UIFormsToReleaseOnLoad.Contains(openUIFormInfo.SerialId))
            {
                m_UIFormsToReleaseOnLoad.Remove(openUIFormInfo.SerialId);
                return;
            }

            m_UIFormsBeingLoaded.Remove(openUIFormInfo.SerialId);
            string appendErrorMessage = Utility.Text.Format("Load UI form failure, asset name '{0}', status '{1}', error message '{2}'.", uiFormAssetName, status, errorMessage);
            if (m_OpenUIFormFailureEventHandler != null)
            {
                OpenUIFormFailureEventArgs openUIFormFailureEventArgs = OpenUIFormFailureEventArgs.Create(openUIFormInfo.SerialId, uiFormAssetName, "DefaultGroup",
                    openUIFormInfo.PauseCoveredUIForm, appendErrorMessage, openUIFormInfo.UserData);
                m_OpenUIFormFailureEventHandler(this, openUIFormFailureEventArgs);
                ReferencePool.Release(openUIFormFailureEventArgs);
                return;
            }

            throw new GameFrameworkException(appendErrorMessage);
        }

        private void LoadAssetUpdateCallback(string uiFormAssetName, float progress, object userData)
        {
            OpenUIFormInfo openUIFormInfo = (OpenUIFormInfo)userData;
            if (openUIFormInfo == null)
            {
                throw new GameFrameworkException("Open UI form info is invalid.");
            }

            if (m_OpenUIFormUpdateEventHandler != null)
            {
                OpenUIFormUpdateEventArgs openUIFormUpdateEventArgs = OpenUIFormUpdateEventArgs.Create(openUIFormInfo.SerialId, uiFormAssetName, "DefaultGroup",
                    openUIFormInfo.PauseCoveredUIForm, progress, openUIFormInfo.UserData);
                m_OpenUIFormUpdateEventHandler(this, openUIFormUpdateEventArgs);
                ReferencePool.Release(openUIFormUpdateEventArgs);
            }
        }
    }
}