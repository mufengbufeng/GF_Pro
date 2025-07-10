using System;
using System.Collections.Generic;
using System.IO;

namespace GameFramework.Data
{
    /// <summary>
    /// 数据管理器。
    /// </summary>
    internal sealed class DataManager : GameFrameworkModule, IDataManager, IUpdateModule
    {
        private readonly Dictionary<Type, Dictionary<string, DataBase>> m_DataGroups;
        private readonly List<DataBase> m_CachedResults;
        private EventHandler<DataChangedEventArgs> m_DataChangedEventHandler;
        private EventHandler<LoadDataSuccessEventArgs> m_LoadDataSuccessEventHandler;
        private EventHandler<LoadDataFailureEventArgs> m_LoadDataFailureEventHandler;
        private EventHandler<SaveDataSuccessEventArgs> m_SaveDataSuccessEventHandler;
        private EventHandler<SaveDataFailureEventArgs> m_SaveDataFailureEventHandler;

        /// <summary>
        /// 初始化数据管理器的新实例。
        /// </summary>
        public DataManager()
        {
            m_DataGroups = new Dictionary<Type, Dictionary<string, DataBase>>();
            m_CachedResults = new List<DataBase>();
            m_DataChangedEventHandler = null;
            m_LoadDataSuccessEventHandler = null;
            m_LoadDataFailureEventHandler = null;
            m_SaveDataSuccessEventHandler = null;
            m_SaveDataFailureEventHandler = null;
        }

        /// <summary>
        /// 获取数据数量。
        /// </summary>
        public int DataCount
        {
            get
            {
                int count = 0;
                foreach (var dataGroup in m_DataGroups.Values)
                {
                    count += dataGroup.Count;
                }
                return count;
            }
        }

        /// <summary>
        /// 数据变更事件。
        /// </summary>
        public event EventHandler<DataChangedEventArgs> DataChanged
        {
            add
            {
                m_DataChangedEventHandler += value;
            }
            remove
            {
                m_DataChangedEventHandler -= value;
            }
        }

        /// <summary>
        /// 数据加载成功事件。
        /// </summary>
        public event EventHandler<LoadDataSuccessEventArgs> LoadDataSuccess
        {
            add
            {
                m_LoadDataSuccessEventHandler += value;
            }
            remove
            {
                m_LoadDataSuccessEventHandler -= value;
            }
        }

        /// <summary>
        /// 数据加载失败事件。
        /// </summary>
        public event EventHandler<LoadDataFailureEventArgs> LoadDataFailure
        {
            add
            {
                m_LoadDataFailureEventHandler += value;
            }
            remove
            {
                m_LoadDataFailureEventHandler -= value;
            }
        }

        /// <summary>
        /// 数据保存成功事件。
        /// </summary>
        public event EventHandler<SaveDataSuccessEventArgs> SaveDataSuccess
        {
            add
            {
                m_SaveDataSuccessEventHandler += value;
            }
            remove
            {
                m_SaveDataSuccessEventHandler -= value;
            }
        }

        /// <summary>
        /// 数据保存失败事件。
        /// </summary>
        public event EventHandler<SaveDataFailureEventArgs> SaveDataFailure
        {
            add
            {
                m_SaveDataFailureEventHandler += value;
            }
            remove
            {
                m_SaveDataFailureEventHandler -= value;
            }
        }

        /// <summary>
        /// 数据管理器轮询。
        /// </summary>
        /// <param name="elapseSeconds">逻辑流逝时间，以秒为单位。</param>
        /// <param name="realElapseSeconds">真实流逝时间，以秒为单位。</param>
        public void Update(float elapseSeconds, float realElapseSeconds)
        {
        }

        /// <summary>
        /// 关闭并清理数据管理器。
        /// </summary>
        internal override void Shutdown()
        {
            RemoveAllData();
            m_CachedResults.Clear();
        }

        /// <summary>
        /// 检查是否存在指定类型的数据。
        /// </summary>
        /// <typeparam name="T">数据类型。</typeparam>
        /// <returns>是否存在指定类型的数据。</returns>
        public bool HasData<T>() where T : DataBase
        {
            return HasData<T>(string.Empty);
        }

