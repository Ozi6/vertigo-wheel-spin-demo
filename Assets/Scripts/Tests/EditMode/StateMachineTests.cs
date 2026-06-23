using NUnit.Framework;
using System;
using System.Collections.Generic;
using WheelOfFortune.Commands;
using WheelOfFortune.Domain;
using WheelOfFortune.Interfaces;
using WheelOfFortune.StateMachine;
using WheelOfFortune.Tests.EditMode.Stubs;
using WheelOfFortune.Events;
using WheelOfFortune.Services;

namespace WheelOfFortune.Tests.EditMode
{
    [TestFixture]
    public class StateMachineTests
    {
        private class StubEventBus : IEventBus
        {
            private readonly Dictionary<Type, Delegate> _handlers = new Dictionary<Type, Delegate>();

            public void Publish<T>(T payload)
            {
                if (_handlers.TryGetValue(typeof(T), out var existing))
                    (existing as Action<T>)?.Invoke(payload);
            }

            public void Subscribe<T>(Action<T> handler)
            {
                var key = typeof(T);
                if (_handlers.TryGetValue(key, out var existing))
                    _handlers[key] = (Action<T>)existing + handler;
                else
                    _handlers[key] = handler;
            }

            public void Unsubscribe<T>(Action<T> handler)
            {
                var key = typeof(T);
                if (!_handlers.TryGetValue(key, out var existing)) return;

                var updated = (Action<T>)existing - handler;
                if (updated == null)
                    _handlers.Remove(key);
                else
                    _handlers[key] = updated;
            }
        }

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
        private StubEventBus _eventBus;
        private StubRewardRegistry _rewardRegistry;

        private IGameState _currentState;
        private GameContext _ctx;
        private IReviveCommand _reviveCommand;
        private ICommand _giveUpCommand;

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
            _weightedStrategy = new StubSpinStrategy();
            _wheelFactory = new StubWheelFactory();
            _rewardRegistry = new StubRewardRegistry();

            _eventBus = new StubEventBus();
            _eventBus.Subscribe<OnStateTransition>(OnStateTransitionEvent);

            _wheel.Initialize(_eventBus);
            _hud.Initialize(_eventBus, _rewardRegistry);
            _dialog.Initialize(_eventBus, _rewardRegistry);

            _ctx = CreateGameContext();
            _reviveCommand = _ctx.ReviveCommand;
            _giveUpCommand = _ctx.GiveUpCommand;
        }

        private void OnStateTransitionEvent(OnStateTransition evt)
        {
            TransitionTo(evt.NewState);
        }

        private GameContext CreateGameContext()
        {
            var revive = new ReviveCommand(_currency, _eventBus, 25);
            var giveUp = new GiveUpCommand(_eventBus);

            return _ctx = new GameContext(
                _zone,
                _spin,
                _reward,
                _currency,
                _wheel,
                _hud,
                _dialog,
                _button,
                _wheelFactory,
                _eventBus,
                _randomStrategy,
                _weightedStrategy,
                _rewardRegistry,
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
            var previousState = _currentState;

            ctx.ReviveCommand.Execute();

            Assert.AreEqual(previousState, _currentState);
        }

        [Test]
        public void ReviveCommand_DoesNotDeductWhenInsufficientFunds()
        {
            _currency = new StubCurrencyService(10);
            var ctx = CreateGameContext();
            var initialBalance = _currency.GetBalance();

            ctx.ReviveCommand.Execute();

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

        // --- Mechanics Tests ---

        [Test]
        public void ResetState_ResetsServicesAndTransitionsToIdle()
        {
            _zone.CurrentZone = 10;
            _reward.CollectedItems.Add(new RewardData("gold", 10f, 1, 1f));

            TransitionTo(new ResetState());

            Assert.AreEqual(1, _zone.ResetCallCount);
            Assert.AreEqual(1, _reward.ResetCallCount);
            Assert.IsInstanceOf<IdleState>(_currentState);
        }

        [Test]
        public void SpinningState_SelectsWeightedStrategy_WhenWheelIsWeighted()
        {
            var slices = new[] { new RuntimeSlice(new RewardData("gold", 10f, 1, 1f), 1, false, 1f) };
            var wheelData = new RuntimeWheelData(slices, -1, false, true);
            _wheelFactory.DataToReturn = wheelData;

            TransitionTo(new SpinningState());

            Assert.AreSame(_weightedStrategy, _spin.LastStrategySet);
        }

        [Test]
        public void SpinningState_SelectsRandomStrategy_WhenWheelIsNotWeighted()
        {
            var slices = new[] { new RuntimeSlice(new RewardData("gold", 10f, 1, 1f), 1, false, 1f) };
            var wheelData = new RuntimeWheelData(slices, -1, false, false);
            _wheelFactory.DataToReturn = wheelData;

            TransitionTo(new SpinningState());

            Assert.AreSame(_randomStrategy, _spin.LastStrategySet);
        }

        [Test]
        public void SpinningState_TransitionsToBombState_OnBombResult()
        {
            var slices = new[] { new RuntimeSlice(default, 0, true, 1f) };
            var wheelData = new RuntimeWheelData(slices, 0, true, false);
            _wheelFactory.DataToReturn = wheelData;
            _spin.ResultToReturn = new SpinResult(default, 0, true, 0);

            TransitionTo(new SpinningState());
            _wheel.InvokeCallback();

            Assert.IsInstanceOf<BombState>(_currentState);
        }

        [Test]
        public void SpinningState_TransitionsToRewardState_OnRewardResult()
        {
            var slices = new[] { new RuntimeSlice(new RewardData("gold", 10f, 1, 1f), 1, false, 1f) };
            var wheelData = new RuntimeWheelData(slices, -1, false, false);
            _wheelFactory.DataToReturn = wheelData;
            _spin.ResultToReturn = new SpinResult(new RewardData("gold", 10f, 1, 1f), 1, false, 0);

            TransitionTo(new SpinningState());
            _wheel.InvokeCallback();

            Assert.IsInstanceOf<RewardState>(_currentState);
        }

        [Test]
        public void RewardState_CollectsRewardAndAdvancesZone()
        {
            var reward = new RewardData("gold", 10f, 1, 1f);
            var result = new SpinResult(reward, 2, false, 0);

            TransitionTo(new RewardState(result));

            Assert.AreEqual(1, _reward.CollectedItems.Count);
            Assert.AreEqual(1, _zone.AdvanceCallCount);
            Assert.AreEqual(1, _wheel.PlayWinEffectCallCount);
        }

        [Test]
        public void BombState_Entering_ClearsActiveRewards_ButStoresBackup()
        {
            _reward.CollectedItems.Add(new RewardData("gold", 10f, 1, 1f));

            TransitionTo(new BombState());

            Assert.AreEqual(0, _reward.CollectedItems.Count);
            Assert.IsTrue(_dialog.BombScreenShown);
        }

        [Test]
        public void BombState_Revive_RestoresBackupAndExecutesDeduction()
        {
            var rewardItem = new RewardData("gold", 10f, 1, 1f);
            _reward.CollectedItems.Add(rewardItem);

            TransitionTo(new BombState());
            _dialog.SimulateRevive();

            Assert.AreEqual(1, _reward.CollectedItems.Count);
            Assert.IsInstanceOf<IdleState>(_currentState);
        }
    }
}