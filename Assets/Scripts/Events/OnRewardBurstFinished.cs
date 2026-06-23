namespace WheelOfFortune.Events
{
    public readonly struct OnRewardBurstFinished
    {
        public readonly string ItemId;
        public readonly int FinalMultiplier;

        public OnRewardBurstFinished(string itemId, int finalMultiplier)
        {
            ItemId = itemId;
            FinalMultiplier = finalMultiplier;
        }
    }
}