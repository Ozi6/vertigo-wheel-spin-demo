namespace WheelOfFortune.StateMachine
{
    public sealed class BombState : IGameState
    {
        public void Enter(GameContext ctx)
        {
            ctx.RewardService.ClearAll();
            ctx.DialogView.ShowBombScreen(
                onRevive: () => OnRevive(ctx),
                onGiveUp: () => OnGiveUp(ctx));
        }

        public void Exit(GameContext ctx)
        {
            ctx.DialogView.Hide();
        }

        private void OnRevive(GameContext ctx)
        {
            ctx.TransitionTo(new IdleState());
        }

        private void OnGiveUp(GameContext ctx)
        {
            ctx.ZoneService.Reset();
            ctx.RewardService.Reset();
            ctx.TransitionTo(new IdleState());
        }
    }
}