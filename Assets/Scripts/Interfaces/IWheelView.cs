using System;
using UnityEngine;
using WheelOfFortune.Data;

namespace WheelOfFortune.Interfaces
{
    public interface IWheelView
    {
        void SetupSlices(SliceDefinition[] slices);
        void SpinTo(int targetSliceIndex, Action onComplete);
        void SetZoneVisuals(Sprite wheelSprite, Sprite arrowSprite);
    }
}