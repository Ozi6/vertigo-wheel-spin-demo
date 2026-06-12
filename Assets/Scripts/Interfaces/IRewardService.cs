using WheelOfFortune.Data;
using WheelOfFortune.Domain;

namespace WheelOfFortune.Interfaces
{
    public interface IRewardService
    {
        void Collect(RewardItemSO item);
        void ClearAll();
        void Reset();
        CollectedRewards GetCurrentRewards();
    }
}
