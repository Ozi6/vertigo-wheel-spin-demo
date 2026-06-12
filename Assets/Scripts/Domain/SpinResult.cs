using WheelOfFortune.Data;

namespace WheelOfFortune.Domain
{
    public readonly struct SpinResult
    {
        public readonly RewardItemSO RewardItem;
        public readonly bool IsBomb;
        public readonly int SliceIndex;

        public SpinResult(RewardItemSO rewardItem, bool isBomb, int sliceIndex)
        {
            RewardItem = rewardItem;
            IsBomb = isBomb;
            SliceIndex = sliceIndex;
        }
    }
}
