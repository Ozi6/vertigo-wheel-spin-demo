using WheelOfFortune.Domain;
using WheelOfFortune.Events;

namespace WheelOfFortune.StateMachine
{
    public sealed class BombState : IGameState
    {
        private GameContext _ctx;
        private CollectedRewards _lostRewards;

        public void Enter(GameContext ctx)
        {
            _ctx = ctx;
            _ctx.EventBus.Subscribe<OnReviveRequested>(OnReviveRequested);
            _ctx.EventBus.Subscribe<OnGiveUpRequested>(OnGiveUpRequested);

            _lostRewards = _ctx.RewardService.GetCurrentRewards().Clone();
            _ctx.RewardService.ClearAll();

            bool canAfford = _ctx.CurrencyService.CanAfford(_ctx.ReviveCommand.CurrentCost);

            _ctx.DialogView.ShowBombScreen(
                _lostRewards,
                _ctx.ReviveCommand.CurrentCost,
                canAfford);
        }

        public void Exit(GameContext ctx)
        {
            ctx.EventBus.Unsubscribe<OnReviveRequested>(OnReviveRequested);
            ctx.EventBus.Unsubscribe<OnGiveUpRequested>(OnGiveUpRequested);
            ctx.DialogView.Hide();
        }

        private void OnReviveRequested(OnReviveRequested evt)
        {
            foreach (var entry in _lostRewards.Entries)
                _ctx.RewardService.Collect(entry.Item, entry.Multiplier);
            _ctx.ReviveCommand.Execute();
        }

        private void OnGiveUpRequested(OnGiveUpRequested evt)
        {
            _ctx.GiveUpCommand.Execute();
        }
    }
}