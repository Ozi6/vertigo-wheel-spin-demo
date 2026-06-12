namespace WheelOfFortune.Domain
{
    public sealed class ZoneProgressModel
    {
        public readonly int ZoneNumber;
        public readonly ZoneType ZoneType;

        public ZoneProgressModel(int zoneNumber, ZoneType zoneType)
        {
            ZoneNumber = zoneNumber;
            ZoneType = zoneType;
        }
    }
}
