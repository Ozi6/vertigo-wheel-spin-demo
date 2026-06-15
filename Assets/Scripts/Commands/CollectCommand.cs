using System;
using WheelOfFortune.StateMachine;
using WheelOfFortune.Interfaces;

namespace WheelOfFortune.Commands
{
    public sealed class CollectCommand : ICommand
    {
        private readonly IGameStateGuard _guard;
        private readonly Action<IGameState> _transitionTo;

        public CollectCommand(IGameStateGuard guard, Action<IGameState> transitionTo)
        {
            _guard = guard;
            _transitionTo = transitionTo;
        }

        public void Execute()
        {
            if (!_guard.CanCollect()) return;
            _transitionTo(new CollectState());
        }
    }
}