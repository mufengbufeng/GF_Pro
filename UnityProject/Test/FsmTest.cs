using System.Collections;
using UnityEngine;
using UnityEngine.TestTools;
using NUnit.Framework;
using GameFramework;
using GameFramework.Fsm;

namespace Tests
{
    /// <summary>
    /// FSM模块单元测试
    /// </summary>
    public class FsmTest
    {
        private FsmManager m_FsmManager;
        private TestOwner m_TestOwner;

        [SetUp]
        public void SetUp()
        {
            m_FsmManager = new FsmManager();
            m_TestOwner = new TestOwner();
        }

        [TearDown]
        public void TearDown()
        {
            m_FsmManager?.Shutdown();
            m_FsmManager = null;
            m_TestOwner = null;
        }

        #region 基础功能测试

        [Test]
        public void CreateFsm_ShouldCreateSuccessfully()
        {
            // Arrange
            var states = new FsmState<TestOwner>[] { new TestStateA(), new TestStateB() };

            // Act
            var fsm = m_FsmManager.CreateFsm("TestFsm", m_TestOwner, states);

            // Assert
            Assert.IsNotNull(fsm);
            Assert.AreEqual("TestFsm", fsm.Name);
            Assert.AreEqual(m_TestOwner, fsm.Owner);
            Assert.AreEqual(2, fsm.FsmStateCount);
            Assert.IsFalse(fsm.IsRunning);
            Assert.IsFalse(fsm.IsDestroyed);
        }

        [Test]
        public void StartFsm_ShouldStartWithSpecifiedState()
        {
            // Arrange
            var states = new FsmState<TestOwner>[] { new TestStateA(), new TestStateB() };
            var fsm = m_FsmManager.CreateFsm("TestFsm", m_TestOwner, states);

            // Act
            fsm.Start<TestStateA>();

            // Assert
            Assert.IsTrue(fsm.IsRunning);
            Assert.IsInstanceOf<TestStateA>(fsm.CurrentState);
            Assert.AreEqual(0f, fsm.CurrentStateTime);
        }

        [Test]
        public void HasFsm_ShouldReturnCorrectResult()
        {
            // Arrange
            var states = new FsmState<TestOwner>[] { new TestStateA() };
            m_FsmManager.CreateFsm("TestFsm", m_TestOwner, states);

            // Act & Assert
            Assert.IsTrue(m_FsmManager.HasFsm<TestOwner>("TestFsm"));
            Assert.IsTrue(m_FsmManager.HasFsm(typeof(TestOwner), "TestFsm"));
            Assert.IsFalse(m_FsmManager.HasFsm<TestOwner>("NonExistentFsm"));
        }

        [Test]
        public void GetFsm_ShouldReturnCorrectFsm()
        {
            // Arrange
            var states = new FsmState<TestOwner>[] { new TestStateA() };
            var originalFsm = m_FsmManager.CreateFsm("TestFsm", m_TestOwner, states);

            // Act
            var retrievedFsm = m_FsmManager.GetFsm<TestOwner>("TestFsm");

            // Assert
            Assert.AreSame(originalFsm, retrievedFsm);
        }

        [Test]
        public void DestroyFsm_ShouldRemoveFsmFromManager()
        {
            // Arrange
            var states = new FsmState<TestOwner>[] { new TestStateA() };
            var fsm = m_FsmManager.CreateFsm("TestFsm", m_TestOwner, states);

            // Act
            bool destroyed = m_FsmManager.DestroyFsm<TestOwner>("TestFsm");

            // Assert
            Assert.IsTrue(destroyed);
            Assert.IsFalse(m_FsmManager.HasFsm<TestOwner>("TestFsm"));
        }

        #endregion

        #region 状态管理测试

        [Test]
        public void HasState_ShouldReturnCorrectResult()
        {
            // Arrange
            var states = new FsmState<TestOwner>[] { new TestStateA(), new TestStateB() };
            var fsm = m_FsmManager.CreateFsm("TestFsm", m_TestOwner, states);

            // Act & Assert
            Assert.IsTrue(fsm.HasState<TestStateA>());
            Assert.IsTrue(fsm.HasState<TestStateB>());
            Assert.IsFalse(fsm.HasState<TestStateC>());
        }

        [Test]
        public void GetState_ShouldReturnCorrectState()
        {
            // Arrange
            var stateA = new TestStateA();
            var stateB = new TestStateB();
            var states = new FsmState<TestOwner>[] { stateA, stateB };
            var fsm = m_FsmManager.CreateFsm("TestFsm", m_TestOwner, states);

            // Act
            var retrievedStateA = fsm.GetState<TestStateA>();
            var retrievedStateB = fsm.GetState<TestStateB>();

            // Assert
            Assert.AreSame(stateA, retrievedStateA);
            Assert.AreSame(stateB, retrievedStateB);
        }

        [Test]
        public void GetAllStates_ShouldReturnAllStates()
        {
            // Arrange
            var states = new FsmState<TestOwner>[] { new TestStateA(), new TestStateB() };
            var fsm = m_FsmManager.CreateFsm("TestFsm", m_TestOwner, states);

            // Act
            var allStates = fsm.GetAllStates();

            // Assert
            Assert.AreEqual(2, allStates.Length);
        }

        #endregion

        #region 数据管理测试

        [Test]
        public void SetAndGetData_ShouldWorkCorrectly()
        {
            // Arrange
            var states = new FsmState<TestOwner>[] { new TestStateA() };
            var fsm = m_FsmManager.CreateFsm("TestFsm", m_TestOwner, states);
            var testData = VarInt32.Create(42);

            // Act
            fsm.SetData("TestInt", testData);
            var retrievedData = fsm.GetData<VarInt32>("TestInt");

            // Assert
            Assert.IsNotNull(retrievedData);
            Assert.AreEqual(42, retrievedData.Value);
        }

