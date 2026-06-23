using System;
using UnityEngine;
using WheelOfFortune.Data;

namespace WheelOfFortune.Domain
{
    public readonly struct WinEffectPayload
    {
        public readonly int WinningSliceIndex;
        public readonly int Multiplier;
        public readonly Sprite ItemIcon;
        public readonly Transform RewardsPanelTarget;
        public readonly WinEffectConfig Config;
        
        public readonly Action OnReelBack;
        public readonly Action OnComplete;
        public readonly Action<int> OnIconArrived;
        public readonly Action OnBurstFinished;

        public WinEffectPayload(
            int winningSliceIndex,
            int multiplier,
            Sprite itemIcon,
            Transform rewardsPanelTarget,
            WinEffectConfig config,
            Action onReelBack,
            Action onComplete,
            Action<int> onIconArrived,
            Action onBurstFinished)
        {
            WinningSliceIndex = winningSliceIndex;
            Multiplier = multiplier;
            ItemIcon = itemIcon;
            RewardsPanelTarget = rewardsPanelTarget;
            Config = config;
            OnReelBack = onReelBack;
            OnComplete = onComplete;
            OnIconArrived = onIconArrived;
            OnBurstFinished = onBurstFinished;
        }
    }
}
