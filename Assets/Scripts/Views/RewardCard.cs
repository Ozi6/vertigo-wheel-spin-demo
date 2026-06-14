using TMPro;
using UnityEngine;
using UnityEngine.UI;
using WheelOfFortune.Domain;

namespace WheelOfFortune.Views
{
    public sealed class RewardCard : MonoBehaviour
    {
        [SerializeField] private Image _icon_value;
        [SerializeField] private TextMeshProUGUI _multiplierLabel_value;

        public void Setup(RewardStack stack)
        {
            if (_icon_value != null)
            {
                _icon_value.sprite = stack.Item != null ? stack.Item.Icon : null;
                _icon_value.enabled = stack.Item != null && stack.Item.Icon != null;
            }

            if (_multiplierLabel_value != null)
                _multiplierLabel_value.text = $"x{stack.TotalMultiplier}";
        }
    }
}