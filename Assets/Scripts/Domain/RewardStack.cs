using WheelOfFortune.Data;

namespace WheelOfFortune.Domain
{
    public readonly struct RewardStack
    {
        public readonly RewardItemSO Item;
        public readonly int TotalMultiplier;

        public RewardStack(RewardItemSO item, int totalMultiplier)
        {
            Item = item;
            TotalMultiplier = totalMultiplier;
        }
    }
}