using WheelOfFortune.Data;
using WheelOfFortune.Domain;
using WheelOfFortune.Events;
using WheelOfFortune.Interfaces;

namespace WheelOfFortune.Services
{
    public class RewardService : IRewardService
    {
        private readonly IEventBus _eventBus;
        private readonly CollectedRewards _rewards;

        public RewardService(IEventBus eventBus)
        {
            _eventBus = eventBus;
            _rewards = new CollectedRewards();
        }

        public void Collect(RewardItemSO item)
        {
            _rewards.Add(item);
            _eventBus.Publish(new OnRewardCollected(_rewards.Clone()));
        }

        public void ClearAll()
        {
            _rewards.Clear();
            _eventBus.Publish(new OnBombHit());
        }
        public void Reset()
        {
            //_rewards.Clear();
        }

        public CollectedRewards GetCurrentRewards()
        {
            return _rewards;
        }
    }
}