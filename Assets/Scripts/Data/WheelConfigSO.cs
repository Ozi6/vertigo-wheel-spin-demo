using UnityEngine;

namespace WheelOfFortune.Data
{
    [CreateAssetMenu(fileName = "WheelConfig", menuName = "WheelOfFortune/Wheel Config")]
    public class WheelConfigSO : ScriptableObject
    {
        [SerializeField] private SliceDefinition[] _slices;
        [SerializeField] private int _bombSlotIndex = -1;

        public SliceDefinition[] Slices => _slices;
        public int BombSlotIndex => _bombSlotIndex;
        public bool HasBomb => _bombSlotIndex >= 0 && _bombSlotIndex < _slices.Length;
    }
}
