namespace WheelOfFortune.Domain
{
    public readonly struct RuntimeSlice
    {
        public readonly RewardData Reward;
        public readonly int Multiplier;
        public readonly bool IsBomb;
        public readonly float Weight;

        public RuntimeSlice(RewardData reward, int multiplier, bool isBomb, float weight)
        {
            Reward = reward;
            Multiplier = multiplier;
            IsBomb = isBomb;
            Weight = weight;
        }
    }
}
