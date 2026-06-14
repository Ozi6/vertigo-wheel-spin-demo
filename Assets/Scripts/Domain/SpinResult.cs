using WheelOfFortune.Data;

namespace WheelOfFortune.Domain
{
    public readonly struct SpinResult
    {
        public readonly RewardItemSO RewardItem;
        public readonly int Multiplier;
        public readonly bool IsBomb;
        public readonly int SliceIndex;

        public SpinResult(RewardItemSO rewardItem, int multiplier, bool isBomb, int sliceIndex)
        {
            RewardItem = rewardItem;
            Multiplier = multiplier;
            IsBomb = isBomb;
            SliceIndex = sliceIndex;
        }
    }
}