namespace WheelOfFortune.Domain
{
    public readonly struct RewardData
    {
        public readonly string Id;
        public readonly float Value;
        public readonly int Tier;
        public readonly float Weight;

        public RewardData(string id, float value, int tier, float weight)
        {
            Id = id;
            Value = value;
            Tier = tier;
            Weight = weight;
        }
    }
}
