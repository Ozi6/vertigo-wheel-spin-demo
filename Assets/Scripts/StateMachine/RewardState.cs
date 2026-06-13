using DG.Tweening;
using WheelOfFortune.Domain;

namespace WheelOfFortune.StateMachine
{
    public sealed class RewardState : IGameState
    {
        private const float TransitionDelay = 1.5f;

        private readonly SpinResult _result;

        public RewardState(SpinResult result)
        {
            _result = result;
        }

        public void Enter(GameContext ctx)
        {
            ctx.RewardService.Collect(_result.RewardItem);
            ctx.ZoneService.Advance();

            DOVirtual.DelayedCall(TransitionDelay, () =>
            {
                var zoneType = ctx.ZoneService.GetCurrentZoneType();
                var zoneNumber = ctx.ZoneService.GetCurrentZoneNumber();
                ctx.WheelView.ResetRotation();
                ctx.WheelFactory.BuildWheel(zoneType, zoneNumber, ctx.WheelView);
                ctx.TransitionTo(new IdleState());
            });
        }

        public void Exit(GameContext ctx) { }
    }
}