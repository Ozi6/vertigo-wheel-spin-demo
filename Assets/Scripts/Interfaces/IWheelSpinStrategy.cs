using WheelOfFortune.Data;

namespace WheelOfFortune.Interfaces
{
    public interface IWheelSpinStrategy
    {
        int GetWinningIndex(WheelConfigSO config);
    }
}