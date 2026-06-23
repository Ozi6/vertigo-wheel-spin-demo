using WheelOfFortune.Domain;
using WheelOfFortune.Interfaces;

namespace WheelOfFortune.Interfaces
{
    public interface IWheelFactory
    {
        RuntimeWheelData BuildWheel(ZoneType zoneType, int zoneNumber, IWheelView wheelView);
    }
}