using System;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using WheelOfFortune.Data;
using WheelOfFortune.Interfaces;

namespace WheelOfFortune.Views
{
    public sealed class WheelPresenter : MonoBehaviour, IWheelView
    {
        [SerializeField] private Transform _wheelRoot;
        [SerializeField] private Image _wheelImage_value;
        [SerializeField] private Image _arrowImage_value;
        [SerializeField] private float _spinDuration = 3f;
        [SerializeField] private int _extraSpins = 5;
        [SerializeField] private Ease _spinEase = Ease.OutQuart;

        private SliceDefinition[] _currentSlices;

        public void SetupSlices(SliceDefinition[] slices)
        {
            _currentSlices = slices;
        }

        public void SetZoneVisuals(Sprite wheelSprite, Sprite arrowSprite)
        {
            if (_wheelImage_value != null && wheelSprite != null)
                _wheelImage_value.sprite = wheelSprite;

            if (_arrowImage_value != null && arrowSprite != null)
                _arrowImage_value.sprite = arrowSprite;
        }

        public void ResetRotation()
        {
            _wheelRoot.localEulerAngles = Vector3.zero;
        }

        public void SpinTo(int targetSliceIndex, Action onComplete)
        {
            if (_currentSlices == null || _currentSlices.Length == 0)
            {
                onComplete?.Invoke();
                return;
            }

            float sliceAngle = 360f / _currentSlices.Length;
            float targetAngle = -(targetSliceIndex * sliceAngle);
            float totalRotation = targetAngle - (360f * _extraSpins);

            _wheelRoot.DORotate(
                    new Vector3(0, 0, totalRotation),
                    _spinDuration,
                    RotateMode.FastBeyond360)
                .SetEase(_spinEase)
                .OnComplete(() => onComplete?.Invoke());
        }
    }
}