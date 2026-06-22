using System;
using UnityEngine;

namespace WheelOfFortune.Data
{
    [Serializable]
    public struct RewardPoolEntry
    {
        [SerializeField] private RewardItemSO _rewardItem;
        [SerializeField, Min(0.01f)] private float _weight;

        public RewardItemSO RewardItem => _rewardItem;
        public float Weight => _weight;
    }
}