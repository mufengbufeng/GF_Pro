using System;
using System.Collections.Generic;
using System.IO;
using GameFramework;
using GameFramework.Data;
using UnityEngine;

namespace UnityGameFramework.Runtime
{
    /// <summary>
    /// 数据组件。
    /// </summary>
    [DisallowMultipleComponent]
    [AddComponentMenu("Game Framework/Data")]
    public sealed class DataComponent : GameFrameworkComponent
    {
        private IDataManager m_DataManager = null;

        /// <summary>
        /// 获取数据数量。
        /// </summary>
        public int DataCount
        {
            get
            {
                return m_DataManager.DataCount;
            }
        }

        /// <summary>
        /// 数据变更事件。
        /// </summary>
        public event EventHandler<DataChangedEventArgs> DataChanged
        {
            add
            {
                m_DataManager.DataChanged += value;
            }
            remove
            {
                m_DataManager.DataChanged -= value;
            }
        }

        /// <summary>
        /// 数据加载成功事件。
        /// </summary>
        public event EventHandler<LoadDataSuccessEventArgs> LoadDataSuccess
        {
            add
            {
                m_DataManager.LoadDataSuccess += value;
            }
            remove
            {
                m_DataManager.LoadDataSuccess -= value;
            }
        }

        /// <summary>
        /// 数据加载失败事件。
        /// </summary>
        public event EventHandler<LoadDataFailureEventArgs> LoadDataFailure
        {
            add
            {
                m_DataManager.LoadDataFailure += value;
            }
            remove
            {
                m_DataManager.LoadDataFailure -= value;
            }
        }

        /// <summary>
        /// 数据保存成功事件。
        /// </summary>
        public event EventHandler<SaveDataSuccessEventArgs> SaveDataSuccess
        {
            add
            {
                m_DataManager.SaveDataSuccess += value;
            }
            remove
            {
                m_DataManager.SaveDataSuccess -= value;
            }
        }

        /// <summary>
        /// 数据保存失败事件。
        /// </summary>
        public event EventHandler<SaveDataFailureEventArgs> SaveDataFailure
        {
            add
            {
                m_DataManager.SaveDataFailure += value;
            }
            remove
            {
                m_DataManager.SaveDataFailure -= value;
            }
        }

        /// <summary>
        /// 游戏框架组件初始化。
        /// </summary>
        protected override void Awake()
        {
            base.Awake();

            m_DataManager = GameFrameworkSystem.GetModule<IDataManager>();
            if (m_DataManager == null)
            {
                Log.Fatal("Data manager is invalid.");
                return;
            }
        }

        /// <summary>
        /// 检查是否存在指定类型的数据。
        /// </summary>
        /// <typeparam name="T">数据类型。</typeparam>
        /// <returns>是否存在指定类型的数据。</returns>
        public bool HasData<T>() where T : DataBase
        {
            return m_DataManager.HasData<T>();
        }

        /// <summary>
        /// 检查是否存在指定类型和标识的数据。
        /// </summary>
        /// <typeparam name="T">数据类型。</typeparam>
        /// <param name="id">数据标识。</param>
        /// <returns>是否存在指定类型和标识的数据。</returns>
        public bool HasData<T>(string id) where T : DataBase
        {
            return m_DataManager.HasData<T>(id);
        }

        /// <summary>
        /// 获取指定类型的数据。
        /// </summary>
        /// <typeparam name="T">数据类型。</typeparam>
        /// <returns>要获取的数据。</returns>
        public T GetData<T>() where T : DataBase
        {
            return m_DataManager.GetData<T>();
        }

        /// <summary>
        /// 获取指定类型和标识的数据。
        /// </summary>
        /// <typeparam name="T">数据类型。</typeparam>
        /// <param name="id">数据标识。</param>
        /// <returns>要获取的数据。</returns>
        public T GetData<T>(string id) where T : DataBase
        {
            return m_DataManager.GetData<T>(id);
        }

        /// <summary>
        /// 获取指定类型的所有数据。
        /// </summary>
        /// <typeparam name="T">数据类型。</typeparam>
        /// <returns>指定类型的所有数据。</returns>
        public T[] GetAllData<T>() where T : DataBase
        {
            return m_DataManager.GetAllData<T>();
        }

        /// <summary>
        /// 获取指定类型的所有数据。
        /// </summary>
        /// <typeparam name="T">数据类型。</typeparam>
        /// <param name="results">指定类型的所有数据。</param>
        public void GetAllData<T>(List<T> results) where T : DataBase
        {
            m_DataManager.GetAllData<T>(results);
        }

        /// <summary>
        /// 获取所有数据。
        /// </summary>
        /// <returns>所有数据。</returns>
        public DataBase[] GetAllData()
        {
            return m_DataManager.GetAllData();
        }

        /// <summary>
        /// 获取所有数据。
        /// </summary>
        /// <param name="results">所有数据。</param>
        public void GetAllData(List<DataBase> results)
        {
            m_DataManager.GetAllData(results);
        }

        /// <summary>
        /// 设置数据。
        /// </summary>
        /// <typeparam name="T">数据类型。</typeparam>
        /// <param name="data">要设置的数据。</param>
        public void SetData<T>(T data) where T : DataBase
        {
            m_DataManager.SetData<T>(data);
        }

        /// <summary>
        /// 设置数据。
        /// </summary>
        /// <typeparam name="T">数据类型。</typeparam>
        /// <param name="id">数据标识。</param>
        /// <param name="data">要设置的数据。</param>
        public void SetData<T>(string id, T data) where T : DataBase
        {
            m_DataManager.SetData<T>(id, data);
        }

