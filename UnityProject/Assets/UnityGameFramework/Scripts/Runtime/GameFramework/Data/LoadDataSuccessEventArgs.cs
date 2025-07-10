using System;

namespace GameFramework.Data
{
    /// <summary>
    /// 数据加载成功事件。
    /// </summary>
    public sealed class LoadDataSuccessEventArgs : GameFrameworkEventArgs
    {
        /// <summary>
        /// 初始化数据加载成功事件的新实例。
        /// </summary>
        public LoadDataSuccessEventArgs()
        {
            DataType = null;
            Id = null;
            Data = null;
            Duration = 0f;
            UserData = null;
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
        /// 获取加载持续时间。
        /// </summary>
        public float Duration
        {
            get;
            private set;
        }

        /// <summary>
        /// 获取用户自定义数据。
        /// </summary>
        public object UserData
        {
            get;
            private set;
        }

        /// <summary>
        /// 创建数据加载成功事件。
        /// </summary>
        /// <param name="dataType">数据类型。</param>
        /// <param name="id">数据标识。</param>
        /// <param name="data">数据。</param>
        /// <param name="duration">加载持续时间。</param>
        /// <param name="userData">用户自定义数据。</param>
        /// <returns>创建的数据加载成功事件。</returns>
        public static LoadDataSuccessEventArgs Create(Type dataType, string id, DataBase data, float duration, object userData)
        {
            LoadDataSuccessEventArgs loadDataSuccessEventArgs = ReferencePool.Acquire<LoadDataSuccessEventArgs>();
            loadDataSuccessEventArgs.DataType = dataType;
            loadDataSuccessEventArgs.Id = id;
            loadDataSuccessEventArgs.Data = data;
            loadDataSuccessEventArgs.Duration = duration;
            loadDataSuccessEventArgs.UserData = userData;
            return loadDataSuccessEventArgs;
        }

        /// <summary>
        /// 清理数据加载成功事件。
        /// </summary>
        public override void Clear()
        {
            DataType = null;
            Id = null;
            Data = null;
            Duration = 0f;
            UserData = null;
        }
    }
}