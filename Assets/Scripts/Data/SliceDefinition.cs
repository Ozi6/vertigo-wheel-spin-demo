using System;
using UnityEngine;

namespace WheelOfFortune.Data
{
    [Serializable]
    public struct SliceDefinition
    {
        [SerializeField] private RewardItemSO _rewardItem;
        [SerializeField] private float _weight;

        public RewardItemSO RewardItem => _rewardItem;
        public float Weight => _weight;
    }
}
