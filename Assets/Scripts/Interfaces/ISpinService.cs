using WheelOfFortune.Data;
using WheelOfFortune.Domain;

namespace WheelOfFortune.Interfaces
{
    public interface ISpinService
    {
        SpinResult Spin(RuntimeWheelData wheelData);
        void SetStrategy(IWheelSpinStrategy strategy);
    }
}