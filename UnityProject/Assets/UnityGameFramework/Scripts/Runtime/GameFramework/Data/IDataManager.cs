using System;
using System.Collections.Generic;

namespace GameFramework.Data
{
    /// <summary>
    /// 数据管理器接口。
    /// </summary>
    public interface IDataManager
    {
        /// <summary>
        /// 获取数据数量。
        /// </summary>
        int DataCount
        {
            get;
        }

        /// <summary>
        /// 数据变更事件。
        /// </summary>
        event EventHandler<DataChangedEventArgs> DataChanged;

        /// <summary>
        /// 数据加载成功事件。
        /// </summary>
        event EventHandler<LoadDataSuccessEventArgs> LoadDataSuccess;

        /// <summary>
        /// 数据加载失败事件。
        /// </summary>
        event EventHandler<LoadDataFailureEventArgs> LoadDataFailure;

        /// <summary>
        /// 数据保存成功事件。
        /// </summary>
        event EventHandler<SaveDataSuccessEventArgs> SaveDataSuccess;

        /// <summary>
        /// 数据保存失败事件。
        /// </summary>
        event EventHandler<SaveDataFailureEventArgs> SaveDataFailure;

        /// <summary>
        /// 检查是否存在指定类型的数据。
        /// </summary>
        /// <typeparam name="T">数据类型。</typeparam>
        /// <returns>是否存在指定类型的数据。</returns>
        bool HasData<T>() where T : DataBase;

        /// <summary>
        /// 检查是否存在指定类型和标识的数据。
        /// </summary>
        /// <typeparam name="T">数据类型。</typeparam>
        /// <param name="id">数据标识。</param>
        /// <returns>是否存在指定类型和标识的数据。</returns>
        bool HasData<T>(string id) where T : DataBase;

        /// <summary>
        /// 获取指定类型的数据。
        /// </summary>
        /// <typeparam name="T">数据类型。</typeparam>
        /// <returns>要获取的数据。</returns>
        T GetData<T>() where T : DataBase;

        /// <summary>
        /// 获取指定类型和标识的数据。
        /// </summary>
        /// <typeparam name="T">数据类型。</typeparam>
        /// <param name="id">数据标识。</param>
        /// <returns>要获取的数据。</returns>
        T GetData<T>(string id) where T : DataBase;

        /// <summary>
        /// 获取指定类型的所有数据。
        /// </summary>
        /// <typeparam name="T">数据类型。</typeparam>
        /// <returns>指定类型的所有数据。</returns>
        T[] GetAllData<T>() where T : DataBase;

        /// <summary>
        /// 获取指定类型的所有数据。
        /// </summary>
        /// <typeparam name="T">数据类型。</typeparam>
        /// <param name="results">指定类型的所有数据。</param>
        void GetAllData<T>(List<T> results) where T : DataBase;

        /// <summary>
        /// 获取所有数据。
        /// </summary>
        /// <returns>所有数据。</returns>
        DataBase[] GetAllData();

        /// <summary>
        /// 获取所有数据。
        /// </summary>
        /// <param name="results">所有数据。</param>
        void GetAllData(List<DataBase> results);

        /// <summary>
        /// 设置数据。
        /// </summary>
        /// <typeparam name="T">数据类型。</typeparam>
        /// <param name="data">要设置的数据。</param>
        void SetData<T>(T data) where T : DataBase;

        /// <summary>
        /// 设置数据。
        /// </summary>
        /// <typeparam name="T">数据类型。</typeparam>
        /// <param name="id">数据标识。</param>
        /// <param name="data">要设置的数据。</param>
        void SetData<T>(string id, T data) where T : DataBase;

        /// <summary>
        /// 移除指定类型的数据。
        /// </summary>
        /// <typeparam name="T">数据类型。</typeparam>
        /// <returns>是否移除数据成功。</returns>
        bool RemoveData<T>() where T : DataBase;

        /// <summary>
        /// 移除指定类型和标识的数据。
        /// </summary>
        /// <typeparam name="T">数据类型。</typeparam>
        /// <param name="id">数据标识。</param>
        /// <returns>是否移除数据成功。</returns>
        bool RemoveData<T>(string id) where T : DataBase;

        /// <summary>
        /// 移除所有数据。
        /// </summary>
        void RemoveAllData();

        /// <summary>
        /// 从文件加载数据。
        /// </summary>
        /// <typeparam name="T">数据类型。</typeparam>
        /// <param name="filePath">文件路径。</param>
        void LoadDataFromFile<T>(string filePath) where T : DataBase, new();

        /// <summary>
        /// 从文件加载数据。
        /// </summary>
        /// <typeparam name="T">数据类型。</typeparam>
        /// <param name="id">数据标识。</param>
        /// <param name="filePath">文件路径。</param>
        void LoadDataFromFile<T>(string id, string filePath) where T : DataBase, new();

        /// <summary>
        /// 保存数据到文件。
        /// </summary>
        /// <typeparam name="T">数据类型。</typeparam>
        /// <param name="filePath">文件路径。</param>
        void SaveDataToFile<T>(string filePath) where T : DataBase;

        /// <summary>
        /// 保存数据到文件。
        /// </summary>
        /// <typeparam name="T">数据类型。</typeparam>
        /// <param name="id">数据标识。</param>
        /// <param name="filePath">文件路径。</param>
        void SaveDataToFile<T>(string id, string filePath) where T : DataBase;

        /// <summary>
        /// 从字节数组加载数据。
        /// </summary>
        /// <typeparam name="T">数据类型。</typeparam>
        /// <param name="bytes">字节数组。</param>
        void LoadDataFromBytes<T>(byte[] bytes) where T : DataBase, new();

        /// <summary>
        /// 从字节数组加载数据。
        /// </summary>
        /// <typeparam name="T">数据类型。</typeparam>
        /// <param name="id">数据标识。</param>
        /// <param name="bytes">字节数组。</param>
        void LoadDataFromBytes<T>(string id, byte[] bytes) where T : DataBase, new();

        /// <summary>
        /// 将数据转换为字节数组。
        /// </summary>
        /// <typeparam name="T">数据类型。</typeparam>
        /// <returns>字节数组。</returns>
        byte[] SaveDataToBytes<T>() where T : DataBase;

        /// <summary>
        /// 将数据转换为字节数组。
        /// </summary>
        /// <typeparam name="T">数据类型。</typeparam>
        /// <param name="id">数据标识。</param>
        /// <returns>字节数组。</returns>
        byte[] SaveDataToBytes<T>(string id) where T : DataBase;
    }
}