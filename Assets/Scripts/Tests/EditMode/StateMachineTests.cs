using System;
using NUnit.Framework;
using WheelOfFortune.Commands;
using WheelOfFortune.Domain;
using WheelOfFortune.Interfaces;
using WheelOfFortune.StateMachine;
using WheelOfFortune.Tests.EditMode.Stubs;

namespace WheelOfFortune.Tests.EditMode
{
    [TestFixture]
    public class StateMachineTests
    {
        private StubZoneService _zone;
        private StubSpinService _spin;
        private StubRewardService _reward;
        private StubWheelView _wheel;
        private StubHudView _hud;
        private StubDialogView _dialog;
        private StubButtonView _button;
        private StubSpinStrategy _randomStrategy;
        private StubSpinStrategy _weightedStrategy;
        private StubWheelFactory _wheelFactory;

        private IGameState _currentState;
        private GameContext _ctx;
        private ReviveCommand _reviveCommand;
        private GiveUpCommand _giveUpCommand;

        [SetUp]
        public void SetUp()
        {
            _zone = new StubZoneService();
            _spin = new StubSpinService();
            _reward = new StubRewardService();
            _wheel = new StubWheelView();
            _hud = new StubHudView();
            _dialog = new StubDialogView();
            _button = new StubButtonView();
            _randomStrategy = new StubSpinStrategy();
            _wheelFactory = new StubWheelFactory();

            _reviveCommand = new ReviveCommand(TransitionTo, () => true);
            _giveUpCommand = new GiveUpCommand(_zone, _reward, TransitionTo);

            _ctx = new GameContext(
                _zone,
                _spin,
                _reward,
                _wheel,
                _hud,
                _dialog,
                _button,
                _wheelFactory,
                TransitionTo,
                _randomStrategy,
                _reviveCommand,
                _giveUpCommand);
        }

        private void TransitionTo(IGameState next)
        {
            _currentState?.Exit(_ctx);
            _currentState = next;
            _currentState.Enter(_ctx);
        }

        private void EnterIdle() => TransitionTo(new IdleState());

        private void SetupSpinResult(bool isBomb, int sliceIndex = 0)
        {
            _spin.ResultToReturn = new SpinResult(null, isBomb, sliceIndex);
            _wheelFactory.DataToReturn = new RuntimeWheelData(
                new WheelOfFortune.Data.SliceDefinition[8], isBomb ? 0 : -1, isBomb);
        }

        [Test]
        public void IdleState_Enter_UpdatesHudWithCurrentZone()
        {
            _zone.CurrentZone = 3;
            _zone.ZoneTypeToReturn = ZoneType.Normal;
            EnterIdle();
            Assert.IsNotNull(_hud.LastZoneProgress);
            Assert.AreEqual(3, _hud.LastZoneProgress.ZoneNumber);
            Assert.AreEqual(ZoneType.Normal, _hud.LastZoneProgress.ZoneType);
        }

        [Test]
        public void IdleState_Enter_UpdatesHudWithCurrentRewards()
        {
            _reward.Collect(null);
            _reward.Collect(null);
            EnterIdle();
            Assert.IsNotNull(_hud.LastRewards);
            Assert.AreEqual(2, _hud.LastRewards.Items.Count);
        }

        /*[Test]
        public void IdleState_Enter_UnlocksSpinButton()
        {
            EnterIdle();
            Assert.IsTrue(_button.SpinInteractable);
        }

        [Test]
        public void IdleState_Enter_HidesCollectWhenCannotLeave()
        {
            _zone.CanLeave = false;
            EnterIdle();
            Assert.IsFalse(_button.CollectVisible);
        }

        [Test]
        public void IdleState_Enter_ShowsCollectWhenCanLeave()
        {
            _zone.CanLeave = true;
            EnterIdle();
            Assert.IsTrue(_button.CollectVisible);
        }*/

        [Test]
        public void IdleState_CanSpin_AlwaysTrue()
        {
            var idle = new IdleState();
            idle.Enter(_ctx);
            Assert.IsTrue(idle.CanSpin());
        }

        [Test]
        public void IdleState_CanCollect_FalseWhenZoneIsNormal()
        {
            _zone.CanLeave = false;
            var idle = new IdleState();
            idle.Enter(_ctx);
            Assert.IsFalse(idle.CanCollect());
        }

