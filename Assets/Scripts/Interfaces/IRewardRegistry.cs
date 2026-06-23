using WheelOfFortune.Data;

namespace WheelOfFortune.Interfaces
{
    public interface IRewardRegistry
    {
        RewardItemSO GetReward(string id);
    }
}
