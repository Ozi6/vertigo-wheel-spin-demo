using WheelOfFortune.Events;

namespace WheelOfFortune.StateMachine
{
    public sealed class CollectState : IGameState
    {
        private GameContext _ctx;

        public void Enter(GameContext ctx)
        {
            _ctx = ctx;
            var finalRewards = _ctx.RewardService.GetCurrentRewards().Clone();
            _ctx.EventBus.Publish(new OnPlayerLeft(finalRewards));

            _ctx.DialogView.ShowCollectConfirmScreen(
                finalRewards,
                onConfirm: OnConfirmClicked,
                onCancel: OnCancelClicked);
        }

        public void Exit(GameContext ctx)
        {
            ctx.DialogView.Hide();
        }

        private void OnConfirmClicked()
        {
            _ctx.EventBus.Publish(new Events.OnStateTransition(new ResetState()));
        }

        private void OnCancelClicked()
        {
            _ctx.EventBus.Publish(new Events.OnStateTransition(new IdleState()));
        }
    }
}
