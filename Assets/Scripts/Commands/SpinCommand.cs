using System;
using WheelOfFortune.StateMachine;
using WheelOfFortune.Interfaces;

namespace WheelOfFortune.Commands
{
    public sealed class SpinCommand : ICommand
    {
        private readonly IGameStateGuard _guard;
        private readonly Action<IGameState> _transitionTo;

        public SpinCommand(IGameStateGuard guard, Action<IGameState> transitionTo)
        {
            _guard = guard;
            _transitionTo = transitionTo;
        }

        public void Execute()
        {
            if (!_guard.CanSpin()) return;
            _transitionTo(new SpinningState());
        }
    }
}