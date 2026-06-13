using WheelOfFortune.Data;

namespace WheelOfFortune.Domain
{
    public readonly struct RuntimeWheelData
    {
        public readonly SliceDefinition[] Slices;
        public readonly int BombSlotIndex;
        public readonly bool HasBomb;

        public RuntimeWheelData(SliceDefinition[] slices, int bombSlotIndex, bool hasBomb)
        {
            Slices = slices;
            BombSlotIndex = bombSlotIndex;
            HasBomb = hasBomb;
        }
    }
}