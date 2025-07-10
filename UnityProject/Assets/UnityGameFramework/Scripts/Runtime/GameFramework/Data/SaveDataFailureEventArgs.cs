using System;

namespace GameFramework.Data
{
    /// <summary>
    /// 数据保存失败事件。
    /// </summary>
    public sealed class SaveDataFailureEventArgs : GameFrameworkEventArgs
    {
        /// <summary>
        /// 初始化数据保存失败事件的新实例。
        /// </summary>
        public SaveDataFailureEventArgs()
        {
            DataType = null;
            Id = null;
            FilePath = null;
            ErrorMessage = null;
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
        /// 获取错误信息。
        /// </summary>
        public string ErrorMessage
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
        /// 创建数据保存失败事件。
        /// </summary>
        /// <param name="dataType">数据类型。</param>
        /// <param name="id">数据标识。</param>
        /// <param name="filePath">数据文件路径。</param>
        /// <param name="errorMessage">错误信息。</param>
        /// <param name="userData">用户自定义数据。</param>
        /// <returns>创建的数据保存失败事件。</returns>
        public static SaveDataFailureEventArgs Create(Type dataType, string id, string filePath, string errorMessage, object userData)
        {
            SaveDataFailureEventArgs saveDataFailureEventArgs = ReferencePool.Acquire<SaveDataFailureEventArgs>();
            saveDataFailureEventArgs.DataType = dataType;
            saveDataFailureEventArgs.Id = id;
            saveDataFailureEventArgs.FilePath = filePath;
            saveDataFailureEventArgs.ErrorMessage = errorMessage;
            saveDataFailureEventArgs.UserData = userData;
            return saveDataFailureEventArgs;
        }

        /// <summary>
        /// 清理数据保存失败事件。
        /// </summary>
        public override void Clear()
        {
            DataType = null;
            Id = null;
            FilePath = null;
            ErrorMessage = null;
            UserData = null;
        }
    }
}