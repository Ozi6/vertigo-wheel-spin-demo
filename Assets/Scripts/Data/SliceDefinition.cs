using System;
using UnityEngine;

namespace WheelOfFortune.Data
{
    [Serializable]
    public struct SliceDefinition
    {
        [SerializeField] private RewardItemSO _rewardItem;
        [SerializeField, Min(1)] private int _multiplier;
        [SerializeField] private float _weight;

        public RewardItemSO RewardItem => _rewardItem;
        public int Multiplier => _multiplier;
        public bool IsBomb => _rewardItem == null;
        public float Weight => _weight;

        public SliceDefinition(RewardItemSO rewardItem, int multiplier, float weight)
        {
            _rewardItem = rewardItem;
            _multiplier = Mathf.Max(1, multiplier);
            _weight = Mathf.Max(0.01f, weight);
        }
    }
}