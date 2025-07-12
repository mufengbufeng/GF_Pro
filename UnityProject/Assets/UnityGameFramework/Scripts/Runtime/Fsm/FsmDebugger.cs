using System;
using System.Collections.Generic;
using System.Text;

namespace GameFramework.Fsm
{
    /// <summary>
    /// 状态机调试器
    /// </summary>
    public static class FsmDebugger
    {
        private static readonly Dictionary<string, StateTransitionRecord> s_TransitionRecords = new Dictionary<string, StateTransitionRecord>();
        private static readonly Dictionary<string, List<string>> s_StateHistory = new Dictionary<string, List<string>>();
        
        /// <summary>
        /// 状态转换记录
        /// </summary>
        public class StateTransitionRecord
        {
            public string FromState { get; set; }
            public string ToState { get; set; }
            public float Timestamp { get; set; }
            public float Duration { get; set; }
        }

        /// <summary>
        /// 记录状态转换
        /// </summary>
        /// <param name="fsmName">状态机名称</param>
        /// <param name="fromState">源状态</param>
        /// <param name="toState">目标状态</param>
        /// <param name="duration">状态持续时间</param>
        public static void RecordStateTransition(string fsmName, string fromState, string toState, float duration)
        {
            var record = new StateTransitionRecord
            {
                FromState = fromState,
                ToState = toState,
                Timestamp = UnityEngine.Time.time,
                Duration = duration
            };

            s_TransitionRecords[fsmName] = record;

            // 记录状态历史
            if (!s_StateHistory.ContainsKey(fsmName))
            {
                s_StateHistory[fsmName] = new List<string>();
            }

            var history = s_StateHistory[fsmName];
            history.Add($"[{record.Timestamp:F2}] {fromState} -> {toState} (Duration: {duration:F2}s)");

            // 限制历史记录数量
            if (history.Count > 100)
            {
                history.RemoveAt(0);
            }
        }

        /// <summary>
        /// 获取状态转换历史
        /// </summary>
        /// <param name="fsmName">状态机名称</param>
        /// <returns>转换历史</returns>
        public static List<string> GetStateHistory(string fsmName)
        {
            return s_StateHistory.ContainsKey(fsmName) ? s_StateHistory[fsmName] : new List<string>();
        }

        /// <summary>
        /// 获取最后的状态转换记录
        /// </summary>
        /// <param name="fsmName">状态机名称</param>
        /// <returns>转换记录</returns>
        public static StateTransitionRecord GetLastTransition(string fsmName)
        {
            return s_TransitionRecords.ContainsKey(fsmName) ? s_TransitionRecords[fsmName] : null;
        }

        /// <summary>
        /// 清除指定状态机的调试数据
        /// </summary>
        /// <param name="fsmName">状态机名称</param>
        public static void ClearDebugData(string fsmName)
        {
            s_TransitionRecords.Remove(fsmName);
            s_StateHistory.Remove(fsmName);
        }

        /// <summary>
        /// 清除所有调试数据
        /// </summary>
        public static void ClearAllDebugData()
        {
            s_TransitionRecords.Clear();
            s_StateHistory.Clear();
        }

        /// <summary>
        /// 生成状态机调试报告
        /// </summary>
        /// <param name="fsm">状态机</param>
        /// <returns>调试报告</returns>
        public static string GenerateDebugReport<T>(IFsm<T> fsm) where T : class
        {
            var sb = new StringBuilder();
            sb.AppendLine($"=== FSM Debug Report: {fsm.FullName} ===");
            sb.AppendLine($"Owner: {fsm.Owner?.GetType().Name}");
            sb.AppendLine($"State Count: {fsm.FsmStateCount}");
            sb.AppendLine($"Is Running: {fsm.IsRunning}");
            sb.AppendLine($"Is Destroyed: {fsm.IsDestroyed}");
            sb.AppendLine($"Current State: {fsm.CurrentState?.GetType().Name ?? "None"}");
            sb.AppendLine($"Current State Time: {fsm.CurrentStateTime:F2}s");

            // 状态历史
            var history = GetStateHistory(fsm.FullName);
            if (history.Count > 0)
            {
                sb.AppendLine("\n--- State History ---");
                foreach (var entry in history)
                {
                    sb.AppendLine(entry);
                }
            }

            // 最后的转换记录
            var lastTransition = GetLastTransition(fsm.FullName);
            if (lastTransition != null)
            {
                sb.AppendLine("\n--- Last Transition ---");
                sb.AppendLine($"From: {lastTransition.FromState}");
                sb.AppendLine($"To: {lastTransition.ToState}");
                sb.AppendLine($"Time: {lastTransition.Timestamp:F2}");
                sb.AppendLine($"Duration: {lastTransition.Duration:F2}s");
            }

            return sb.ToString();
        }

