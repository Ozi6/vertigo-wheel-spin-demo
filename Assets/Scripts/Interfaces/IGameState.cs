namespace WheelOfFortune.StateMachine
{
    public interface IGameState
    {
        void Enter(GameContext ctx);
        void Exit(GameContext ctx);
    }
}