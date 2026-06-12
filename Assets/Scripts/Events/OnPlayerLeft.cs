using WheelOfFortune.Domain;

namespace WheelOfFortune.Events
{
    public readonly struct OnPlayerLeft
    {
        public readonly CollectedRewards FinalRewards;

        public OnPlayerLeft(CollectedRewards finalRewards)
        {
            FinalRewards = finalRewards;
        }
    }
}
