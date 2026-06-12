using WheelOfFortune.Data;

namespace WheelOfFortune.Services
{
    public interface IWheelSpinStrategy
    {
        int GetWinningIndex(WheelConfigSO config);
    }
}