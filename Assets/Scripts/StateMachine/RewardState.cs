using WheelOfFortune.Domain;

namespace WheelOfFortune.StateMachine
{
    public sealed class RewardState : IGameState
    {
        private readonly SpinResult _result;

        public RewardState(SpinResult result)
        {
            _result = result;
        }

        public void Enter(GameContext ctx)
        {
            ctx.RewardService.Collect(_result.RewardItem);
            ctx.ZoneService.Advance();
            ctx.TransitionTo(new IdleState());
        }

        public void Exit(GameContext ctx) { }
    }
}