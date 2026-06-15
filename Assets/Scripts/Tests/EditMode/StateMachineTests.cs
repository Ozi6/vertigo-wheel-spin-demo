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
        private StubCurrencyService _currency;
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
            _currency = new StubCurrencyService(10000);
            _wheel = new StubWheelView();
            _hud = new StubHudView();
            _dialog = new StubDialogView();
            _button = new StubButtonView();
            _randomStrategy = new StubSpinStrategy();
            _wheelFactory = new StubWheelFactory();

            _ctx = CreateGameContext();
            _reviveCommand = new ReviveCommand(_ctx, 25);
            _giveUpCommand = new GiveUpCommand(_zone, _reward, TransitionTo);
        }

        private GameContext CreateGameContext()
        {
            var revive = new ReviveCommand(null, 25);
            var giveUp = new GiveUpCommand(_zone, _reward, TransitionTo);

            return new GameContext(
                _zone,
                _spin,
                _reward,
                _currency,
                _wheel,
                _hud,
                _dialog,
                _button,
                _wheelFactory,
                TransitionTo,
                _randomStrategy,
                revive,
                giveUp,
                null);
        }

        private void TransitionTo(IGameState next)
        {
            _currentState?.Exit(_ctx);
            _currentState = next;
            _currentState.Enter(_ctx);
        }

        private void EnterIdle() => TransitionTo(new IdleState());

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

        [Test]
        public void ReviveCommand_TransitionsToIdle()
        {
            _reviveCommand.Execute();
            Assert.IsInstanceOf<IdleState>(_currentState);
        }

        [Test]
        public void ReviveCommand_DeductsCurrencyOnSuccess()
        {
            var initialBalance = _currency.GetBalance();
            _reviveCommand.Execute();
            Assert.AreEqual(initialBalance - 25, _currency.GetBalance());
        }

        [Test]
        public void ReviveCommand_DoesNotTransitionWhenInsufficientFunds()
        {
            _currency = new StubCurrencyService(10);
            var ctx = CreateGameContext();
            var reviveCommand = new ReviveCommand(ctx, 25);
            var previousState = _currentState;

            reviveCommand.Execute();

            Assert.AreEqual(previousState, _currentState);
        }

        [Test]
        public void ReviveCommand_DoesNotDeductWhenInsufficientFunds()
        {
            _currency = new StubCurrencyService(10);
            var ctx = CreateGameContext();
            var reviveCommand = new ReviveCommand(ctx, 25);
            var initialBalance = _currency.GetBalance();

            reviveCommand.Execute();

            Assert.AreEqual(initialBalance, _currency.GetBalance());
        }

        [Test]
        public void GiveUpCommand_Execute_TransitionsToIdle()
        {
            _giveUpCommand.Execute();
            Assert.IsInstanceOf<IdleState>(_currentState);
        }

        [Test]
        public void GiveUpCommand_Execute_ResetsZoneAndRewards()
        {
            _giveUpCommand.Execute();
            Assert.AreEqual(1, _zone.ResetCallCount);
            Assert.AreEqual(1, _reward.ResetCallCount);
        }
    }
}