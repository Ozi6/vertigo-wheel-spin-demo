using WheelOfFortune.Domain;

namespace WheelOfFortune.Events
{
    public readonly struct OnZoneAdvanced
    {
        public readonly ZoneProgressModel Progress;

        public OnZoneAdvanced(ZoneProgressModel progress)
        {
            Progress = progress;
        }
    }
}