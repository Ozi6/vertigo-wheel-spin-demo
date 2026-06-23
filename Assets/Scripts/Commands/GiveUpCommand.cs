using WheelOfFortune.Interfaces;
using WheelOfFortune.StateMachine;
using WheelOfFortune.Events;

namespace WheelOfFortune.Commands
{
    public sealed class GiveUpCommand : ICommand
    {
        private readonly IEventBus _eventBus;

        public GiveUpCommand(IEventBus eventBus)
        {
            _eventBus = eventBus;
        }

        public void Execute()
        {
            _eventBus.Publish(new OnStateTransition(new ResetState()));
        }
    }
}
