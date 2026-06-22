using UnityEngine;

namespace WheelOfFortune.Data
{
    [CreateAssetMenu(fileName = "RewardItem", menuName = "WheelOfFortune/Reward Item")]
    public class RewardItemSO : ScriptableObject
    {
        [SerializeField] private string _id;
        [SerializeField] private Sprite _icon;
        [SerializeField, Min(1f)] private float _value;
        [SerializeField, Min(1)] private int _tier;

        public string Id => _id;
        public Sprite Icon => _icon;
        public float Value => _value;
        public int Tier => _tier;

        private void OnValidate()
        {
            if (_value < 1f) _value = 1f;
            if (_tier < 1) _tier = 1;
        }
    }
}