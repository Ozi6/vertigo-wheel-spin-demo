using UnityEngine;
using WheelOfFortune.Domain;

namespace WheelOfFortune.StateMachine
{
    public sealed class RewardState : IGameState
    {
        private const float ReelBackDuration = 1.125f;

        private readonly SpinResult _result;
        private GameContext _ctx;

        public RewardState(SpinResult result)
        {
            _result = result;
        }

        public void Enter(GameContext ctx)
        {
            _ctx = ctx;
            int previousMultiplier = 0;

            foreach (var entry in _ctx.RewardService.GetCurrentRewards().Entries)
            {
                if (!string.IsNullOrEmpty(entry.Item.Id) &&
                    entry.Item.Id == _result.RewardItem.Id)
                {
                    previousMultiplier += entry.Multiplier;
                }
            }

            _ctx.RewardService.Collect(_result.RewardItem, _result.Multiplier);
            _ctx.ZoneService.Advance();

            string itemId = _result.RewardItem.Id;

            _ctx.HudView.InitializeNewRewardCard(
                _ctx.RewardService.GetCurrentRewards(),
                itemId);

            var onIconArrived =
                _ctx.HudView.BuildIconArrivedCallback(
                    itemId,
                    previousMultiplier,
                    _result.Multiplier);

            var onBurstFinished =
                _ctx.HudView.BuildFinalMultiplierCallback(
                    itemId,
                    previousMultiplier + _result.Multiplier);

            Sprite itemIcon = null;
            if (!string.IsNullOrEmpty(_result.RewardItem.Id))
            {
                var so = _ctx.RewardRegistry.GetReward(_result.RewardItem.Id);
                if (so != null) itemIcon = so.Icon;
            }

            Transform panel = _ctx.HudView.GetRewardsPanelTarget();

            _ctx.WheelView.PlayWinEffect(
                _result.SliceIndex,
                _result.Multiplier,
                itemIcon,
                panel,
                _ctx.WinEffectConfig,
                onReelBack: OnReelBack,
                onComplete: OnComplete,
                onIconArrived: onIconArrived,
                onBurstFinished: onBurstFinished);
        }

        public void Exit(GameContext ctx) { }

        private void OnReelBack()
        {
            _ctx.WheelView.RotateToOrigin(ReelBackDuration);
        }

        private void OnComplete()
        {
            _ctx.WheelFactory.BuildWheel(
                _ctx.ZoneService.GetCurrentZoneType(),
                _ctx.ZoneService.GetCurrentZoneNumber(),
                _ctx.WheelView);
            _ctx.EventBus.Publish(new Events.OnStateTransition(new IdleState()));
        }
    }
}