        /// <summary>
        /// 检查是否存在指定类型和标识的数据。
        /// </summary>
        /// <typeparam name="T">数据类型。</typeparam>
        /// <param name="id">数据标识。</param>
        /// <returns>是否存在指定类型和标识的数据。</returns>
        public bool HasData<T>(string id) where T : DataBase
        {
            Type dataType = typeof(T);
            if (!m_DataGroups.TryGetValue(dataType, out Dictionary<string, DataBase> dataGroup))
            {
                return false;
            }

            return dataGroup.ContainsKey(id ?? string.Empty);
        }

        /// <summary>
        /// 获取指定类型的数据。
        /// </summary>
        /// <typeparam name="T">数据类型。</typeparam>
        /// <returns>要获取的数据。</returns>
        public T GetData<T>() where T : DataBase
        {
            return GetData<T>(string.Empty);
        }

        /// <summary>
        /// 获取指定类型和标识的数据。
        /// </summary>
        /// <typeparam name="T">数据类型。</typeparam>
        /// <param name="id">数据标识。</param>
        /// <returns>要获取的数据。</returns>
        public T GetData<T>(string id) where T : DataBase
        {
            Type dataType = typeof(T);
            if (!m_DataGroups.TryGetValue(dataType, out Dictionary<string, DataBase> dataGroup))
            {
                return null;
            }

            if (dataGroup.TryGetValue(id ?? string.Empty, out DataBase data))
            {
                return (T)data;
            }

            return null;
        }

        /// <summary>
        /// 获取指定类型的所有数据。
        /// </summary>
        /// <typeparam name="T">数据类型。</typeparam>
        /// <returns>指定类型的所有数据。</returns>
        public T[] GetAllData<T>() where T : DataBase
        {
            List<T> cachedTypedResults = new List<T>();
            GetAllData<T>(cachedTypedResults);
            T[] results = new T[cachedTypedResults.Count];
            for (int i = 0; i < cachedTypedResults.Count; i++)
            {
                results[i] = cachedTypedResults[i];
            }
            return results;
        }

        /// <summary>
        /// 获取指定类型的所有数据。
        /// </summary>
        /// <typeparam name="T">数据类型。</typeparam>
        /// <param name="results">指定类型的所有数据。</param>
        public void GetAllData<T>(List<T> results) where T : DataBase
        {
            if (results == null)
            {
                throw new GameFrameworkException("Results is invalid.");
            }

            results.Clear();
            Type dataType = typeof(T);
            if (!m_DataGroups.TryGetValue(dataType, out Dictionary<string, DataBase> dataGroup))
            {
                return;
            }

            foreach (DataBase data in dataGroup.Values)
            {
                results.Add((T)data);
            }
        }

        /// <summary>
        /// 获取所有数据。
        /// </summary>
        /// <returns>所有数据。</returns>
        public DataBase[] GetAllData()
        {
            m_CachedResults.Clear();
            GetAllData(m_CachedResults);
            DataBase[] results = new DataBase[m_CachedResults.Count];
            for (int i = 0; i < m_CachedResults.Count; i++)
            {
                results[i] = m_CachedResults[i];
            }
            m_CachedResults.Clear();
            return results;
        }

        /// <summary>
        /// 获取所有数据。
        /// </summary>
        /// <param name="results">所有数据。</param>
        public void GetAllData(List<DataBase> results)
        {
            if (results == null)
            {
                throw new GameFrameworkException("Results is invalid.");
            }

            results.Clear();
            foreach (var dataGroup in m_DataGroups.Values)
            {
                foreach (DataBase data in dataGroup.Values)
                {
                    results.Add(data);
                }
            }
        }

        /// <summary>
        /// 设置数据。
        /// </summary>
        /// <typeparam name="T">数据类型。</typeparam>
        /// <param name="data">要设置的数据。</param>
        public void SetData<T>(T data) where T : DataBase
        {
            SetData(data?.Id ?? string.Empty, data);
        }

        /// <summary>
        /// 设置数据。
        /// </summary>
        /// <typeparam name="T">数据类型。</typeparam>
        /// <param name="id">数据标识。</param>
        /// <param name="data">要设置的数据。</param>
        public void SetData<T>(string id, T data) where T : DataBase
        {
            if (data == null)
            {
                throw new GameFrameworkException("Data is invalid.");
            }

            Type dataType = typeof(T);
            string dataId = id ?? string.Empty;

            if (!m_DataGroups.TryGetValue(dataType, out Dictionary<string, DataBase> dataGroup))
            {
                dataGroup = new Dictionary<string, DataBase>();
                m_DataGroups.Add(dataType, dataGroup);
            }

            DataChangeType changeType = dataGroup.ContainsKey(dataId) ? DataChangeType.Update : DataChangeType.Set;
            dataGroup[dataId] = data;

            data.MarkClean();

            if (m_DataChangedEventHandler != null)
            {
                DataChangedEventArgs eventArgs = DataChangedEventArgs.Create(dataType, dataId, data, changeType);
                m_DataChangedEventHandler(this, eventArgs);
                ReferencePool.Release(eventArgs);
            }
        }

