using WheelOfFortune.Data;

namespace WheelOfFortune.Domain
{
    public readonly struct RuntimeWheelData
    {
        public readonly RuntimeSlice[] Slices;
        public readonly int BombSlotIndex;
        public readonly bool HasBomb;

        public RuntimeWheelData(RuntimeSlice[] slices, int bombSlotIndex, bool hasBomb)
        {
            Slices = slices;
            BombSlotIndex = bombSlotIndex;
            HasBomb = hasBomb;
        }
    }
}