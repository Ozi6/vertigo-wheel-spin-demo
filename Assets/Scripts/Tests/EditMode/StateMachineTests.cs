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
        private StubSpinStrategy _randomStrategy;
        private StubSpinStrategy _weightedStrategy;

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
            _randomStrategy = new StubSpinStrategy();
            _weightedStrategy = new StubSpinStrategy();

            _reviveCommand = new ReviveCommand(TransitionTo, () => true);
            _giveUpCommand = new GiveUpCommand(_zone, _reward, TransitionTo);

            _ctx = new GameContext(
                _zone,
                _spin,
                _reward,
                _wheel,
                _hud,
                _dialog,
                null,
                TransitionTo,
                _randomStrategy,
                _weightedStrategy,
                _reviveCommand,
                _giveUpCommand);
        }

        private void TransitionTo(IGameState next)
        {
            _currentState?.Exit(_ctx);
            _currentState = next;
            _currentState.Enter(_ctx);
        }

        private void EnterIdle()
        {
            TransitionTo(new IdleState());
        }

        // ── IdleState ─────────────────────────────────────────────────────────

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

        // ── Happy path: Idle → Spinning → Reward → Idle ───────────────────────

        [Test]
        public void HappyPath_SpinReward_AdvancesZone()
        {
            _spin.ResultToReturn = new SpinResult(null, false, 2);

            EnterIdle();
            TransitionTo(new SpinningState());

            Assert.AreEqual(1, _zone.AdvanceCallCount);
        }

        [Test]
        public void HappyPath_SpinReward_CollectsItem()
        {
            _spin.ResultToReturn = new SpinResult(null, false, 2);

            EnterIdle();
            TransitionTo(new SpinningState());

            Assert.AreEqual(1, _reward.CollectedItems.Count);
        }

        [Test]
        public void HappyPath_SpinReward_EndsInIdleState()
        {
            _spin.ResultToReturn = new SpinResult(null, false, 2);

            EnterIdle();
            TransitionTo(new SpinningState());

            Assert.IsInstanceOf<IdleState>(_currentState);
        }

        [Test]
        public void HappyPath_SpinReward_WheelSpinsToCorrectIndex()
        {
            _spin.ResultToReturn = new SpinResult(null, false, 5);

            EnterIdle();
            TransitionTo(new SpinningState());

            Assert.AreEqual(5, _wheel.LastTargetIndex);
        }

        [Test]
        public void HappyPath_SpinReward_SetsRandomStrategyForNormalZone()
        {
            _zone.ZoneTypeToReturn = ZoneType.Normal;
            _spin.ResultToReturn = new SpinResult(null, false, 0);

            EnterIdle();
            TransitionTo(new SpinningState());

            Assert.AreSame(_randomStrategy, _spin.LastStrategySet);
        }

        [Test]
        public void HappyPath_SpinReward_SetsWeightedStrategyForSuperZone()
        {
            _zone.ZoneTypeToReturn = ZoneType.Super;
            _spin.ResultToReturn = new SpinResult(null, false, 0);

            EnterIdle();
            TransitionTo(new SpinningState());

            Assert.AreSame(_weightedStrategy, _spin.LastStrategySet);
        }

        [Test]
        public void HappyPath_MultipleSpins_AccumulatesRewards()
        {
            _spin.ResultToReturn = new SpinResult(null, false, 0);

            EnterIdle();
            TransitionTo(new SpinningState());
            TransitionTo(new SpinningState());
            TransitionTo(new SpinningState());

            Assert.AreEqual(3, _reward.CollectedItems.Count);
        }

        // ── Bomb path: Idle → Spinning → Bomb → (revive or give up) ──────────

        [Test]
        public void BombPath_Spin_ShowsBombScreen()
        {
            _spin.ResultToReturn = new SpinResult(null, true, 0);

            EnterIdle();
            TransitionTo(new SpinningState());

            Assert.IsTrue(_dialog.BombScreenShown);
        }

        [Test]
        public void BombPath_Spin_ClearsAllRewards()
        {
            _reward.Collect(null);
            _reward.Collect(null);
            _spin.ResultToReturn = new SpinResult(null, true, 0);

            EnterIdle();
            TransitionTo(new SpinningState());

            Assert.AreEqual(1, _reward.ClearAllCallCount);
            Assert.AreEqual(0, _reward.CollectedItems.Count);
        }

        [Test]
        public void BombPath_Spin_EndsInBombState()
        {
            _spin.ResultToReturn = new SpinResult(null, true, 0);

            EnterIdle();
            TransitionTo(new SpinningState());

            Assert.IsInstanceOf<BombState>(_currentState);
        }

        [Test]
        public void BombPath_Revive_TransitionsToIdle()
        {
            _spin.ResultToReturn = new SpinResult(null, true, 0);

            EnterIdle();
            TransitionTo(new SpinningState());
            _dialog.SimulateRevive();

            Assert.IsInstanceOf<IdleState>(_currentState);
        }

        [Test]
        public void BombPath_Revive_DoesNotResetZone()
        {
            _zone.CurrentZone = 7;
            _spin.ResultToReturn = new SpinResult(null, true, 0);

            EnterIdle();
            TransitionTo(new SpinningState());
            _dialog.SimulateRevive();

            Assert.AreEqual(0, _zone.ResetCallCount);
        }

        [Test]
        public void BombPath_GiveUp_TransitionsToIdle()
        {
            _spin.ResultToReturn = new SpinResult(null, true, 0);

            EnterIdle();
            TransitionTo(new SpinningState());
            _dialog.SimulateGiveUp();

            Assert.IsInstanceOf<IdleState>(_currentState);
        }

        [Test]
        public void BombPath_GiveUp_ResetsZoneAndRewards()
        {
            _spin.ResultToReturn = new SpinResult(null, true, 0);

            EnterIdle();
            TransitionTo(new SpinningState());
            _dialog.SimulateGiveUp();

            Assert.AreEqual(1, _zone.ResetCallCount);
            Assert.AreEqual(1, _reward.ResetCallCount);
        }

        [Test]
        public void BombPath_ExitBombState_HidesDialog()
        {
            _spin.ResultToReturn = new SpinResult(null, true, 0);

            EnterIdle();
            TransitionTo(new SpinningState());
            _dialog.SimulateGiveUp();

            Assert.IsTrue(_dialog.HideCallCount);
        }

        // ── Collect path ──────────────────────────────────────────────────────

        [Test]
        public void CollectPath_Enter_ShowsConfirmScreen()
        {
            TransitionTo(new CollectState());

            Assert.IsTrue(_dialog.CollectScreenShown);
        }

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

        // ── SpinningState deferred callback ───────────────────────────────────

        [Test]
        public void SpinningState_DeferredCallback_StillTransitionsCorrectly()
        {
            _wheel.AutoInvokeCallback = false;
            _spin.ResultToReturn = new SpinResult(null, false, 1);

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
            _spin.ResultToReturn = new SpinResult(null, true, 0);

            EnterIdle();
            TransitionTo(new SpinningState());
            _wheel.InvokeCallback();

            Assert.IsInstanceOf<BombState>(_currentState);
        }
    }
}