        /// <summary>
        /// 移除指定类型的数据。
        /// </summary>
        /// <typeparam name="T">数据类型。</typeparam>
        /// <returns>是否移除数据成功。</returns>
        public bool RemoveData<T>() where T : DataBase
        {
            return RemoveData<T>(string.Empty);
        }

        /// <summary>
        /// 移除指定类型和标识的数据。
        /// </summary>
        /// <typeparam name="T">数据类型。</typeparam>
        /// <param name="id">数据标识。</param>
        /// <returns>是否移除数据成功。</returns>
        public bool RemoveData<T>(string id) where T : DataBase
        {
            Type dataType = typeof(T);
            string dataId = id ?? string.Empty;

            if (!m_DataGroups.TryGetValue(dataType, out Dictionary<string, DataBase> dataGroup))
            {
                return false;
            }

            if (!dataGroup.TryGetValue(dataId, out DataBase data))
            {
                return false;
            }

            dataGroup.Remove(dataId);
            if (dataGroup.Count == 0)
            {
                m_DataGroups.Remove(dataType);
            }

            ReferencePool.Release(data);

            if (m_DataChangedEventHandler != null)
            {
                DataChangedEventArgs eventArgs = DataChangedEventArgs.Create(dataType, dataId, null, DataChangeType.Remove);
                m_DataChangedEventHandler(this, eventArgs);
                ReferencePool.Release(eventArgs);
            }

            return true;
        }

        /// <summary>
        /// 移除所有数据。
        /// </summary>
        public void RemoveAllData()
        {
            foreach (var dataGroup in m_DataGroups.Values)
            {
                foreach (DataBase data in dataGroup.Values)
                {
                    ReferencePool.Release(data);
                }
            }

            m_DataGroups.Clear();

            if (m_DataChangedEventHandler != null)
            {
                DataChangedEventArgs eventArgs = DataChangedEventArgs.Create(null, null, null, DataChangeType.Clear);
                m_DataChangedEventHandler(this, eventArgs);
                ReferencePool.Release(eventArgs);
            }
        }

        /// <summary>
        /// 从文件加载数据。
        /// </summary>
        /// <typeparam name="T">数据类型。</typeparam>
        /// <param name="filePath">文件路径。</param>
        public void LoadDataFromFile<T>(string filePath) where T : DataBase, new()
        {
            LoadDataFromFile<T>(string.Empty, filePath);
        }

        /// <summary>
        /// 从文件加载数据。
        /// </summary>
        /// <typeparam name="T">数据类型。</typeparam>
        /// <param name="id">数据标识。</param>
        /// <param name="filePath">文件路径。</param>
        public void LoadDataFromFile<T>(string id, string filePath) where T : DataBase, new()
        {
            DateTime startTime = DateTime.UtcNow;
            Type dataType = typeof(T);
            string dataId = id ?? string.Empty;

            try
            {
                if (string.IsNullOrEmpty(filePath))
                {
                    throw new GameFrameworkException("File path is invalid.");
                }

                if (!File.Exists(filePath))
                {
                    throw new GameFrameworkException(Utility.Text.Format("File '{0}' does not exist.", filePath));
                }

                byte[] bytes = File.ReadAllBytes(filePath);
                LoadDataFromBytes<T>(dataId, bytes);

                float duration = (float)(DateTime.UtcNow - startTime).TotalSeconds;
                T data = GetData<T>(dataId);

                if (m_LoadDataSuccessEventHandler != null)
                {
                    LoadDataSuccessEventArgs eventArgs = LoadDataSuccessEventArgs.Create(dataType, dataId, data, duration, filePath);
                    m_LoadDataSuccessEventHandler(this, eventArgs);
                    ReferencePool.Release(eventArgs);
                }
            }
            catch (Exception exception)
            {
                if (m_LoadDataFailureEventHandler != null)
                {
                    LoadDataFailureEventArgs eventArgs = LoadDataFailureEventArgs.Create(dataType, dataId, filePath, exception.Message, filePath);
                    m_LoadDataFailureEventHandler(this, eventArgs);
                    ReferencePool.Release(eventArgs);
                }
            }
        }

