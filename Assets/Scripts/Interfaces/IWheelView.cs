using System;
using UnityEngine;
using WheelOfFortune.Data;
using WheelOfFortune.Views;

namespace WheelOfFortune.Interfaces
{
    public interface IWheelView
    {
        void SetupSlices(SliceDefinition[] slices);
        void SpinTo(int targetSliceIndex, Action onComplete);
        void SetZoneVisuals(Sprite wheelSprite, Sprite arrowSprite);
        void RotateToOrigin(float duration);

        void SetLiveSlices(WheelSlice[] slices);
        void PlayWinEffect(
            int winningSliceIndex,
            int multiplier,
            Sprite itemIcon,
            Transform rewardsPanelTarget,
            WinEffectConfig cfg,
            Action onReelBack,
            Action onComplete,
            Action<int> onIconArrived,
            Action onBurstFinished);
    }
}