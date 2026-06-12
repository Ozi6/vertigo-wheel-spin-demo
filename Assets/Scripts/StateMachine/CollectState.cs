using WheelOfFortune.Events;

namespace WheelOfFortune.StateMachine
{
    public sealed class CollectState : IGameState
    {
        public void Enter(GameContext ctx)
        {
            var finalRewards = ctx.RewardService.GetCurrentRewards().Clone();
            ctx.DialogView.ShowCollectConfirmScreen(
                onConfirm: () => OnConfirm(ctx, finalRewards),
                onCancel: () => OnCancel(ctx));
        }

        public void Exit(GameContext ctx)
        {
            ctx.DialogView.Hide();
        }

        private void OnConfirm(GameContext ctx, WheelOfFortune.Domain.CollectedRewards finalRewards)
        {
            ctx.ZoneService.Reset();
            ctx.RewardService.Reset();
            ctx.TransitionTo(new IdleState());
        }

        private void OnCancel(GameContext ctx)
        {
            ctx.TransitionTo(new IdleState());
        }
    }
}