namespace GameFramework.UI
{
    internal sealed partial class UIManager : GameFrameworkModule, IUIManager
    {
        /// <summary>
        /// 打开界面信息类。
        /// 已修改：将UIGroup改为Order来管理层级
        /// </summary>
        private sealed class OpenUIFormInfo : IReference
        {
            private int m_SerialId;
            private int m_Order; // 已修改：将UIGroup改为Order
            private bool m_PauseCoveredUIForm;
            private object m_UserData;

            public OpenUIFormInfo()
            {
                m_SerialId = 0;
                m_Order = 0; // 已修改：初始化Order
                m_PauseCoveredUIForm = false;
                m_UserData = null;
            }

            public int SerialId
            {
                get
                {
                    return m_SerialId;
                }
            }

            /// <summary>
            /// 界面显示顺序（数值越大越靠前显示）
            /// 已修改：替换UIGroup属性
            /// </summary>
            public int Order
            {
                get
                {
                    return m_Order;
                }
            }

            public bool PauseCoveredUIForm
            {
                get
                {
                    return m_PauseCoveredUIForm;
                }
            }

            public object UserData
            {
                get
                {
                    return m_UserData;
                }
            }

            /// <summary>
            /// 创建打开界面信息。
            /// 已修改：将UIGroup参数改为order参数
            /// </summary>
            public static OpenUIFormInfo Create(int serialId, int order, bool pauseCoveredUIForm, object userData)
            {
                OpenUIFormInfo openUIFormInfo = ReferencePool.Acquire<OpenUIFormInfo>();
                openUIFormInfo.m_SerialId = serialId;
                openUIFormInfo.m_Order = order;
                openUIFormInfo.m_PauseCoveredUIForm = pauseCoveredUIForm;
                openUIFormInfo.m_UserData = userData;
                return openUIFormInfo;
            }

            public void Clear()
            {
                m_SerialId = 0;
                m_Order = 0; // 已修改：清理Order
                m_PauseCoveredUIForm = false;
                m_UserData = null;
            }
        }
    }
}