        [Test]
        public void IdleState_CanCollect_TrueWhenZoneAllowsLeave()
        {
            _zone.CanLeave = true;
            var idle = new IdleState();
            idle.Enter(_ctx);
            Assert.IsTrue(idle.CanCollect());
        }

        [Test]
        public void IdleState_CanCollect_FalseAfterExit()
        {
            _zone.CanLeave = true;
            var idle = new IdleState();
            idle.Enter(_ctx);
            idle.Exit(_ctx);
            Assert.IsFalse(idle.CanCollect());
        }

        /*[Test]
        public void HappyPath_SpinReward_LocksSpinDuringSpin()
        {
            SetupSpinResult(false);
            _wheel.AutoInvokeCallback = false;
            EnterIdle();
            TransitionTo(new SpinningState());
            Assert.IsFalse(_button.SpinInteractable);
        }*/

        [Test]
        public void HappyPath_SpinReward_AdvancesZone()
        {
            SetupSpinResult(false, 2);
            EnterIdle();
            TransitionTo(new SpinningState());
            Assert.AreEqual(1, _zone.AdvanceCallCount);
        }

        [Test]
        public void HappyPath_SpinReward_CollectsItem()
        {
            SetupSpinResult(false, 2);
            EnterIdle();
            TransitionTo(new SpinningState());
            Assert.AreEqual(1, _reward.CollectedItems.Count);
        }

        [Test]
        public void HappyPath_SpinReward_EndsInIdleState()
        {
            SetupSpinResult(false, 2);
            EnterIdle();
            TransitionTo(new SpinningState());
            Assert.IsInstanceOf<IdleState>(_currentState);
        }

        [Test]
        public void HappyPath_SpinReward_WheelSpinsToCorrectIndex()
        {
            SetupSpinResult(false, 5);
            _spin.ResultToReturn = new SpinResult(null, false, 5);
            EnterIdle();
            TransitionTo(new SpinningState());
            Assert.AreEqual(5, _wheel.LastTargetIndex);
        }

        [Test]
        public void HappyPath_SpinReward_SetsRandomStrategyForNormalZone()
        {
            _zone.ZoneTypeToReturn = ZoneType.Normal;
            SetupSpinResult(false);
            EnterIdle();
            TransitionTo(new SpinningState());
            Assert.AreSame(_randomStrategy, _spin.LastStrategySet);
        }

        [Test]
        public void HappyPath_SpinReward_SetsWeightedStrategyForSuperZone()
        {
            _zone.ZoneTypeToReturn = ZoneType.Super;
            SetupSpinResult(false);
            EnterIdle();
            TransitionTo(new SpinningState());
            Assert.AreSame(_weightedStrategy, _spin.LastStrategySet);
        }

        [Test]
        public void HappyPath_WheelFactory_CalledWithCorrectZoneType()
        {
            _zone.ZoneTypeToReturn = ZoneType.Safe;
            SetupSpinResult(false);
            EnterIdle();
            TransitionTo(new SpinningState());
            Assert.AreEqual(ZoneType.Safe, _wheelFactory.LastZoneType);
        }

        [Test]
        public void HappyPath_WheelFactory_CalledWithCorrectZoneNumber()
        {
            _zone.CurrentZone = 10;
            SetupSpinResult(false);
            EnterIdle();
            TransitionTo(new SpinningState());
            Assert.AreEqual(10, _wheelFactory.LastZoneNumber);
        }

        [Test]
        public void HappyPath_MultipleSpins_AccumulatesRewards()
        {
            SetupSpinResult(false);
            EnterIdle();
            TransitionTo(new SpinningState());
            TransitionTo(new SpinningState());
            TransitionTo(new SpinningState());
            Assert.AreEqual(3, _reward.CollectedItems.Count);
        }

        [Test]
        public void BombPath_Spin_ShowsBombScreen()
        {
            SetupSpinResult(true);
            EnterIdle();
            TransitionTo(new SpinningState());
            Assert.IsTrue(_dialog.BombScreenShown);
        }

        [Test]
        public void BombPath_Spin_ClearsAllRewards()
        {
            _reward.Collect(null);
            _reward.Collect(null);
            SetupSpinResult(true);
            EnterIdle();
            TransitionTo(new SpinningState());
            Assert.AreEqual(1, _reward.ClearAllCallCount);
            Assert.AreEqual(0, _reward.CollectedItems.Count);
        }

