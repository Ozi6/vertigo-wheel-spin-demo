namespace WheelOfFortune.Domain
{
    public readonly struct RewardStack
    {
        public readonly RewardData Item;
        public readonly int TotalMultiplier;

        public RewardStack(RewardData item, int totalMultiplier)
        {
            Item = item;
            TotalMultiplier = totalMultiplier;
        }
    }
}