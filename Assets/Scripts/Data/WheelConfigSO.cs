using UnityEngine;

namespace WheelOfFortune.Data
{
    [CreateAssetMenu(fileName = "WheelConfig", menuName = "WheelOfFortune/Wheel Config")]
    public class WheelConfigSO : ScriptableObject
    {
        [SerializeField] private RewardPoolEntry[] _rewardPool;
        [SerializeField, Min(1)] private int _sliceCount = 8;
        [SerializeField] private bool _hasBomb = true;
        [SerializeField, Min(1)] private int _minMultiplier = 1;
        [SerializeField, Min(1)] private int _maxMultiplier = 10;

        public RewardPoolEntry[] RewardPool => _rewardPool;
        public int SliceCount => _sliceCount;
        public bool HasBomb => _hasBomb;
        public int MinMultiplier => _minMultiplier;
        public int MaxMultiplier => _maxMultiplier;

        private void OnValidate()
        {
            if (_sliceCount < 1) _sliceCount = 1;
            if (_minMultiplier < 1) _minMultiplier = 1;
            if (_maxMultiplier < _minMultiplier)
                _maxMultiplier = _minMultiplier;
        }
    }
}