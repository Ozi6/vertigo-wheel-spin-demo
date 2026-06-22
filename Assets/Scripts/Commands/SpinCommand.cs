using WheelOfFortune.StateMachine;
using WheelOfFortune.Interfaces;
using WheelOfFortune.Events;

namespace WheelOfFortune.Commands
{
    public sealed class SpinCommand : ICommand
    {
        private readonly IGameStateGuard _guard;
        private readonly IEventBus _eventBus;

        public SpinCommand(IGameStateGuard guard, IEventBus eventBus)
        {
            _guard = guard;
            _eventBus = eventBus;
        }

        public void Execute()
        {
            if (!_guard.CanSpin()) return;
            _eventBus.Publish(new OnStateTransition(new SpinningState()));
        }
    }
}