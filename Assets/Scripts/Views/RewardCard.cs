using DG.Tweening;
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

        [SerializeField] private float _punchScaleAmount = 0.25f;
        [SerializeField] private float _punchDuration = 0.22f;
        [SerializeField] private int _punchVibrato = 1;

        private Tweener _punchTween;

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

        public void InitializeEmpty(RewardStack stack)
        {
            if (_icon_value != null)
            {
                _icon_value.sprite = stack.Item != null ? stack.Item.Icon : null;
                _icon_value.enabled = stack.Item != null && stack.Item.Icon != null;
            }

            if (_multiplierLabel_value != null)
                _multiplierLabel_value.text = "x0";
        }

        public void SetMultiplier(int value)
        {
            if (_multiplierLabel_value != null)
                _multiplierLabel_value.text = $"x{value}";

            PunchScale();
        }

        private void PunchScale()
        {
            _punchTween?.Kill(true);
            transform.localScale = Vector3.one;
            _punchTween = transform
                .DOPunchScale(Vector3.one * _punchScaleAmount, _punchDuration, _punchVibrato, 0.5f)
                .SetEase(Ease.OutQuad);
        }
    }
}