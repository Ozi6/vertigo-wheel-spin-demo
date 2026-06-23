using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using WheelOfFortune.Domain;
using WheelOfFortune.Utility;

namespace WheelOfFortune.Views
{
    public sealed class RewardCard : MonoBehaviour
    {
        [SerializeField] private Image _icon_value;
        [SerializeField] private TextMeshProUGUI _multiplierLabel_value;

        [SerializeField, Min(0.01f)] private float _punchScaleAmount = 0.25f;
        [SerializeField, Min(0.01f)] private float _punchDuration = 0.22f;
        [SerializeField, Min(1)] private int _punchVibrato = 1;

        public void Setup(RewardStack stack, Sprite icon)
        {
            if (_icon_value != null)
            {
                _icon_value.sprite = icon;
                _icon_value.enabled = icon != null;
            }

            if (_multiplierLabel_value != null)
                _multiplierLabel_value.text = MultiplierFormatter.Format(stack.TotalMultiplier);
        }

        public void InitializeEmpty(RewardStack stack, Sprite icon)
        {
            if (_icon_value != null)
            {
                _icon_value.sprite = icon;
                _icon_value.enabled = icon != null;
            }

            if (_multiplierLabel_value != null)
                _multiplierLabel_value.text = MultiplierFormatter.Format(0);
        }

        private int _currentMultiplier;

        public void SetMultiplier(int value, bool punch = true)
        {
            _currentMultiplier = value;
            if (_multiplierLabel_value != null)
                _multiplierLabel_value.text = MultiplierFormatter.Format(_currentMultiplier);

            if (punch)
                PunchScale();
        }

        public void AddMultiplier(int increment, bool punch = true)
        {
            _currentMultiplier += increment;
            if (_multiplierLabel_value != null)
                _multiplierLabel_value.text = MultiplierFormatter.Format(_currentMultiplier);

            if (punch)
                PunchScale();
        }

        private void PunchScale()
        {
            transform.DOComplete();
            transform.localScale = Vector3.one;
            transform
                .DOPunchScale(Vector3.one * _punchScaleAmount, _punchDuration, _punchVibrato, 0.5f)
                .SetEase(Ease.OutQuad);
        }

        private void OnDisable()
        {
            transform.DOKill();
            transform.localScale = Vector3.one;
        }

        private void OnValidate()
        {
            if (_punchScaleAmount < 0.01f) _punchScaleAmount = 0.01f;
            if (_punchDuration < 0.01f) _punchDuration = 0.01f;
            if (_punchVibrato < 1) _punchVibrato = 1;
        }
    }
}