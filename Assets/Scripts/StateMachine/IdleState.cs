using WheelOfFortune.Domain;

namespace WheelOfFortune.StateMachine
{
    public sealed class IdleState : IGameState
    {
        private GameContext _ctx;

        public void Enter(GameContext ctx)
        {
            _ctx = ctx;
            var progress = new ZoneProgressModel(
                ctx.ZoneService.GetCurrentZoneNumber(),
                ctx.ZoneService.GetCurrentZoneType());
            ctx.HudView.UpdateZoneDisplay(progress);
            ctx.HudView.UpdateRewardsDisplay(ctx.RewardService.GetCurrentRewards());
        }

        public void Exit(GameContext ctx)
        {
            _ctx = null;
        }

        public bool CanSpin() => true;

        public bool CanCollect() => _ctx != null && _ctx.ZoneService.CanPlayerLeave();
    }
}