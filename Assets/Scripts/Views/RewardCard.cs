using TMPro;
using UnityEngine;
using UnityEngine.UI;
using WheelOfFortune.Domain;

namespace WheelOfFortune.Views
{
    public sealed class RewardCard : MonoBehaviour
    {
        [SerializeField] private Image _icon_value;
        [SerializeField] private TextMeshProUGUI _totalLabel_value;
        [SerializeField] private GameObject _countBadge;
        [SerializeField] private TextMeshProUGUI _countLabel_value;

        public void Setup(RewardStack stack)
        {
            if (_icon_value != null)
            {
                _icon_value.sprite = stack.Item != null ? stack.Item.Icon : null;
                _icon_value.enabled = stack.Item != null && stack.Item.Icon != null;
            }

            if (_totalLabel_value != null)
                _totalLabel_value.text = stack.TotalValue.ToString("F0");

            bool showBadge = stack.Count > 1;
            if (_countBadge != null)
                _countBadge.SetActive(showBadge);

            if (_countLabel_value != null)
                _countLabel_value.text = showBadge ? $"x{stack.Count}" : string.Empty;
        }
    }
}