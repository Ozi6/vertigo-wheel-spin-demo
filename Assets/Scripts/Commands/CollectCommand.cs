using System;
using WheelOfFortune.StateMachine;
using WheelOfFortune.Interfaces;

namespace WheelOfFortune.Commands
{
    public sealed class CollectCommand : ICommand
    {
        private readonly IdleState _idleState;
        private readonly Action<IGameState> _transitionTo;

        public CollectCommand(IdleState idleState, Action<IGameState> transitionTo)
        {
            _idleState = idleState;
            _transitionTo = transitionTo;
        }

        public void Execute()
        {
            if (!_idleState.CanCollect()) return;
            _transitionTo(new CollectState());
        }
    }
}