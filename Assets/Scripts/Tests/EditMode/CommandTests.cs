using NUnit.Framework;
using WheelOfFortune.Commands;
using WheelOfFortune.Interfaces;
using WheelOfFortune.StateMachine;
using WheelOfFortune.Tests.EditMode.Stubs;

namespace WheelOfFortune.Tests.EditMode
{
    [TestFixture]
    public class CommandTests
    {
        private StubZoneService _zone;
        private StubRewardService _reward;
        private StubCurrencyService _currency;

        private IGameState _lastTransitionTarget;
        private int _transitionCount;

        [SetUp]
        public void SetUp()
        {
            _zone = new StubZoneService();
            _reward = new StubRewardService();
            _currency = new StubCurrencyService(10000);
            _lastTransitionTarget = null;
            _transitionCount = 0;
        }

        private void CaptureTransition(IGameState next)
        {
            _lastTransitionTarget = next;
            _transitionCount++;
        }

        private GameContext CreateGameContext(
            StubZoneService zone,
            StubRewardService reward,
            StubCurrencyService currency,
            StubHudView hud,
            StubDialogView dialog,
            StubSpinStrategy randomStrategy,
            StubButtonView buttonView = null)
        {
            GameContext context = null;

            var revive = new ReviveCommand(() => context, 25);
            var giveUp = new GiveUpCommand(zone, reward, CaptureTransition, revive.Reset);

            context = new GameContext(
                zone, new StubSpinService(), reward, currency,
                new StubWheelView(), hud, dialog,
                buttonView ?? new StubButtonView(),
                null, CaptureTransition,
                randomStrategy,
                revive, giveUp, null);

            return context;
        }

        private IdleState MakeActiveIdleState(StubZoneService zone)
        {
            var ctx = CreateGameContext(
                zone,
                new StubRewardService(),
                new StubCurrencyService(10000),
                new StubHudView(),
                new StubDialogView(),
                new StubSpinStrategy());

            var idle = new IdleState();
            idle.Enter(ctx);
            return idle;
        }

        [Test]
        public void SpinCommand_Execute_TransitionsToSpinningState()
        {
            var idle = MakeActiveIdleState(_zone);
            var cmd = new SpinCommand(idle, CaptureTransition);

            cmd.Execute();

            Assert.IsInstanceOf<SpinningState>(_lastTransitionTarget);
        }

        [Test]
        public void SpinCommand_CanSpin_AlwaysAllowsSpin()
        {
            var idle = MakeActiveIdleState(_zone);
            var cmd = new SpinCommand(idle, CaptureTransition);

            cmd.Execute();
            cmd.Execute();

            Assert.AreEqual(2, _transitionCount);
        }

        [Test]
        public void SpinCommand_AfterIdleExited_CanStillSpin()
        {
            var idle = MakeActiveIdleState(_zone);
            idle.Exit(null);
            var cmd = new SpinCommand(idle, CaptureTransition);

            cmd.Execute();

            Assert.AreEqual(1, _transitionCount);
        }

        [Test]
        public void CollectCommand_Execute_WhenCanLeave_TransitionsToCollectState()
        {
            _zone.CanLeave = true;
            var idle = MakeActiveIdleState(_zone);
            var cmd = new CollectCommand(idle, CaptureTransition);

            cmd.Execute();

            Assert.IsInstanceOf<CollectState>(_lastTransitionTarget);
        }

        [Test]
        public void CollectCommand_Execute_WhenCannotLeave_DoesNotTransition()
        {
            _zone.CanLeave = false;
            var idle = MakeActiveIdleState(_zone);
            var cmd = new CollectCommand(idle, CaptureTransition);

            cmd.Execute();

            Assert.AreEqual(0, _transitionCount);
        }

        [Test]
        public void CollectCommand_Execute_WhenCannotLeave_TargetIsNull()
        {
            _zone.CanLeave = false;
            var idle = MakeActiveIdleState(_zone);
            var cmd = new CollectCommand(idle, CaptureTransition);

            cmd.Execute();

            Assert.IsNull(_lastTransitionTarget);
        }

