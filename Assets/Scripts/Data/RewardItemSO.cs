using UnityEngine;

namespace WheelOfFortune.Data
{
    [CreateAssetMenu(fileName = "RewardItem", menuName = "WheelOfFortune/Reward Item")]
    public class RewardItemSO : ScriptableObject
    {
        [SerializeField] private string _id;
        [SerializeField] private Sprite _icon;
        [SerializeField] private float _value;
        [SerializeField] private int _tier;

        public string Id => _id;
        public Sprite Icon => _icon;
        public float Value => _value;
        public int Tier => _tier;
    }
}
