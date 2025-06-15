namespace GameFramework.UI
{
    /// <summary>
    /// 界面辅助器接口。
    /// </summary>
    public interface IUIFormHelper
    {
        /// <summary>
        /// 实例化界面。
        /// </summary>
        /// <param name="uiFormAsset">要实例化的界面资源。</param>
        /// <returns>实例化后的界面。</returns>
        object InstantiateUIForm(object uiFormAsset);

        /// <summary>
        /// 创建界面。
        /// 已修改：去除uiGroup参数，改为使用parentTransform
        /// </summary>
        /// <param name="uiFormInstance">界面实例。</param>
        /// <param name="parentTransform">界面的父节点Transform。</param>
        /// <param name="userData">用户自定义数据。</param>
        /// <returns>界面。</returns>
        IUIForm CreateUIForm(object uiFormInstance, UnityEngine.Transform parentTransform, object userData);

        /// <summary>
        /// 创建界面。
        /// 已废弃：请使用带parentTransform参数的重载方法
        /// </summary>
        /// <param name="uiFormInstance">界面实例。</param>
        /// <param name="uiGroup">界面所属的界面组。</param>
        /// <param name="userData">用户自定义数据。</param>
        /// <returns>界面。</returns>
        [System.Obsolete("请使用CreateUIForm(object uiFormInstance, Transform parentTransform, object userData)方法")]
        IUIForm CreateUIForm(object uiFormInstance, IUIGroup uiGroup, object userData);

        /// <summary>
        /// 释放界面。
        /// </summary>
        /// <param name="uiFormAsset">要释放的界面资源。</param>
        /// <param name="uiFormInstance">要释放的界面实例。</param>
        void ReleaseUIForm(object uiFormAsset, object uiFormInstance);
    }
}
