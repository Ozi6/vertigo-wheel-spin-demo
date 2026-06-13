using DG.Tweening;
using WheelOfFortune.Domain;

namespace WheelOfFortune.StateMachine
{

    public sealed class RewardState : IGameState
    {

        private const float TotalTransitionDelay = 1.5f;

        private const float ReelBackDuration = TotalTransitionDelay * 0.75f;

        private readonly SpinResult _result;

        public RewardState(SpinResult result)
        {
            _result = result;
        }

        public void Enter(GameContext ctx)
        {

            ctx.RewardService.Collect(_result.RewardItem);
            ctx.ZoneService.Advance();

            ctx.WheelView.PlayWinEffect(
                _result.SliceIndex,
                onComplete: () => StartReelBack(ctx));
        }

        public void Exit(GameContext ctx) { }

        private static void StartReelBack(GameContext ctx)
        {

            ctx.WheelView.RotateToOrigin(ReelBackDuration);

            DOVirtual.DelayedCall(ReelBackDuration, () => RebuildAndIdle(ctx));
        }

        private static void RebuildAndIdle(GameContext ctx)
        {
            var zoneType = ctx.ZoneService.GetCurrentZoneType();
            var zoneNumber = ctx.ZoneService.GetCurrentZoneNumber();

            ctx.WheelFactory.BuildWheel(zoneType, zoneNumber, ctx.WheelView);
            ctx.TransitionTo(new IdleState());
        }
    }
}