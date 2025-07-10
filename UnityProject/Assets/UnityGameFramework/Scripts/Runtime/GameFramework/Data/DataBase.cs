using System;

namespace GameFramework.Data
{
    /// <summary>
    /// 数据基类。
    /// </summary>
    [Serializable]
    public abstract class DataBase : IReference
    {
        private string m_Id;
        private DateTime m_LastUpdateTime;
        private bool m_IsDirty;

        /// <summary>
        /// 初始化数据基类的新实例。
        /// </summary>
        protected DataBase()
        {
            m_Id = string.Empty;
            m_LastUpdateTime = DateTime.UtcNow;
            m_IsDirty = false;
        }

        /// <summary>
        /// 获取或设置数据标识。
        /// </summary>
        public string Id
        {
            get
            {
                return m_Id;
            }
            set
            {
                if (m_Id != value)
                {
                    m_Id = value ?? string.Empty;
                    MarkDirty();
                }
            }
        }

        /// <summary>
        /// 获取数据最后更新时间。
        /// </summary>
        public DateTime LastUpdateTime
        {
            get
            {
                return m_LastUpdateTime;
            }
        }

        /// <summary>
        /// 获取数据是否已修改。
        /// </summary>
        public bool IsDirty
        {
            get
            {
                return m_IsDirty;
            }
        }

        /// <summary>
        /// 标记数据为已修改。
        /// </summary>
        public void MarkDirty()
        {
            m_IsDirty = true;
            m_LastUpdateTime = DateTime.UtcNow;
            OnDataChanged();
        }

        /// <summary>
        /// 标记数据为未修改。
        /// </summary>
        public void MarkClean()
        {
            m_IsDirty = false;
        }

        /// <summary>
        /// 初始化数据。
        /// </summary>
        /// <param name="id">数据标识。</param>
        protected void Initialize(string id)
        {
            m_Id = id ?? string.Empty;
            m_LastUpdateTime = DateTime.UtcNow;
            m_IsDirty = false;
            OnInitialize();
        }

        /// <summary>
        /// 清理数据。
        /// </summary>
        public virtual void Clear()
        {
            m_Id = string.Empty;
            m_LastUpdateTime = default(DateTime);
            m_IsDirty = false;
        }

        /// <summary>
        /// 复制数据。
        /// </summary>
        /// <param name="other">要复制的数据。</param>
        public virtual void CopyFrom(DataBase other)
        {
            if (other == null)
            {
                throw new GameFrameworkException("Copy source data is invalid.");
            }

            if (other.GetType() != GetType())
            {
                throw new GameFrameworkException(Utility.Text.Format("Copy source data type '{0}' does not match current data type '{1}'.", other.GetType().FullName, GetType().FullName));
            }

            m_Id = other.m_Id;
            MarkDirty();
        }

        /// <summary>
        /// 序列化数据为字节数组。
        /// </summary>
        /// <returns>序列化后的字节数组。</returns>
        public virtual byte[] Serialize()
        {
            string json = Utility.Json.ToJson(this);
            return System.Text.Encoding.UTF8.GetBytes(json);
        }

        /// <summary>
        /// 从字节数组反序列化数据。
        /// </summary>
        /// <param name="bytes">字节数组。</param>
        public virtual void Deserialize(byte[] bytes)
        {
            if (bytes == null || bytes.Length == 0)
            {
                throw new GameFrameworkException("Deserialize data bytes is invalid.");
            }

            string json = System.Text.Encoding.UTF8.GetString(bytes);
            object data = Utility.Json.ToObject(GetType(), json);
            if (data == null)
            {
                throw new GameFrameworkException("Deserialize data failed.");
            }

            if (data.GetType() != GetType())
            {
                throw new GameFrameworkException(Utility.Text.Format("Deserialize data type '{0}' does not match current data type '{1}'.", data.GetType().FullName, GetType().FullName));
            }

            CopyFrom((DataBase)data);
        }

        /// <summary>
        /// 验证数据是否有效。
        /// </summary>
        /// <returns>数据是否有效。</returns>
        public virtual bool IsValid()
        {
            return true;
        }

        /// <summary>
        /// 数据初始化时的回调。
        /// </summary>
        protected virtual void OnInitialize()
        {
        }

        /// <summary>
        /// 数据变更时的回调。
        /// </summary>
        protected virtual void OnDataChanged()
        {
        }

        /// <summary>
        /// 返回表示当前对象的字符串。
        /// </summary>
        /// <returns>表示当前对象的字符串。</returns>
        public override string ToString()
        {
            return Utility.Text.Format("[{0}] Id: {1}, LastUpdateTime: {2}, IsDirty: {3}", GetType().Name, m_Id, m_LastUpdateTime.ToString("yyyy-MM-dd HH:mm:ss"), m_IsDirty);
        }
    }
}