        /// <summary>
        /// 移除指定类型的数据。
        /// </summary>
        /// <typeparam name="T">数据类型。</typeparam>
        /// <returns>是否移除数据成功。</returns>
        public bool RemoveData<T>() where T : DataBase
        {
            return m_DataManager.RemoveData<T>();
        }

        /// <summary>
        /// 移除指定类型和标识的数据。
        /// </summary>
        /// <typeparam name="T">数据类型。</typeparam>
        /// <param name="id">数据标识。</param>
        /// <returns>是否移除数据成功。</returns>
        public bool RemoveData<T>(string id) where T : DataBase
        {
            return m_DataManager.RemoveData<T>(id);
        }

        /// <summary>
        /// 移除所有数据。
        /// </summary>
        public void RemoveAllData()
        {
            m_DataManager.RemoveAllData();
        }

        /// <summary>
        /// 从文件加载数据。
        /// </summary>
        /// <typeparam name="T">数据类型。</typeparam>
        /// <param name="filePath">文件路径。</param>
        public void LoadDataFromFile<T>(string filePath) where T : DataBase, new()
        {
            m_DataManager.LoadDataFromFile<T>(filePath);
        }

        /// <summary>
        /// 从文件加载数据。
        /// </summary>
        /// <typeparam name="T">数据类型。</typeparam>
        /// <param name="id">数据标识。</param>
        /// <param name="filePath">文件路径。</param>
        public void LoadDataFromFile<T>(string id, string filePath) where T : DataBase, new()
        {
            m_DataManager.LoadDataFromFile<T>(id, filePath);
        }

        /// <summary>
        /// 保存数据到文件。
        /// </summary>
        /// <typeparam name="T">数据类型。</typeparam>
        /// <param name="filePath">文件路径。</param>
        public void SaveDataToFile<T>(string filePath) where T : DataBase
        {
            m_DataManager.SaveDataToFile<T>(filePath);
        }

        /// <summary>
        /// 保存数据到文件。
        /// </summary>
        /// <typeparam name="T">数据类型。</typeparam>
        /// <param name="id">数据标识。</param>
        /// <param name="filePath">文件路径。</param>
        public void SaveDataToFile<T>(string id, string filePath) where T : DataBase
        {
            m_DataManager.SaveDataToFile<T>(id, filePath);
        }

        /// <summary>
        /// 从字节数组加载数据。
        /// </summary>
        /// <typeparam name="T">数据类型。</typeparam>
        /// <param name="bytes">字节数组。</param>
        public void LoadDataFromBytes<T>(byte[] bytes) where T : DataBase, new()
        {
            m_DataManager.LoadDataFromBytes<T>(bytes);
        }

        /// <summary>
        /// 从字节数组加载数据。
        /// </summary>
        /// <typeparam name="T">数据类型。</typeparam>
        /// <param name="id">数据标识。</param>
        /// <param name="bytes">字节数组。</param>
        public void LoadDataFromBytes<T>(string id, byte[] bytes) where T : DataBase, new()
        {
            m_DataManager.LoadDataFromBytes<T>(id, bytes);
        }

        /// <summary>
        /// 将数据转换为字节数组。
        /// </summary>
        /// <typeparam name="T">数据类型。</typeparam>
        /// <returns>字节数组。</returns>
        public byte[] SaveDataToBytes<T>() where T : DataBase
        {
            return m_DataManager.SaveDataToBytes<T>();
        }

        /// <summary>
        /// 将数据转换为字节数组。
        /// </summary>
        /// <typeparam name="T">数据类型。</typeparam>
        /// <param name="id">数据标识。</param>
        /// <returns>字节数组。</returns>
        public byte[] SaveDataToBytes<T>(string id) where T : DataBase
        {
            return m_DataManager.SaveDataToBytes<T>(id);
        }

        /// <summary>
        /// 从持久化路径加载数据。
        /// </summary>
        /// <typeparam name="T">数据类型。</typeparam>
        /// <param name="fileName">文件名。</param>
        public void LoadDataFromPersistentPath<T>(string fileName) where T : DataBase, new()
        {
            string filePath = Utility.Path.GetRegularPath(Path.Combine(Application.persistentDataPath, fileName));
            LoadDataFromFile<T>(filePath);
        }

        /// <summary>
        /// 从持久化路径加载数据。
        /// </summary>
        /// <typeparam name="T">数据类型。</typeparam>
        /// <param name="id">数据标识。</param>
        /// <param name="fileName">文件名。</param>
        public void LoadDataFromPersistentPath<T>(string id, string fileName) where T : DataBase, new()
        {
            string filePath = Utility.Path.GetRegularPath(Path.Combine(Application.persistentDataPath, fileName));
            LoadDataFromFile<T>(id, filePath);
        }

        /// <summary>
        /// 保存数据到持久化路径。
        /// </summary>
        /// <typeparam name="T">数据类型。</typeparam>
        /// <param name="fileName">文件名。</param>
        public void SaveDataToPersistentPath<T>(string fileName) where T : DataBase
        {
            string filePath = Utility.Path.GetRegularPath(Path.Combine(Application.persistentDataPath, fileName));
            SaveDataToFile<T>(filePath);
        }

        /// <summary>
        /// 保存数据到持久化路径。
        /// </summary>
        /// <typeparam name="T">数据类型。</typeparam>
        /// <param name="id">数据标识。</param>
        /// <param name="fileName">文件名。</param>
        public void SaveDataToPersistentPath<T>(string id, string fileName) where T : DataBase
        {
            string filePath = Utility.Path.GetRegularPath(Path.Combine(Application.persistentDataPath, fileName));
            SaveDataToFile<T>(id, filePath);
        }
    }
}