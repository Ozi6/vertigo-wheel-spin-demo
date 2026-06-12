using WheelOfFortune.Commands;

namespace WheelOfFortune.StateMachine
{
    public sealed class BombState : IGameState
    {
        private readonly ReviveCommand _reviveCommand;
        private readonly GiveUpCommand _giveUpCommand;

        public BombState(ReviveCommand reviveCommand, GiveUpCommand giveUpCommand)
        {
            _reviveCommand = reviveCommand;
            _giveUpCommand = giveUpCommand;
        }

        public void Enter(GameContext ctx)
        {
            ctx.RewardService.ClearAll();
            ctx.DialogView.ShowBombScreen(
                onRevive: () => _reviveCommand.Execute(),
                onGiveUp: () => _giveUpCommand.Execute());
        }

        public void Exit(GameContext ctx)
        {
            ctx.DialogView.Hide();
        }
    }
}