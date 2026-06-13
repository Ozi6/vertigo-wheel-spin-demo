using System;
using UnityEngine;

namespace WheelOfFortune.Data
{
    [Serializable]
    public struct SliceDefinition
    {
        [SerializeField] private RewardItemSO _rewardItem;
        [SerializeField] private float _weight;
        [SerializeField] private float _scaledValue;

        public RewardItemSO RewardItem => _rewardItem;
        public float Weight => _weight;
        public float ScaledValue => _scaledValue;

        public SliceDefinition(RewardItemSO rewardItem, float weight, float scaledValue)
        {
            _rewardItem = rewardItem;
            _weight = weight;
            _scaledValue = scaledValue;
        }
    }
}