        [Test]
        public void BombPath_Spin_EndsInBombState()
        {
            SetupSpinResult(true);
            EnterIdle();
            TransitionTo(new SpinningState());
            Assert.IsInstanceOf<BombState>(_currentState);
        }

        [Test]
        public void BombPath_Revive_TransitionsToIdle()
        {
            SetupSpinResult(true);
            EnterIdle();
            TransitionTo(new SpinningState());
            _dialog.SimulateRevive();
            Assert.IsInstanceOf<IdleState>(_currentState);
        }

        [Test]
        public void BombPath_Revive_DoesNotResetZone()
        {
            _zone.CurrentZone = 7;
            SetupSpinResult(true);
            EnterIdle();
            TransitionTo(new SpinningState());
            _dialog.SimulateRevive();
            Assert.AreEqual(0, _zone.ResetCallCount);
        }

        [Test]
        public void BombPath_GiveUp_TransitionsToIdle()
        {
            SetupSpinResult(true);
            EnterIdle();
            TransitionTo(new SpinningState());
            _dialog.SimulateGiveUp();
            Assert.IsInstanceOf<IdleState>(_currentState);
        }

        [Test]
        public void BombPath_GiveUp_ResetsZoneAndRewards()
        {
            SetupSpinResult(true);
            EnterIdle();
            TransitionTo(new SpinningState());
            _dialog.SimulateGiveUp();
            Assert.AreEqual(1, _zone.ResetCallCount);
            Assert.AreEqual(1, _reward.ResetCallCount);
        }

        [Test]
        public void BombPath_ExitBombState_HidesDialog()
        {
            SetupSpinResult(true);
            EnterIdle();
            TransitionTo(new SpinningState());
            _dialog.SimulateGiveUp();
            Assert.IsTrue(_dialog.HideCallCount);
        }

        [Test]
        public void CollectPath_Enter_ShowsConfirmScreen()
        {
            TransitionTo(new CollectState());
            Assert.IsTrue(_dialog.CollectScreenShown);
        }

        /*[Test]
        public void CollectPath_Enter_PassesRewardsToDialog()
        {
            _reward.Collect(null);
            _reward.Collect(null);
            TransitionTo(new CollectState());
            Assert.IsNotNull(_dialog.LastRewardsPassedToCollect);
            Assert.AreEqual(2, _dialog.LastRewardsPassedToCollect.Items.Count);
        }*/

        [Test]
        public void CollectPath_Confirm_ResetsZoneAndRewards()
        {
            TransitionTo(new CollectState());
            _dialog.SimulateConfirmCollect();
            Assert.AreEqual(1, _zone.ResetCallCount);
            Assert.AreEqual(1, _reward.ResetCallCount);
        }

        [Test]
        public void CollectPath_Confirm_TransitionsToIdle()
        {
            TransitionTo(new CollectState());
            _dialog.SimulateConfirmCollect();
            Assert.IsInstanceOf<IdleState>(_currentState);
        }

        [Test]
        public void CollectPath_Cancel_TransitionsToIdleWithoutReset()
        {
            TransitionTo(new CollectState());
            _dialog.SimulateCancelCollect();
            Assert.IsInstanceOf<IdleState>(_currentState);
            Assert.AreEqual(0, _zone.ResetCallCount);
            Assert.AreEqual(0, _reward.ResetCallCount);
        }

        [Test]
        public void CollectPath_Exit_HidesDialog()
        {
            TransitionTo(new CollectState());
            _dialog.SimulateConfirmCollect();
            Assert.IsTrue(_dialog.HideCallCount);
        }

        [Test]
        public void SpinningState_DeferredCallback_StillTransitionsCorrectly()
        {
            _wheel.AutoInvokeCallback = false;
            SetupSpinResult(false, 1);
            EnterIdle();
            TransitionTo(new SpinningState());
            Assert.IsInstanceOf<SpinningState>(_currentState);
            _wheel.InvokeCallback();
            Assert.IsInstanceOf<IdleState>(_currentState);
        }

        [Test]
        public void SpinningState_DeferredBombCallback_TransitionsToBomb()
        {
            _wheel.AutoInvokeCallback = false;
            SetupSpinResult(true);
            EnterIdle();
            TransitionTo(new SpinningState());
            _wheel.InvokeCallback();
            Assert.IsInstanceOf<BombState>(_currentState);
        }
    }
}