using WheelOfFortune.Events;
using WheelOfFortune.Domain;

namespace WheelOfFortune.StateMachine
{
    public sealed class CollectState : IGameState
    {
        public void Enter(GameContext ctx)
        {
            var finalRewards = ctx.RewardService.GetCurrentRewards().Clone();
            ctx.DialogView.ShowCollectConfirmScreen(
                finalRewards,
                onConfirm: () => OnConfirm(ctx),
                onCancel: () => OnCancel(ctx));
        }

        public void Exit(GameContext ctx)
        {
            ctx.DialogView.Hide();
        }

        private void OnConfirm(GameContext ctx)
        {
            ctx.ZoneService.Reset();
            ctx.RewardService.Reset();

            ctx.WheelFactory.BuildWheel(
                ctx.ZoneService.GetCurrentZoneType(),
                ctx.ZoneService.GetCurrentZoneNumber(),
                ctx.WheelView);

            ctx.TransitionTo(new IdleState());
        }

        private void OnCancel(GameContext ctx)
        {
            ctx.TransitionTo(new IdleState());
        }
    }
}