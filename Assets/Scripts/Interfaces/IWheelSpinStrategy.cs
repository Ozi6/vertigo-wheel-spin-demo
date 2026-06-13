using WheelOfFortune.Data;
using WheelOfFortune.Domain;

namespace WheelOfFortune.Interfaces
{
    public interface IWheelSpinStrategy
    {
        int GetWinningIndex(RuntimeWheelData wheelData);
    }
}