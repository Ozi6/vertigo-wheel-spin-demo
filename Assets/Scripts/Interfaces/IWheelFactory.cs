using WheelOfFortune.Domain;
using WheelOfFortune.Interfaces;

namespace WheelOfFortune.Factory
{
    public interface IWheelFactory
    {
        RuntimeWheelData BuildWheel(ZoneType zoneType, int zoneNumber, IWheelView wheelView);
    }
}