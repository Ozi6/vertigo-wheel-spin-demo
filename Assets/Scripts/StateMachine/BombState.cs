using WheelOfFortune.Commands;
using WheelOfFortune.Domain;

namespace WheelOfFortune.StateMachine
{
    public sealed class BombState : IGameState
    {
        private readonly ReviveCommand _reviveCommand;
        private readonly GiveUpCommand _giveUpCommand;
        private CollectedRewards _lostRewards;

        public BombState(ReviveCommand reviveCommand, GiveUpCommand giveUpCommand)
        {
            _reviveCommand = reviveCommand;
            _giveUpCommand = giveUpCommand;
        }

        public void Enter(GameContext ctx)
        {
            _lostRewards = ctx.RewardService.GetCurrentRewards().Clone();
            ctx.RewardService.ClearAll();

            ctx.DialogView.ShowBombScreen(
                _lostRewards,
                onRevive: () => OnRevive(ctx),
                onGiveUp: () => _giveUpCommand.Execute());
        }

        public void Exit(GameContext ctx)
        {
            ctx.DialogView.Hide();
        }

        private void OnRevive(GameContext ctx)
        {
            _reviveCommand.Execute();
            foreach (var entry in _lostRewards.Entries)
                ctx.RewardService.Collect(entry.Item, entry.Multiplier);
        }
    }
}