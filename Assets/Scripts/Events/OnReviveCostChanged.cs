namespace WheelOfFortune.Events
{
    public readonly struct OnReviveCostChanged
    {
        public readonly int NextCost;
        public OnReviveCostChanged(int nextCost) => NextCost = nextCost;
    }
}
