using System;
using WheelOfFortune.Commands;
using WheelOfFortune.Interfaces;
using WheelOfFortune.StateMachine;

namespace WheelOfFortune.Factory
{
    public sealed class CommandFactory
    {
        public SpinCommand CreateSpinCommand(IGameStateGuard guard, Action<IGameState> transitionTo)
        {
            return new SpinCommand(guard, transitionTo);
        }

        public CollectCommand CreateCollectCommand(IGameStateGuard guard, Action<IGameState> transitionTo)
        {
            return new CollectCommand(guard, transitionTo);
        }
    }
}