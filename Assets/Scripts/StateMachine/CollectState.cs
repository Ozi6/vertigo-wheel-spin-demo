using WheelOfFortune.Events;

namespace WheelOfFortune.StateMachine
{
    public sealed class CollectState : IGameState
    {
        public void Enter(GameContext ctx)
        {
            var finalRewards = ctx.RewardService.GetCurrentRewards().Clone();
            ctx.EventBus.Publish(new OnPlayerLeft(finalRewards));

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
            ctx.EventBus.Publish(new Events.OnStateTransition(new ResetState()));
        }

        private void OnCancel(GameContext ctx)
        {
            ctx.EventBus.Publish(new Events.OnStateTransition(new IdleState()));
        }
    }
}