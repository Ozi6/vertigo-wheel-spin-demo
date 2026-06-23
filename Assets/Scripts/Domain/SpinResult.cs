namespace WheelOfFortune.Domain
{
    public readonly struct SpinResult
    {
        public readonly RewardData RewardItem;
        public readonly int Multiplier;
        public readonly bool IsBomb;
        public readonly int SliceIndex;

        public SpinResult(RewardData rewardItem, int multiplier, bool isBomb, int sliceIndex)
        {
            RewardItem = rewardItem;
            Multiplier = multiplier;
            IsBomb = isBomb;
            SliceIndex = sliceIndex;
        }
    }
}