using UnityEngine;

namespace WheelOfFortune.Data
{
    [CreateAssetMenu(fileName = "WheelConfig", menuName = "WheelOfFortune/Wheel Config")]
    public class WheelConfigSO : ScriptableObject
    {
        [SerializeField] private RewardPoolEntry[] _rewardPool;
        [SerializeField] private int _sliceCount = 8;
        [SerializeField] private bool _hasBomb = true;

        public RewardPoolEntry[] RewardPool => _rewardPool;
        public int SliceCount => _sliceCount;
        public bool HasBomb => _hasBomb;
    }
}