using System;
using UnityEngine;
using WheelOfFortune.Domain;

namespace WheelOfFortune.Interfaces
{
    public interface IHudView
    {
        void UpdateZoneDisplay(ZoneProgressModel progress);
        void UpdateRewardsDisplay(CollectedRewards rewards);
        void UpdateCurrencyDisplay(int balance);
        Transform GetRewardsPanelTarget();
        void InitializeNewRewardCard(CollectedRewards rewards, string newItemId);
        Action<int> BuildIconArrivedCallback(string itemId, int previousMultiplier, int rewardMultiplier);
        Action BuildFinalMultiplierCallback(string itemId, int finalValue);
    }
}