using WheelOfFortune.Domain;

namespace WheelOfFortune.StateMachine
{
    public sealed class SpinningState : IGameState
    {
        private SpinResult _pendingResult;

        public void Enter(GameContext ctx)
        {
            var zoneType = ctx.ZoneService.GetCurrentZoneType();
            var zoneNumber = ctx.ZoneService.GetCurrentZoneNumber();
            var strategy = ctx.RandomStrategy;

            ctx.SpinService.SetStrategy(strategy);

            var wheelData = ctx.WheelFactory.BuildWheel(zoneType, zoneNumber, ctx.WheelView);
            _pendingResult = ctx.SpinService.Spin(wheelData);

            ctx.WheelView.SpinTo(_pendingResult.SliceIndex, () => OnSpinAnimationComplete(ctx));
        }

        public void Exit(GameContext ctx) { }

        private void OnSpinAnimationComplete(GameContext ctx)
        {
            if (_pendingResult.IsBomb)
                ctx.TransitionTo(new BombState(ctx.ReviveCommand, ctx.GiveUpCommand));
            else
                ctx.TransitionTo(new RewardState(_pendingResult));
        }
    }
}