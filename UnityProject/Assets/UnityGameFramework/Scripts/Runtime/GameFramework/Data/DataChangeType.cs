namespace GameFramework.Data
{
    /// <summary>
    /// 数据变更类型。
    /// </summary>
    public enum DataChangeType : byte
    {
        /// <summary>
        /// 未知。
        /// </summary>
        Unknown = 0,

        /// <summary>
        /// 设置数据。
        /// </summary>
        Set,

        /// <summary>
        /// 更新数据。
        /// </summary>
        Update,

        /// <summary>
        /// 移除数据。
        /// </summary>
        Remove,

        /// <summary>
        /// 清空所有数据。
        /// </summary>
        Clear,

        /// <summary>
        /// 加载数据。
        /// </summary>
        Load,

        /// <summary>
        /// 保存数据。
        /// </summary>
        Save
    }
}