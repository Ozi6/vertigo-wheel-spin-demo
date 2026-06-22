using WheelOfFortune.StateMachine;

namespace WheelOfFortune.Events
{
    public struct OnStateTransition
    {
        public IGameState NewState { get; }
        public OnStateTransition(IGameState newState) => NewState = newState;
    }
}