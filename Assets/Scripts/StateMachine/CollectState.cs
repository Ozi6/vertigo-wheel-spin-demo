using WheelOfFortune.Events;

namespace WheelOfFortune.StateMachine
{
    public sealed class CollectState : IGameState
    {
        private GameContext _ctx;

        public void Enter(GameContext ctx)
        {
            _ctx = ctx;
            _ctx.EventBus.Subscribe<OnCollectConfirmed>(OnCollectConfirmed);
            _ctx.EventBus.Subscribe<OnCollectCanceled>(OnCollectCanceled);

            var finalRewards = _ctx.RewardService.GetCurrentRewards().Clone();
            _ctx.EventBus.Publish(new OnPlayerLeft(finalRewards));

            _ctx.DialogView.ShowCollectConfirmScreen(finalRewards);
        }

        public void Exit(GameContext ctx)
        {
            ctx.EventBus.Unsubscribe<OnCollectConfirmed>(OnCollectConfirmed);
            ctx.EventBus.Unsubscribe<OnCollectCanceled>(OnCollectCanceled);
            ctx.DialogView.Hide();
        }

        private void OnCollectConfirmed(OnCollectConfirmed evt)
        {
            _ctx.EventBus.Publish(new OnStateTransition(new ResetState()));
        }

        private void OnCollectCanceled(OnCollectCanceled evt)
        {
            _ctx.EventBus.Publish(new OnStateTransition(new IdleState()));
        }
    }
}