        /// <summary>
        /// 输出状态机调试信息到日志
        /// </summary>
        /// <param name="fsm">状态机</param>
        public static void LogDebugInfo<T>(IFsm<T> fsm) where T : class
        {
            string report = GenerateDebugReport(fsm);
            UnityEngine.Debug.Log(report);
        }
    }

    /// <summary>
    /// 带调试功能的状态基类
    /// </summary>
    /// <typeparam name="T">状态机持有者类型</typeparam>
    public abstract class DebuggableFsmState<T> : FsmStateBase<T> where T : class
    {
        private string m_LastStateName;

        protected internal override void OnEnter(IFsm<T> fsm)
        {
            base.OnEnter(fsm);
            
            // 记录状态转换
            string currentStateName = GetType().Name;
            if (!string.IsNullOrEmpty(m_LastStateName))
            {
                FsmDebugger.RecordStateTransition(fsm.FullName, m_LastStateName, currentStateName, StateTime);
            }
            
            OnDebugEnter();
        }

        protected internal override void OnLeave(IFsm<T> fsm, bool isShutdown)
        {
            m_LastStateName = GetType().Name;
            OnDebugLeave(isShutdown);
            base.OnLeave(fsm, isShutdown);
        }

        /// <summary>
        /// 调试用的进入方法
        /// </summary>
        protected virtual void OnDebugEnter()
        {
            if (IsDebugEnabled())
            {
                UnityEngine.Debug.Log($"[FSM] Enter State: {GetType().Name} in {Fsm.FullName}");
            }
        }

        /// <summary>
        /// 调试用的离开方法
        /// </summary>
        /// <param name="isShutdown">是否是关闭状态机</param>
        protected virtual void OnDebugLeave(bool isShutdown)
        {
            if (IsDebugEnabled())
            {
                UnityEngine.Debug.Log($"[FSM] Leave State: {GetType().Name} in {Fsm.FullName} (Duration: {StateTime:F2}s, Shutdown: {isShutdown})");
            }
        }

        /// <summary>
        /// 是否启用调试
        /// </summary>
        /// <returns>是否启用</returns>
        protected virtual bool IsDebugEnabled()
        {
            return UnityEngine.Debug.isDebugBuild;
        }

        /// <summary>
        /// 输出调试信息
        /// </summary>
        /// <param name="message">调试信息</param>
        protected void DebugLog(string message)
        {
            if (IsDebugEnabled())
            {
                UnityEngine.Debug.Log($"[FSM] [{GetType().Name}] {message}");
            }
        }

        /// <summary>
        /// 输出调试警告
        /// </summary>
        /// <param name="message">警告信息</param>
        protected void DebugLogWarning(string message)
        {
            if (IsDebugEnabled())
            {
                UnityEngine.Debug.LogWarning($"[FSM] [{GetType().Name}] {message}");
            }
        }

        /// <summary>
        /// 输出调试错误
        /// </summary>
        /// <param name="message">错误信息</param>
        protected void DebugLogError(string message)
        {
            if (IsDebugEnabled())
            {
                UnityEngine.Debug.LogError($"[FSM] [{GetType().Name}] {message}");
            }
        }
    }
}