using WheelOfFortune.Domain;

namespace WheelOfFortune.Interfaces
{
    public interface IZoneService
    {
        ZoneType GetCurrentZoneType();
        int GetCurrentZoneNumber();
        ZoneProgressModel Advance();
        void Reset();
        bool CanPlayerLeave();
    }
}