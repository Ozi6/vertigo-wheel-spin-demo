using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace WheelOfFortune.Views
{
    public sealed class RewardCard : MonoBehaviour
    {
        [SerializeField] private Image _icon_value;
        [SerializeField] private TextMeshProUGUI _label_value;

        public void Setup(Sprite icon, string label)
        {
            if (_icon_value != null)
            {
                _icon_value.sprite = icon;
                _icon_value.enabled = icon != null;
            }

            if (_label_value != null)
                _label_value.text = label;
        }
    }
}