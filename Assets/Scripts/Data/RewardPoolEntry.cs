using System;
using UnityEngine;

namespace WheelOfFortune.Data
{
    [Serializable]
    public struct RewardPoolEntry
    {
        [SerializeField] private RewardItemSO _rewardItem;
        [SerializeField] private bool _overrideWeight;
        [SerializeField, Min(0.01f)] private float _weight;

        public RewardItemSO RewardItem => _rewardItem;
        
        public float Weight 
        {
            get 
            {
                if (_overrideWeight) return _weight;
                return _rewardItem != null ? _rewardItem.Weight : 1f;
            }
        }
    }
}
