using System;

namespace GameFramework.Data
{
    /// <summary>
    /// 数据保存成功事件。
    /// </summary>
    public sealed class SaveDataSuccessEventArgs : GameFrameworkEventArgs
    {
        /// <summary>
        /// 初始化数据保存成功事件的新实例。
        /// </summary>
        public SaveDataSuccessEventArgs()
        {
            DataType = null;
            Id = null;
            FilePath = null;
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
        /// 获取数据文件路径。
        /// </summary>
        public string FilePath
        {
            get;
            private set;
        }

        /// <summary>
        /// 获取保存持续时间。
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
        /// 创建数据保存成功事件。
        /// </summary>
        /// <param name="dataType">数据类型。</param>
        /// <param name="id">数据标识。</param>
        /// <param name="filePath">数据文件路径。</param>
        /// <param name="duration">保存持续时间。</param>
        /// <param name="userData">用户自定义数据。</param>
        /// <returns>创建的数据保存成功事件。</returns>
        public static SaveDataSuccessEventArgs Create(Type dataType, string id, string filePath, float duration, object userData)
        {
            SaveDataSuccessEventArgs saveDataSuccessEventArgs = ReferencePool.Acquire<SaveDataSuccessEventArgs>();
            saveDataSuccessEventArgs.DataType = dataType;
            saveDataSuccessEventArgs.Id = id;
            saveDataSuccessEventArgs.FilePath = filePath;
            saveDataSuccessEventArgs.Duration = duration;
            saveDataSuccessEventArgs.UserData = userData;
            return saveDataSuccessEventArgs;
        }

        /// <summary>
        /// 清理数据保存成功事件。
        /// </summary>
        public override void Clear()
        {
            DataType = null;
            Id = null;
            FilePath = null;
            Duration = 0f;
            UserData = null;
        }
    }
}