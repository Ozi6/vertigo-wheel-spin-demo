namespace WheelOfFortune.Domain
{
    public readonly struct RuntimeWheelData
    {
        public readonly RuntimeSlice[] Slices;
        public readonly int BombSlotIndex;
        public readonly bool HasBomb;
        public readonly bool IsWeighted;

        public RuntimeWheelData(RuntimeSlice[] slices, int bombSlotIndex, bool hasBomb, bool isWeighted)
        {
            Slices = slices;
            BombSlotIndex = bombSlotIndex;
            HasBomb = hasBomb;
            IsWeighted = isWeighted;
        }
    }
}