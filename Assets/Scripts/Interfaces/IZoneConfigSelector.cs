using WheelOfFortune.Data;
using WheelOfFortune.Domain;

namespace WheelOfFortune.Interfaces
{
    public interface IZoneConfigSelector
    {
        ZoneConfigSO GetZoneConfig(ZoneType zoneType);
    }
}