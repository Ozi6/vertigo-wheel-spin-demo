using WheelOfFortune.Data;
using WheelOfFortune.Domain;
using WheelOfFortune.Events;
using WheelOfFortune.Interfaces;

namespace WheelOfFortune.Services
{
    public class ZoneService : IZoneService
    {
        private readonly GameSettingsSO _settings;
        private readonly IEventBus _eventBus;
        private int _currentZone;

        public ZoneService(GameSettingsSO settings, IEventBus eventBus)
        {
            _settings = settings;
            _eventBus = eventBus;
            _currentZone = 1;
        }

        public ZoneType GetCurrentZoneType()
        {
            return DeriveZoneType(_currentZone);
        }

        public int GetCurrentZoneNumber()
        {
            return _currentZone;
        }

        public ZoneProgressModel Advance()
        {
            _currentZone++;
            ZoneProgressModel model = new ZoneProgressModel(_currentZone, DeriveZoneType(_currentZone));
            _eventBus.Publish(new OnZoneAdvanced(model));
            return model;
        }

        public bool CanPlayerLeave()
        {
            ZoneType type = GetCurrentZoneType();
            return type == ZoneType.Safe || type == ZoneType.Super;
        }

        public void Reset()
        {
            _currentZone = 1;
        }

        private ZoneType DeriveZoneType(int zone)
        {
            if (zone % _settings.SuperZoneInterval == 0)
                return ZoneType.Super;
            if (zone % _settings.SafeZoneInterval == 0)
                return ZoneType.Safe;
            return ZoneType.Normal;
        }
    }
}