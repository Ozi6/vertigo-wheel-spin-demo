using WheelOfFortune.Data;
using WheelOfFortune.Domain;

namespace WheelOfFortune.Factory
{
    public sealed class ZoneConfigSelector
    {
        private readonly ZoneConfigSO[] _zoneConfigs;

        public ZoneConfigSelector(ZoneConfigSO[] zoneConfigs)
        { 
            _zoneConfigs = zoneConfigs;
        }

        public ZoneConfigSO GetZoneConfig(ZoneType zoneType)
        { 
            if (_zoneConfigs == null) return null;
            foreach (var zoneConfig in _zoneConfigs)
            { 
                if (zoneConfig != null && zoneConfig.ZoneType == zoneType)
                    return zoneConfig;
            }
            return null;
        }
    }
}
