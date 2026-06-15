using WheelOfFortune.Domain;

namespace WheelOfFortune.StateMachine
{
    public sealed class BombState : IGameState
    {
        private CollectedRewards _lostRewards;

        public void Enter(GameContext ctx)
        {
            _lostRewards = ctx.RewardService.GetCurrentRewards().Clone();
            ctx.RewardService.ClearAll();

            ctx.DialogView.ShowBombScreen(
                _lostRewards,
                onRevive: () => OnRevive(ctx),
                onGiveUp: () => ctx.GiveUpCommand.Execute());
        }

        public void Exit(GameContext ctx)
        {
            ctx.DialogView.Hide();
        }

        private void OnRevive(GameContext ctx)
        {
            foreach (var entry in _lostRewards.Entries)
                ctx.RewardService.Collect(entry.Item, entry.Multiplier);
            ctx.ReviveCommand.Execute();
        }
    }
}