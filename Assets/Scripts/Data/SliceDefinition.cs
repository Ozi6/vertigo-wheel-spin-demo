using System;
using UnityEngine;

namespace WheelOfFortune.Data
{
    [Serializable]
    public struct SliceDefinition
    {
        [SerializeField] private RewardItemSO _rewardItem;
        [SerializeField] private int _multiplier;

        public RewardItemSO RewardItem => _rewardItem;
        public int Multiplier => _multiplier;
        public bool IsBomb => _rewardItem == null;

        public SliceDefinition(RewardItemSO rewardItem, int multiplier)
        {
            _rewardItem = rewardItem;
            _multiplier = multiplier;
        }
    }
}