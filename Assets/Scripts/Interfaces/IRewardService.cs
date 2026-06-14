using WheelOfFortune.Data;
using WheelOfFortune.Domain;

namespace WheelOfFortune.Interfaces
{
    public interface IRewardService
    {
        void Collect(RewardItemSO item, int multiplier);
        void ClearAll();
        void Reset();
        CollectedRewards GetCurrentRewards();
    }
}
