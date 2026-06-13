using System;
using UnityEngine;
using DG.Tweening;
using WheelOfFortune.Data;
using WheelOfFortune.Interfaces;

namespace WheelOfFortune.Views
{
    public sealed class WheelPresenter : MonoBehaviour, IWheelView
    {
        [SerializeField] private Transform _wheelRoot;
        [SerializeField] private float _spinDuration = 3f;
        [SerializeField] private int _extraSpins = 5;
        [SerializeField] private Ease _spinEase = Ease.OutQuart;

        private SliceDefinition[] _currentSlices;

        public void SetupSlices(SliceDefinition[] slices)
        {
            _currentSlices = slices;
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

            _wheelRoot.DORotate(new Vector3(0, 0, totalRotation), _spinDuration, RotateMode.FastBeyond360)
                .SetEase(_spinEase)
                .OnComplete(() => onComplete?.Invoke());
        }
    }
}