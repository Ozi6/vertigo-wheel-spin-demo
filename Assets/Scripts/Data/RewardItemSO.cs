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
        [SerializeField, Min(0.01f)] private float _weight = 1f;

        public string Id => _id;
        public Sprite Icon => _icon;
        public float Value => _value;
        public int Tier => _tier;
        public float Weight => _weight;

        public Domain.RewardData ToData() => new Domain.RewardData(_id, _value, _tier, _weight);

        private void OnValidate()
        {
            if (_value < 1f) _value = 1f;
            if (_tier < 1) _tier = 1;
            if (_weight < 0.01f) _weight = 0.01f;
        }
    }
}