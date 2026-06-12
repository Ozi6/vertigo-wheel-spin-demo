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

        private IGameState _lastTransitionTarget;
        private int _transitionCount;

        [SetUp]
        public void SetUp()
        {
            _zone = new StubZoneService();
            _reward = new StubRewardService();
            _lastTransitionTarget = null;
            _transitionCount = 0;
        }

        private void CaptureTransition(IGameState next)
        {
            _lastTransitionTarget = next;
            _transitionCount++;
        }

        private IdleState MakeActiveIdleState(StubZoneService zone)
        {
            var zone2 = zone;
            var reward = new StubRewardService();
            var hud = new StubHudView();
            var dialog = new StubDialogView();
            var randomStrategy = new StubSpinStrategy();
            var weightedStrategy = new StubSpinStrategy();
            var revive = new ReviveCommand(CaptureTransition, () => true);
            var giveUp = new GiveUpCommand(zone2, reward, CaptureTransition);

            var ctx = new GameContext(
                zone2, new StubSpinService(), reward,
                new StubWheelView(), hud, dialog,
                null, CaptureTransition,
                randomStrategy, weightedStrategy,
                revive, giveUp);

            var idle = new IdleState();
            idle.Enter(ctx);
            return idle;
        }

        // ── SpinCommand ───────────────────────────────────────────────────────

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

        // ── CollectCommand ────────────────────────────────────────────────────

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
        public void CollectCommand_Execute_AfterIdleExited_IsBlocked()
        {
            _zone.CanLeave = true;
            var idle = MakeActiveIdleState(_zone);
            idle.Exit(null);
            var cmd = new CollectCommand(idle, CaptureTransition);

            cmd.Execute();

            Assert.AreEqual(0, _transitionCount);
        }

        // ── ReviveCommand ─────────────────────────────────────────────────────

        [Test]
        public void ReviveCommand_Execute_WhenCanAfford_TransitionsToIdle()
        {
            var cmd = new ReviveCommand(CaptureTransition, () => true);

            cmd.Execute();

            Assert.IsInstanceOf<IdleState>(_lastTransitionTarget);
        }

        [Test]
        public void ReviveCommand_Execute_WhenCannotAfford_DoesNotTransition()
        {
            var cmd = new ReviveCommand(CaptureTransition, () => false);

            cmd.Execute();

            Assert.AreEqual(0, _transitionCount);
        }

        [Test]
        public void ReviveCommand_Execute_WhenCannotAfford_TargetIsNull()
        {
            var cmd = new ReviveCommand(CaptureTransition, () => false);

            cmd.Execute();

            Assert.IsNull(_lastTransitionTarget);
        }

        [Test]
        public void ReviveCommand_GuardIsReevaluatedEachCall()
        {
            var canAfford = false;
            var cmd = new ReviveCommand(CaptureTransition, () => canAfford);

            cmd.Execute();
            Assert.AreEqual(0, _transitionCount);

            canAfford = true;
            cmd.Execute();
            Assert.AreEqual(1, _transitionCount);
        }

        // ── GiveUpCommand ─────────────────────────────────────────────────────

        [Test]
        public void GiveUpCommand_Execute_TransitionsToIdle()
        {
            var cmd = new GiveUpCommand(_zone, _reward, CaptureTransition);

            cmd.Execute();

            Assert.IsInstanceOf<IdleState>(_lastTransitionTarget);
        }

        [Test]
        public void GiveUpCommand_Execute_ResetsZoneService()
        {
            var cmd = new GiveUpCommand(_zone, _reward, CaptureTransition);

            cmd.Execute();

            Assert.AreEqual(1, _zone.ResetCallCount);
        }

        [Test]
        public void GiveUpCommand_Execute_ResetsRewardService()
        {
            var cmd = new GiveUpCommand(_zone, _reward, CaptureTransition);

            cmd.Execute();

            Assert.AreEqual(1, _reward.ResetCallCount);
        }

        [Test]
        public void GiveUpCommand_Execute_HasNoGuard_AlwaysRuns()
        {
            var cmd = new GiveUpCommand(_zone, _reward, CaptureTransition);

            cmd.Execute();
            cmd.Execute();

            Assert.AreEqual(2, _transitionCount);
            Assert.AreEqual(2, _zone.ResetCallCount);
        }

        // ── Cross-command guard isolation ─────────────────────────────────────

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
