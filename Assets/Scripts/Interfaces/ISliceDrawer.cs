using WheelOfFortune.Data;

namespace WheelOfFortune.Interfaces
{
    public interface ISliceDrawer
    {
        SliceDefinition[] DrawSlices(WheelConfigSO config);
    }
}