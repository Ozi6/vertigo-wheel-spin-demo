using UnityEngine;
using WheelOfFortune.Domain;

namespace WheelOfFortune.StateMachine
{
    public sealed class RewardState : IGameState
    {
        private const float ReelBackDuration = 1.125f;

        private readonly SpinResult _result;

        public RewardState(SpinResult result)
        {
            _result = result;
        }

        public void Enter(GameContext ctx)
        {
            ctx.RewardService.Collect(_result.RewardItem, _result.Multiplier);
            ctx.ZoneService.Advance();

            Sprite itemIcon = _result.RewardItem != null ? _result.RewardItem.Icon : null;
            Transform panel = ctx.HudView.GetRewardsPanelTarget();

            ctx.WheelView.PlayWinEffect(
                _result.SliceIndex,
                _result.Multiplier,
                itemIcon,
                panel,
                ctx.WinEffectConfig,
                onReelBack: () => ctx.WheelView.RotateToOrigin(ReelBackDuration),
                onComplete: () => RebuildAndIdle(ctx));
        }

        public void Exit(GameContext ctx) { }

        private static void RebuildAndIdle(GameContext ctx)
        {
            ctx.WheelFactory.BuildWheel(
                ctx.ZoneService.GetCurrentZoneType(),
                ctx.ZoneService.GetCurrentZoneNumber(),
                ctx.WheelView);
            ctx.TransitionTo(new IdleState());
        }
    }
}