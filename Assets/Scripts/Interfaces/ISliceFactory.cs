using WheelOfFortune.Data;
using WheelOfFortune.Views;

namespace WheelOfFortune.Interfaces
{
    public interface ISliceFactory
    {
        WheelSlice[] CreateSlices(SliceDefinition[] slices, SlotDefinition[] slots);
        void ReturnSlices(WheelSlice[] slices);
        void Clear();
    }
}