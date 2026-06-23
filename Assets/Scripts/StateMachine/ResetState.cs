using WheelOfFortune.Events;

namespace WheelOfFortune.StateMachine
{
    public sealed class ResetState : IGameState
    {
        public void Enter(GameContext ctx)
        {
            ctx.ZoneService.Reset();
            ctx.RewardService.Reset();
            ctx.ReviveCommand.Reset();

            ctx.WheelFactory.BuildWheel(
                ctx.ZoneService.GetCurrentZoneType(),
                ctx.ZoneService.GetCurrentZoneNumber(),
                ctx.WheelView);

            ctx.EventBus.Publish(new OnStateTransition(new IdleState()));
        }

        public void Exit(GameContext ctx) { }
    }
}
