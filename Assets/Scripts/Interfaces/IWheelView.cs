using UnityEngine;
using WheelOfFortune.Data;
using WheelOfFortune.Views;

namespace WheelOfFortune.Interfaces
{
    public interface IWheelView
    {
        void Initialize(IEventBus eventBus);
        void SetupSlices(SliceDefinition[] slices);
        void SpinTo(int targetSliceIndex);
        void SetZoneVisuals(Sprite wheelSprite, Sprite arrowSprite);
        void RotateToOrigin(float duration);
        void SetLiveSlices(WheelSlice[] slices);
        void PlayWinEffect(Domain.WinEffectPayload payload);
    }
}