namespace WheelOfFortune.Events
{
    public readonly struct OnRewardIconArrived
    {
        public readonly string ItemId;
        public readonly int MultiplierIncrement;

        public OnRewardIconArrived(string itemId, int multiplierIncrement)
        {
            ItemId = itemId;
            MultiplierIncrement = multiplierIncrement;
        }
    }
}