        /// <summary>
        /// 保存数据到文件。
        /// </summary>
        /// <typeparam name="T">数据类型。</typeparam>
        /// <param name="filePath">文件路径。</param>
        public void SaveDataToFile<T>(string filePath) where T : DataBase
        {
            SaveDataToFile<T>(string.Empty, filePath);
        }

        /// <summary>
        /// 保存数据到文件。
        /// </summary>
        /// <typeparam name="T">数据类型。</typeparam>
        /// <param name="id">数据标识。</param>
        /// <param name="filePath">文件路径。</param>
        public void SaveDataToFile<T>(string id, string filePath) where T : DataBase
        {
            DateTime startTime = DateTime.UtcNow;
            Type dataType = typeof(T);
            string dataId = id ?? string.Empty;

            try
            {
                if (string.IsNullOrEmpty(filePath))
                {
                    throw new GameFrameworkException("File path is invalid.");
                }

                T data = GetData<T>(dataId);
                if (data == null)
                {
                    throw new GameFrameworkException(Utility.Text.Format("Data '{0}' with id '{1}' does not exist.", dataType.FullName, dataId));
                }

                byte[] bytes = SaveDataToBytes<T>(dataId);
                string directory = Path.GetDirectoryName(filePath);
                if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
                {
                    Directory.CreateDirectory(directory);
                }

                File.WriteAllBytes(filePath, bytes);
                data.MarkClean();

                float duration = (float)(DateTime.UtcNow - startTime).TotalSeconds;

                if (m_SaveDataSuccessEventHandler != null)
                {
                    SaveDataSuccessEventArgs eventArgs = SaveDataSuccessEventArgs.Create(dataType, dataId, filePath, duration, filePath);
                    m_SaveDataSuccessEventHandler(this, eventArgs);
                    ReferencePool.Release(eventArgs);
                }
            }
            catch (Exception exception)
            {
                if (m_SaveDataFailureEventHandler != null)
                {
                    SaveDataFailureEventArgs eventArgs = SaveDataFailureEventArgs.Create(dataType, dataId, filePath, exception.Message, filePath);
                    m_SaveDataFailureEventHandler(this, eventArgs);
                    ReferencePool.Release(eventArgs);
                }
            }
        }

        /// <summary>
        /// 从字节数组加载数据。
        /// </summary>
        /// <typeparam name="T">数据类型。</typeparam>
        /// <param name="bytes">字节数组。</param>
        public void LoadDataFromBytes<T>(byte[] bytes) where T : DataBase, new()
        {
            LoadDataFromBytes<T>(string.Empty, bytes);
        }

        /// <summary>
        /// 从字节数组加载数据。
        /// </summary>
        /// <typeparam name="T">数据类型。</typeparam>
        /// <param name="id">数据标识。</param>
        /// <param name="bytes">字节数组。</param>
        public void LoadDataFromBytes<T>(string id, byte[] bytes) where T : DataBase, new()
        {
            if (bytes == null || bytes.Length == 0)
            {
                throw new GameFrameworkException("Bytes is invalid.");
            }

            T data = ReferencePool.Acquire<T>();
            data.Deserialize(bytes);
            SetData(id, data);

            if (m_DataChangedEventHandler != null)
            {
                DataChangedEventArgs eventArgs = DataChangedEventArgs.Create(typeof(T), id ?? string.Empty, data, DataChangeType.Load);
                m_DataChangedEventHandler(this, eventArgs);
                ReferencePool.Release(eventArgs);
            }
        }

        /// <summary>
        /// 将数据转换为字节数组。
        /// </summary>
        /// <typeparam name="T">数据类型。</typeparam>
        /// <returns>字节数组。</returns>
        public byte[] SaveDataToBytes<T>() where T : DataBase
        {
            return SaveDataToBytes<T>(string.Empty);
        }

        /// <summary>
        /// 将数据转换为字节数组。
        /// </summary>
        /// <typeparam name="T">数据类型。</typeparam>
        /// <param name="id">数据标识。</param>
        /// <returns>字节数组。</returns>
        public byte[] SaveDataToBytes<T>(string id) where T : DataBase
        {
            T data = GetData<T>(id);
            if (data == null)
            {
                throw new GameFrameworkException(Utility.Text.Format("Data '{0}' with id '{1}' does not exist.", typeof(T).FullName, id ?? string.Empty));
            }

            return data.Serialize();
        }
    }
}