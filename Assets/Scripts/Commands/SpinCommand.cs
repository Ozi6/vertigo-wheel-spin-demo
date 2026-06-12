using System;
using WheelOfFortune.StateMachine;
using WheelOfFortune.Interfaces;

namespace WheelOfFortune.Commands
{
    public sealed class SpinCommand : ICommand
    {
        private readonly IdleState _idleState;
        private readonly Action<IGameState> _transitionTo;

        public SpinCommand(IdleState idleState, Action<IGameState> transitionTo)
        {
            _idleState = idleState;
            _transitionTo = transitionTo;
        }

        public void Execute()
        {
            if (!_idleState.CanSpin()) return;
            _transitionTo(new SpinningState());
        }
    }
}