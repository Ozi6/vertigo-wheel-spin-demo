using DG.Tweening;
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

            Sprite icon = _result.RewardItem != null ? _result.RewardItem.Icon : null;
            Transform panel = ctx.HudView.GetRewardsPanelTarget();

            ctx.WheelView.PlayWinEffect(
                _result.SliceIndex,
                _result.Multiplier,
                icon,
                panel,
                onReelBack: () => StartReelBack(ctx),
                onComplete: () => RebuildAndIdle(ctx));
        }

        public void Exit(GameContext ctx) { }

        private static void StartReelBack(GameContext ctx)
        {
            ctx.WheelView.RotateToOrigin(ReelBackDuration);
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