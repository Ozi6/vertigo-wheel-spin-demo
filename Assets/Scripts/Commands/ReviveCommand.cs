using System;
using WheelOfFortune.Interfaces;
using WheelOfFortune.StateMachine;

namespace WheelOfFortune.Commands
{
    public sealed class ReviveCommand : ICommand
    {
        private readonly Action<IGameState> _transitionTo;
        private readonly Func<bool> _canAffordRevive;

        public ReviveCommand(Action<IGameState> transitionTo, Func<bool> canAffordRevive)
        {
            _transitionTo = transitionTo;
            _canAffordRevive = canAffordRevive;
        }

        public void Execute()
        {
            if (!_canAffordRevive()) return;
            _transitionTo(new IdleState());
        }
    }
}
