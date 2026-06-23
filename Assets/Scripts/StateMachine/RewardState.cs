using UnityEngine;
using WheelOfFortune.Domain;
using WheelOfFortune.Events;
using WheelOfFortune.Interfaces;

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
            _ctx.EventBus.Subscribe<OnWinEffectReelBack>(OnWinEffectReelBack);
            _ctx.EventBus.Subscribe<OnWinEffectCompleted>(OnWinEffectCompleted);

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

            Sprite itemIcon = null;
            if (!string.IsNullOrEmpty(_result.RewardItem.Id))
            {
                var so = _ctx.RewardRegistry.GetReward(_result.RewardItem.Id);
                if (so != null) itemIcon = so.Icon;
            }

            Transform panel = _ctx.HudView.GetRewardsPanelTarget();

            var payload = new WinEffectPayload(
                _result.SliceIndex,
                _result.Multiplier,
                itemIcon,
                panel,
                _ctx.WinEffectConfig,
                itemId,
                previousMultiplier,
                previousMultiplier + _result.Multiplier,
                _ctx.EventBus);

            _ctx.WheelView.PlayWinEffect(payload);
        }

        public void Exit(GameContext ctx)
        {
            ctx.EventBus.Unsubscribe<OnWinEffectReelBack>(OnWinEffectReelBack);
            ctx.EventBus.Unsubscribe<OnWinEffectCompleted>(OnWinEffectCompleted);
        }

        private void OnWinEffectReelBack(OnWinEffectReelBack evt)
        {
            _ctx.WheelView.RotateToOrigin(ReelBackDuration);
        }

        private void OnWinEffectCompleted(OnWinEffectCompleted evt)
        {
            _ctx.WheelFactory.BuildWheel(
                _ctx.ZoneService.GetCurrentZoneType(),
                _ctx.ZoneService.GetCurrentZoneNumber(),
                _ctx.WheelView);
            _ctx.EventBus.Publish(new OnStateTransition(new IdleState()));
        }
    }
}