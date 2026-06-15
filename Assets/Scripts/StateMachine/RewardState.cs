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
            int previousMultiplier = 0;

            foreach (var entry in ctx.RewardService.GetCurrentRewards().Entries)
            {
                if (entry.Item != null &&
                    entry.Item.Id == _result.RewardItem.Id)
                {
                    previousMultiplier += entry.Multiplier;
                }
            }

            ctx.RewardService.Collect(_result.RewardItem, _result.Multiplier);
            ctx.ZoneService.Advance();

            string itemId = _result.RewardItem.Id;

            ctx.HudView.InitializeNewRewardCard(
                ctx.RewardService.GetCurrentRewards(),
                itemId);

            var onIconArrived =
                ctx.HudView.BuildIconArrivedCallback(
                    itemId,
                    previousMultiplier,
                    _result.Multiplier);

            var onBurstFinished =
                ctx.HudView.BuildFinalMultiplierCallback(
                    itemId,
                    previousMultiplier + _result.Multiplier);

            Sprite itemIcon = _result.RewardItem != null
                ? _result.RewardItem.Icon
                : null;

            Transform panel = ctx.HudView.GetRewardsPanelTarget();

            ctx.WheelView.PlayWinEffect(
                _result.SliceIndex,
                _result.Multiplier,
                itemIcon,
                panel,
                ctx.WinEffectConfig,
                onReelBack: () => ctx.WheelView.RotateToOrigin(ReelBackDuration),
                onComplete: () => RebuildAndIdle(ctx),
                onIconArrived: onIconArrived,
                onBurstFinished: onBurstFinished);
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