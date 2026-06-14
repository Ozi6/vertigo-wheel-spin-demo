using WheelOfFortune.Data;

namespace WheelOfFortune.Domain
{
    public readonly struct RewardStack
    {
        public readonly RewardItemSO Item;
        public readonly int Count;
        public readonly float TotalValue;

        public RewardStack(RewardItemSO item, int count, float totalValue)
        {
            Item = item;
            Count = count;
            TotalValue = totalValue;
        }
    }
}