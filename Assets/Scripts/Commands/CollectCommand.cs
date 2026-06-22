using WheelOfFortune.StateMachine;
using WheelOfFortune.Interfaces;
using WheelOfFortune.Events;

namespace WheelOfFortune.Commands
{
    public sealed class CollectCommand : ICommand
    {
        private readonly IGameStateGuard _guard;
        private readonly IEventBus _eventBus;

        public CollectCommand(IGameStateGuard guard, IEventBus eventBus)
        {
            _guard = guard;
            _eventBus = eventBus;
        }

        public void Execute()
        {
            if (!_guard.CanCollect()) return;
            _eventBus.Publish(new OnStateTransition(new CollectState()));
        }
    }
}