        [Test]
        public void ReviveCommand_Execute_WhenCanAfford_TransitionsToIdle()
        {
            var ctx = CreateGameContext(_zone, _reward, _currency, new StubHudView(), new StubDialogView(), new StubSpinStrategy());

            ctx.ReviveCommand.Execute();

            Assert.IsInstanceOf<IdleState>(_lastTransitionTarget);
        }

        [Test]
        public void ReviveCommand_Execute_WhenCannotAfford_DoesNotTransition()
        {
            _currency = new StubCurrencyService(10);
            var ctx = CreateGameContext(_zone, _reward, _currency, new StubHudView(), new StubDialogView(), new StubSpinStrategy());

            ctx.ReviveCommand.Execute();

            Assert.AreEqual(0, _transitionCount);
        }

        [Test]
        public void ReviveCommand_Execute_WhenCannotAfford_TargetIsNull()
        {
            _currency = new StubCurrencyService(10);
            var ctx = CreateGameContext(_zone, _reward, _currency, new StubHudView(), new StubDialogView(), new StubSpinStrategy());

            ctx.ReviveCommand.Execute();

            Assert.IsNull(_lastTransitionTarget);
        }

        [Test]
        public void ReviveCommand_Execute_DeductsCost()
        {
            var ctx = CreateGameContext(_zone, _reward, _currency, new StubHudView(), new StubDialogView(), new StubSpinStrategy());
            var initialBalance = _currency.GetBalance();

            ctx.ReviveCommand.Execute();

            Assert.AreEqual(initialBalance - 25, _currency.GetBalance());
        }

        [Test]
        public void ReviveCommand_Execute_DoublesCostForNextRevive()
        {
            var buttonView = new StubButtonView();
            var ctx = CreateGameContext(_zone, _reward, _currency, new StubHudView(), new StubDialogView(), new StubSpinStrategy(), buttonView);

            ctx.ReviveCommand.Execute();

            Assert.AreEqual(50, buttonView.LastReviveCost);
        }

        [Test]
        public void GiveUpCommand_Execute_TransitionsToIdle()
        {
            var cmd = new GiveUpCommand(_zone, _reward, CaptureTransition, () => { });

            cmd.Execute();

            Assert.IsInstanceOf<IdleState>(_lastTransitionTarget);
        }

        [Test]
        public void GiveUpCommand_Execute_ResetsZoneService()
        {
            var cmd = new GiveUpCommand(_zone, _reward, CaptureTransition, () => { });

            cmd.Execute();

            Assert.AreEqual(1, _zone.ResetCallCount);
        }

        [Test]
        public void GiveUpCommand_Execute_ResetsRewardService()
        {
            var cmd = new GiveUpCommand(_zone, _reward, CaptureTransition, () => { });

            cmd.Execute();

            Assert.AreEqual(1, _reward.ResetCallCount);
        }

        [Test]
        public void GiveUpCommand_Execute_HasNoGuard_AlwaysRuns()
        {
            var cmd = new GiveUpCommand(_zone, _reward, CaptureTransition, () => { });

            cmd.Execute();
            cmd.Execute();

            Assert.AreEqual(2, _transitionCount);
            Assert.AreEqual(2, _zone.ResetCallCount);
        }

        [Test]
        public void SpinAndCollect_GuardsAreIndependent()
        {
            _zone.CanLeave = false;
            var idle = MakeActiveIdleState(_zone);
            var spinCmd = new SpinCommand(idle, CaptureTransition);
            var collectCmd = new CollectCommand(idle, CaptureTransition);

            spinCmd.Execute();
            collectCmd.Execute();

            Assert.AreEqual(1, _transitionCount);
            Assert.IsInstanceOf<SpinningState>(_lastTransitionTarget);
        }
    }
}