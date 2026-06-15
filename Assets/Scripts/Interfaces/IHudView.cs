using UnityEngine;
using WheelOfFortune.Domain;

namespace WheelOfFortune.Interfaces
{
    public interface IHudView
    {
        void UpdateZoneDisplay(ZoneProgressModel progress);
        void UpdateRewardsDisplay(CollectedRewards rewards);
        Transform GetRewardsPanelTarget();
    }
}