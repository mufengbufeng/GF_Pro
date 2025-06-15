//------------------------------------------------------------
// Game Framework
// Copyright © 2013-2021 Jiang Yin. All rights reserved.
// Homepage: https://gameframework.cn/
// Feedback: mailto:ellan@gameframework.cn
//------------------------------------------------------------

using GameFramework.UI;
using UnityEngine;

namespace UnityGameFramework.Runtime
{
    /// <summary>
    /// 界面辅助器基类。
    /// </summary>
    public abstract class UIFormHelperBase : MonoBehaviour, IUIFormHelper
    {
        /// <summary>
        /// 实例化界面。
        /// </summary>
        /// <param name="uiFormAsset">要实例化的界面资源。</param>
        /// <returns>实例化后的界面。</returns>
        public abstract object InstantiateUIForm(object uiFormAsset);

        /// <summary>
        /// 创建界面。
        /// 已修改：去除uiGroup参数，改为使用parentTransform
        /// </summary>
        /// <param name="uiFormInstance">界面实例。</param>
        /// <param name="parentTransform">界面的父节点Transform。</param>
        /// <param name="userData">用户自定义数据。</param>
        /// <returns>界面。</returns>
        public abstract IUIForm CreateUIForm(object uiFormInstance, Transform parentTransform, object userData);

        /// <summary>
        /// 创建界面。
        /// 已废弃：请使用带parentTransform参数的重载方法
        /// </summary>
        /// <param name="uiFormInstance">界面实例。</param>
        /// <param name="uiGroup">界面所属的界面组。</param>
        /// <param name="userData">用户自定义数据。</param>
        /// <returns>界面。</returns>
        [System.Obsolete("请使用CreateUIForm(object uiFormInstance, Transform parentTransform, object userData)方法")]
        public virtual IUIForm CreateUIForm(object uiFormInstance, IUIGroup uiGroup, object userData)
        {
            // 向后兼容实现，如果子类没有重写新方法，则尝试调用旧的实现
            return null;
        }

        /// <summary>
        /// 释放界面。
        /// </summary>
        /// <param name="uiFormAsset">要释放的界面资源。</param>
        /// <param name="uiFormInstance">要释放的界面实例。</param>
        public abstract void ReleaseUIForm(object uiFormAsset, object uiFormInstance);
    }
}
