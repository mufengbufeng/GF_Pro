using System;

namespace GameFramework.Fsm
{
    /// <summary>
    /// 增强的状态机状态基类，提供常用功能
    /// </summary>
    /// <typeparam name="T">有限状态机持有者类型</typeparam>
    public abstract class FsmStateBase<T> : FsmState<T> where T : class
    {
        private float m_StateTime;
        private IFsm<T> m_Fsm;

        /// <summary>
        /// 获取状态持续时间
        /// </summary>
        protected float StateTime => m_StateTime;

        /// <summary>
        /// 获取状态机引用
        /// </summary>
        protected IFsm<T> Fsm => m_Fsm;

        /// <summary>
        /// 获取状态机持有者
        /// </summary>
        protected T Owner => m_Fsm?.Owner;

        /// <summary>
        /// 状态进入时调用
        /// </summary>
        /// <param name="fsm">有限状态机引用</param>
        protected internal override void OnEnter(IFsm<T> fsm)
        {
            m_StateTime = 0f;
            m_Fsm = fsm;
            OnStateEnter();
        }

        /// <summary>
        /// 状态更新时调用
        /// </summary>
        /// <param name="fsm">有限状态机引用</param>
        /// <param name="elapseSeconds">逻辑流逝时间</param>
        /// <param name="realElapseSeconds">真实流逝时间</param>
        protected internal override void OnUpdate(IFsm<T> fsm, float elapseSeconds, float realElapseSeconds)
        {
            m_StateTime += elapseSeconds;
            OnStateUpdate(elapseSeconds, realElapseSeconds);
        }

        /// <summary>
        /// 状态离开时调用
        /// </summary>
        /// <param name="fsm">有限状态机引用</param>
        /// <param name="isShutdown">是否是关闭状态机</param>
        protected internal override void OnLeave(IFsm<T> fsm, bool isShutdown)
        {
            OnStateLeave(isShutdown);
            m_Fsm = null;
        }

        /// <summary>
        /// 状态进入时的具体实现
        /// </summary>
        protected virtual void OnStateEnter()
        {
        }

        /// <summary>
        /// 状态更新时的具体实现
        /// </summary>
        /// <param name="elapseSeconds">逻辑流逝时间</param>
        /// <param name="realElapseSeconds">真实流逝时间</param>
        protected virtual void OnStateUpdate(float elapseSeconds, float realElapseSeconds)
        {
        }

        /// <summary>
        /// 状态离开时的具体实现
        /// </summary>
        /// <param name="isShutdown">是否是关闭状态机</param>
        protected virtual void OnStateLeave(bool isShutdown)
        {
        }

        /// <summary>
        /// 切换到指定状态
        /// </summary>
        /// <typeparam name="TState">目标状态类型</typeparam>
        protected void SwitchState<TState>() where TState : FsmState<T>
        {
            ChangeState<TState>(m_Fsm);
        }

        /// <summary>
        /// 切换到指定状态
        /// </summary>
        /// <param name="stateType">目标状态类型</param>
        protected void SwitchState(Type stateType)
        {
            ChangeState(m_Fsm, stateType);
        }

        /// <summary>
        /// 检查是否可以切换到指定状态
        /// </summary>
        /// <typeparam name="TState">目标状态类型</typeparam>
        /// <returns>是否可以切换</returns>
        protected virtual bool CanSwitchTo<TState>() where TState : FsmState<T>
        {
            return m_Fsm.HasState<TState>();
        }

        /// <summary>
        /// 安全切换状态，会检查是否可以切换
        /// </summary>
        /// <typeparam name="TState">目标状态类型</typeparam>
        /// <returns>是否成功切换</returns>
        protected bool TrySwitchState<TState>() where TState : FsmState<T>
        {
            if (CanSwitchTo<TState>())
            {
                SwitchState<TState>();
                return true;
            }
            return false;
        }

        /// <summary>
        /// 设置状态数据
        /// </summary>
        /// <typeparam name="TData">数据类型</typeparam>
        /// <param name="name">数据名称</param>
        /// <param name="data">数据值</param>
        protected void SetData<TData>(string name, TData data) where TData : Variable
        {
            m_Fsm.SetData(name, data);
        }

        /// <summary>
        /// 获取状态数据
        /// </summary>
        /// <typeparam name="TData">数据类型</typeparam>
        /// <param name="name">数据名称</param>
        /// <returns>数据值</returns>
        protected TData GetData<TData>(string name) where TData : Variable
        {
            return m_Fsm.GetData<TData>(name);
        }

        /// <summary>
        /// 检查是否存在数据
        /// </summary>
        /// <param name="name">数据名称</param>
        /// <returns>是否存在</returns>
        protected bool HasData(string name)
        {
            return m_Fsm.HasData(name);
        }

        /// <summary>
        /// 移除数据
        /// </summary>
        /// <param name="name">数据名称</param>
        /// <returns>是否移除成功</returns>
        protected bool RemoveData(string name)
        {
            return m_Fsm.RemoveData(name);
        }
    }
}