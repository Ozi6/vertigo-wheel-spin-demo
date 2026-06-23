using UnityEngine;
using WheelOfFortune.Data;

namespace WheelOfFortune.Interfaces
{
    public readonly struct WinEffectPayload
    {
        public readonly int WinningSliceIndex;
        public readonly int Multiplier;
        public readonly Sprite ItemIcon;
        public readonly Transform RewardsPanelTarget;
        public readonly WinEffectConfig Config;
        
        public readonly string ItemId;
        public readonly int PreviousMultiplier;
        public readonly int RewardMultiplier;
        public readonly IEventBus EventBus;

        public WinEffectPayload(
            int winningSliceIndex,
            int multiplier,
            Sprite itemIcon,
            Transform rewardsPanelTarget,
            WinEffectConfig config,
            string itemId,
            int previousMultiplier,
            int rewardMultiplier,
            IEventBus eventBus)
        {
            WinningSliceIndex = winningSliceIndex;
            Multiplier = multiplier;
            ItemIcon = itemIcon;
            RewardsPanelTarget = rewardsPanelTarget;
            Config = config;
            ItemId = itemId;
            PreviousMultiplier = previousMultiplier;
            RewardMultiplier = rewardMultiplier;
            EventBus = eventBus;
        }
    }
}