        [Test]
        public void HasData_ShouldReturnCorrectResult()
        {
            // Arrange
            var states = new FsmState<TestOwner>[] { new TestStateA() };
            var fsm = m_FsmManager.CreateFsm("TestFsm", m_TestOwner, states);

            // Act
            fsm.SetData("TestData", VarString.Create("test"));

            // Assert
            Assert.IsTrue(fsm.HasData("TestData"));
            Assert.IsFalse(fsm.HasData("NonExistentData"));
        }

        [Test]
        public void RemoveData_ShouldRemoveDataSuccessfully()
        {
            // Arrange
            var states = new FsmState<TestOwner>[] { new TestStateA() };
            var fsm = m_FsmManager.CreateFsm("TestFsm", m_TestOwner, states);
            fsm.SetData("TestData", VarString.Create("test"));

            // Act
            bool removed = fsm.RemoveData("TestData");

            // Assert
            Assert.IsTrue(removed);
            Assert.IsFalse(fsm.HasData("TestData"));
        }

        #endregion

        #region 状态转换测试

        [UnityTest]
        public IEnumerator StateTransition_ShouldCallCorrectLifecycleMethods()
        {
            // Arrange
            var stateA = new TestStateA();
            var stateB = new TestStateB();
            var states = new FsmState<TestOwner>[] { stateA, stateB };
            var fsm = m_FsmManager.CreateFsm("TestFsm", m_TestOwner, states);

            fsm.Start<TestStateA>();

            // Act - 模拟状态转换
            yield return new WaitForSeconds(0.1f);
            stateA.TriggerTransition = true;
            
            // 更新状态机
            m_FsmManager.Update(0.1f, 0.1f);

            // Assert
            Assert.IsInstanceOf<TestStateB>(fsm.CurrentState);
            Assert.IsTrue(stateA.OnLeaveCalled);
            Assert.IsTrue(stateB.OnEnterCalled);
        }

        [UnityTest]
        public IEnumerator Update_ShouldUpdateCurrentStateTime()
        {
            // Arrange
            var states = new FsmState<TestOwner>[] { new TestStateA() };
            var fsm = m_FsmManager.CreateFsm("TestFsm", m_TestOwner, states);
            fsm.Start<TestStateA>();

            // Act
            yield return new WaitForSeconds(0.1f);
            m_FsmManager.Update(0.1f, 0.1f);

            // Assert
            Assert.Greater(fsm.CurrentStateTime, 0f);
        }

        #endregion

        #region 异常处理测试

        [Test]
        public void CreateFsm_WithNullOwner_ShouldThrowException()
        {
            // Arrange
            var states = new FsmState<TestOwner>[] { new TestStateA() };

            // Act & Assert
            Assert.Throws<GameFrameworkException>(() => 
                m_FsmManager.CreateFsm<TestOwner>("TestFsm", null, states));
        }

        [Test]
        public void CreateFsm_WithNullStates_ShouldThrowException()
        {
            // Act & Assert
            Assert.Throws<GameFrameworkException>(() => 
                m_FsmManager.CreateFsm("TestFsm", m_TestOwner, (FsmState<TestOwner>[])null));
        }

        [Test]
        public void CreateFsm_WithEmptyStates_ShouldThrowException()
        {
            // Arrange
            var states = new FsmState<TestOwner>[0];

            // Act & Assert
            Assert.Throws<GameFrameworkException>(() => 
                m_FsmManager.CreateFsm("TestFsm", m_TestOwner, states));
        }

        [Test]
        public void StartFsm_WithNonExistentState_ShouldThrowException()
        {
            // Arrange
            var states = new FsmState<TestOwner>[] { new TestStateA() };
            var fsm = m_FsmManager.CreateFsm("TestFsm", m_TestOwner, states);

            // Act & Assert
            Assert.Throws<GameFrameworkException>(() => fsm.Start<TestStateB>());
        }

        #endregion
    }

    #region 测试用类

    public class TestOwner
    {
        public string Name { get; set; } = "TestOwner";
    }

    public class TestStateA : FsmState<TestOwner>
    {
        public bool OnInitCalled { get; private set; }
        public bool OnEnterCalled { get; private set; }
        public bool OnUpdateCalled { get; private set; }
        public bool OnLeaveCalled { get; private set; }
        public bool OnDestroyCalled { get; private set; }
        public bool TriggerTransition { get; set; }

        protected internal override void OnInit(IFsm<TestOwner> fsm)
        {
            OnInitCalled = true;
        }

        protected internal override void OnEnter(IFsm<TestOwner> fsm)
        {
            OnEnterCalled = true;
        }

        protected internal override void OnUpdate(IFsm<TestOwner> fsm, float elapseSeconds, float realElapseSeconds)
        {
            OnUpdateCalled = true;
            
            if (TriggerTransition && fsm.HasState<TestStateB>())
            {
                ChangeState<TestStateB>(fsm);
            }
        }

        protected internal override void OnLeave(IFsm<TestOwner> fsm, bool isShutdown)
        {
            OnLeaveCalled = true;
        }

        protected internal override void OnDestroy(IFsm<TestOwner> fsm)
        {
            OnDestroyCalled = true;
        }
    }

    public class TestStateB : FsmState<TestOwner>
    {
        public bool OnEnterCalled { get; private set; }

        protected internal override void OnEnter(IFsm<TestOwner> fsm)
        {
            OnEnterCalled = true;
        }
    }

    public class TestStateC : FsmState<TestOwner>
    {
        // 用于测试不存在的状态
    }

    #endregion
}