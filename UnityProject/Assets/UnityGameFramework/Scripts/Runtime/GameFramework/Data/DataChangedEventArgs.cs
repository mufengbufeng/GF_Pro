using System;

namespace GameFramework.Data
{
    /// <summary>
    /// 数据变更事件。
    /// </summary>
    public sealed class DataChangedEventArgs : GameFrameworkEventArgs
    {
        /// <summary>
        /// 初始化数据变更事件的新实例。
        /// </summary>
        public DataChangedEventArgs()
        {
            DataType = null;
            Id = null;
            Data = null;
            ChangeType = DataChangeType.Unknown;
        }

        /// <summary>
        /// 获取数据类型。
        /// </summary>
        public Type DataType
        {
            get;
            private set;
        }

        /// <summary>
        /// 获取数据标识。
        /// </summary>
        public string Id
        {
            get;
            private set;
        }

        /// <summary>
        /// 获取数据。
        /// </summary>
        public DataBase Data
        {
            get;
            private set;
        }

        /// <summary>
        /// 获取数据变更类型。
        /// </summary>
        public DataChangeType ChangeType
        {
            get;
            private set;
        }

        /// <summary>
        /// 创建数据变更事件。
        /// </summary>
        /// <param name="dataType">数据类型。</param>
        /// <param name="id">数据标识。</param>
        /// <param name="data">数据。</param>
        /// <param name="changeType">数据变更类型。</param>
        /// <returns>创建的数据变更事件。</returns>
        public static DataChangedEventArgs Create(Type dataType, string id, DataBase data, DataChangeType changeType)
        {
            DataChangedEventArgs dataChangedEventArgs = ReferencePool.Acquire<DataChangedEventArgs>();
            dataChangedEventArgs.DataType = dataType;
            dataChangedEventArgs.Id = id;
            dataChangedEventArgs.Data = data;
            dataChangedEventArgs.ChangeType = changeType;
            return dataChangedEventArgs;
        }

        /// <summary>
        /// 清理数据变更事件。
        /// </summary>
        public override void Clear()
        {
            DataType = null;
            Id = null;
            Data = null;
            ChangeType = DataChangeType.Unknown;
        }
    }
}