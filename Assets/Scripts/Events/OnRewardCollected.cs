using WheelOfFortune.Domain;

namespace WheelOfFortune.Events
{
    public readonly struct OnRewardCollected
    {
        public readonly CollectedRewards Snapshot;

        public OnRewardCollected(CollectedRewards snapshot)
        {
            Snapshot = snapshot;
        }
    }
}