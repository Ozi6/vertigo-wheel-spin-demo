using WheelOfFortune.Domain;

namespace WheelOfFortune.Interfaces
{
    public interface IZoneService
    {
        ZoneType GetCurrentZoneType();
        ZoneProgressModel Advance();
        bool CanPlayerLeave();
    }
}