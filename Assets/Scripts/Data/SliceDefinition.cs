using System;
using UnityEngine;

namespace WheelOfFortune.Data
{
    [Serializable]
    public struct SliceDefinition
    {
        [SerializeField] private RewardItemSO _rewardItem;
        [SerializeField, Min(1)] private int _multiplier;

        public RewardItemSO RewardItem => _rewardItem;
        public int Multiplier => _multiplier;
        public bool IsBomb => _rewardItem == null;
        public float Weight => _rewardItem != null ? _rewardItem.Weight : 1f;

        public SliceDefinition(RewardItemSO rewardItem, int multiplier)
        {
            _rewardItem = rewardItem;
            _multiplier = Mathf.Max(1, multiplier);
        }
    }
}