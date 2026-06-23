using WheelOfFortune.Domain;

namespace WheelOfFortune.StateMachine
{
    public sealed class BombState : IGameState
    {
        private GameContext _ctx;
        private CollectedRewards _lostRewards;

        public void Enter(GameContext ctx)
        {
            _ctx = ctx;
            _lostRewards = _ctx.RewardService.GetCurrentRewards().Clone();
            _ctx.RewardService.ClearAll();

            bool canAfford = _ctx.CurrencyService.CanAfford(_ctx.ReviveCommand.CurrentCost);

            _ctx.DialogView.ShowBombScreen(
                _lostRewards,
                _ctx.ReviveCommand.CurrentCost,
                canAfford,
                onRevive: OnReviveClicked,
                onGiveUp: OnGiveUpClicked);
        }

        public void Exit(GameContext ctx)
        {
            ctx.DialogView.Hide();
        }

        private void OnReviveClicked()
        {
            foreach (var entry in _lostRewards.Entries)
                _ctx.RewardService.Collect(entry.Item, entry.Multiplier);
            _ctx.ReviveCommand.Execute();
        }

        private void OnGiveUpClicked()
        {
            _ctx.GiveUpCommand.Execute();
        }
    }
}
