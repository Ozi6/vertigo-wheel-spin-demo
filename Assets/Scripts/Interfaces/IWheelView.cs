using System;
using WheelOfFortune.Data;

namespace WheelOfFortune.Interfaces
{
    public interface IWheelView
    {
        void SetupSlices(SliceDefinition[] slices);
        void SpinTo(int targetSliceIndex, Action onComplete);
    }
}