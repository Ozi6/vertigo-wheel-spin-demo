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

            ctx.ButtonView.SetSpinInteractable(true);
            ctx.ButtonView.SetCollectVisible(ctx.ZoneService.CanPlayerLeave());
        }

        public void Exit(GameContext ctx) { }

        public bool CanSpin() => true;

        public bool CanCollect() => _ctx != null && _ctx.ZoneService.CanPlayerLeave();
    }
}