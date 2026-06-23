using WheelOfFortune.Data;
using WheelOfFortune.Domain;

using WheelOfFortune.Interfaces;

namespace WheelOfFortune.Factory
{
    public sealed class ZoneConfigSelector : IZoneConfigSelector
    {
        private readonly ZoneConfigSO[] _zoneConfigs;

        public ZoneConfigSelector(ZoneConfigSO[] zoneConfigs)
        {
            _zoneConfigs = zoneConfigs;
        }

        public ZoneConfigSO GetZoneConfig(ZoneType zoneType)
        {
            foreach (var zoneConfig in _zoneConfigs)
            {
                if (zoneConfig.ZoneType == zoneType)
                    return zoneConfig;
            }
            return null;
        }
    }
}