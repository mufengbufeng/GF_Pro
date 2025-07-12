using System;
using System.Collections.Generic;

namespace GameFramework.Fsm
{
    /// <summary>
    /// 状态机转换条件基类
    /// </summary>
    /// <typeparam name="T">状态机持有者类型</typeparam>
    public abstract class FsmCondition<T> where T : class
    {
        /// <summary>
        /// 检查条件是否满足
        /// </summary>
        /// <param name="fsm">状态机引用</param>
        /// <returns>条件是否满足</returns>
        public abstract bool Check(IFsm<T> fsm);
    }

    /// <summary>
    /// 数据条件检查器
    /// </summary>
    /// <typeparam name="T">状态机持有者类型</typeparam>
    /// <typeparam name="TData">数据类型</typeparam>
    public class DataCondition<T, TData> : FsmCondition<T> 
        where T : class 
        where TData : Variable, IComparable<TData>
    {
        private readonly string m_DataName;
        private readonly TData m_TargetValue;
        private readonly CompareType m_CompareType;

        public enum CompareType
        {
            Equal,
            NotEqual,
            Greater,
            GreaterOrEqual,
            Less,
            LessOrEqual
        }

        public DataCondition(string dataName, TData targetValue, CompareType compareType = CompareType.Equal)
        {
            m_DataName = dataName;
            m_TargetValue = targetValue;
            m_CompareType = compareType;
        }

        public override bool Check(IFsm<T> fsm)
        {
            if (!fsm.HasData(m_DataName))
                return false;

            var data = fsm.GetData<TData>(m_DataName);
            if (data == null)
                return false;

            int compareResult = data.CompareTo(m_TargetValue);

            return m_CompareType switch
            {
                CompareType.Equal => compareResult == 0,
                CompareType.NotEqual => compareResult != 0,
                CompareType.Greater => compareResult > 0,
                CompareType.GreaterOrEqual => compareResult >= 0,
                CompareType.Less => compareResult < 0,
                CompareType.LessOrEqual => compareResult <= 0,
                _ => false
            };
        }
    }

    /// <summary>
    /// 时间条件检查器
    /// </summary>
    /// <typeparam name="T">状态机持有者类型</typeparam>
    public class TimeCondition<T> : FsmCondition<T> where T : class
    {
        private readonly float m_Duration;

        public TimeCondition(float duration)
        {
            m_Duration = duration;
        }

        public override bool Check(IFsm<T> fsm)
        {
            return fsm.CurrentStateTime >= m_Duration;
        }
    }

    /// <summary>
    /// 复合条件检查器
    /// </summary>
    /// <typeparam name="T">状态机持有者类型</typeparam>
    public class CompositeCondition<T> : FsmCondition<T> where T : class
    {
        private readonly List<FsmCondition<T>> m_Conditions;
        private readonly LogicType m_LogicType;

        public enum LogicType
        {
            And,
            Or
        }

        public CompositeCondition(LogicType logicType, params FsmCondition<T>[] conditions)
        {
            m_LogicType = logicType;
            m_Conditions = new List<FsmCondition<T>>(conditions);
        }

        public void AddCondition(FsmCondition<T> condition)
        {
            m_Conditions.Add(condition);
        }

        public override bool Check(IFsm<T> fsm)
        {
            if (m_Conditions.Count == 0)
                return true;

            return m_LogicType switch
            {
                LogicType.And => CheckAnd(fsm),
                LogicType.Or => CheckOr(fsm),
                _ => false
            };
        }

        private bool CheckAnd(IFsm<T> fsm)
        {
            foreach (var condition in m_Conditions)
            {
                if (!condition.Check(fsm))
                    return false;
            }
            return true;
        }

        private bool CheckOr(IFsm<T> fsm)
        {
            foreach (var condition in m_Conditions)
            {
                if (condition.Check(fsm))
                    return true;
            }
            return false;
        }
    }

    /// <summary>
    /// 委托条件检查器
    /// </summary>
    /// <typeparam name="T">状态机持有者类型</typeparam>
    public class DelegateCondition<T> : FsmCondition<T> where T : class
    {
        private readonly Func<IFsm<T>, bool> m_CheckFunc;

        public DelegateCondition(Func<IFsm<T>, bool> checkFunc)
        {
            m_CheckFunc = checkFunc ?? throw new ArgumentNullException(nameof(checkFunc));
        }

        public override bool Check(IFsm<T> fsm)
        {
            return m_CheckFunc(fsm);
        }
    }

    /// <summary>
    /// 条件状态：带有转换条件的状态基类
    /// </summary>
    /// <typeparam name="T">状态机持有者类型</typeparam>
    public abstract class ConditionalFsmState<T> : FsmStateBase<T> where T : class
    {
        private readonly Dictionary<Type, FsmCondition<T>> m_StateConditions;

        protected ConditionalFsmState()
        {
            m_StateConditions = new Dictionary<Type, FsmCondition<T>>();
            SetupConditions();
        }

        /// <summary>
        /// 设置状态转换条件
        /// </summary>
        protected abstract void SetupConditions();

        /// <summary>
        /// 添加状态转换条件
        /// </summary>
        /// <typeparam name="TState">目标状态类型</typeparam>
        /// <param name="condition">转换条件</param>
        protected void AddCondition<TState>(FsmCondition<T> condition) where TState : FsmState<T>
        {
            m_StateConditions[typeof(TState)] = condition;
        }

        /// <summary>
        /// 状态更新时检查转换条件
        /// </summary>
        /// <param name="elapseSeconds">逻辑流逝时间</param>
        /// <param name="realElapseSeconds">真实流逝时间</param>
        protected override void OnStateUpdate(float elapseSeconds, float realElapseSeconds)
        {
            base.OnStateUpdate(elapseSeconds, realElapseSeconds);

            // 检查所有转换条件
            foreach (var kvp in m_StateConditions)
            {
                if (kvp.Value.Check(Fsm))
                {
                    SwitchState(kvp.Key);
                    return;
                }
            }
        }
    }
}