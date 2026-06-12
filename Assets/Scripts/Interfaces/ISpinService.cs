using WheelOfFortune.Data;
using WheelOfFortune.Domain;

namespace WheelOfFortune.Interfaces
{
    public interface ISpinService
    {
        SpinResult Spin(WheelConfigSO config);
        void SetStrategy(IWheelSpinStrategy strategy);
    }
}