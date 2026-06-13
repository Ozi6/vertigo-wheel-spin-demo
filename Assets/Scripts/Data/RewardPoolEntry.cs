using System;
using UnityEngine;

namespace WheelOfFortune.Data
{
    [Serializable]
    public struct RewardPoolEntry
    {
        [SerializeField] private RewardItemSO _rewardItem;
        [SerializeField] private float _weight;
        [SerializeField] private float _zoneValueMultiplier;

        public RewardItemSO RewardItem => _rewardItem;
        public float Weight => _weight;
        public float ZoneValueMultiplier => _zoneValueMultiplier;
    }
}