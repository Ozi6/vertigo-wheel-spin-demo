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
        [SerializeField] private HudPresenter _hudPresenter;

        private SliceDefinition[] _currentSlices;
        private WheelSlice[] _liveSlices;

        public void SetupSlices(SliceDefinition[] slices) => _currentSlices = slices;
        public void SetLiveSlices(WheelSlice[] slices) => _liveSlices = slices;

        public void SetZoneVisuals(Sprite wheelSprite, Sprite arrowSprite)
        {
            if (_wheelImage_value != null && wheelSprite != null)
                _wheelImage_value.sprite = wheelSprite;

            if (_arrowImage_value != null && arrowSprite != null)
                _arrowImage_value.sprite = arrowSprite;
        }

        public void RotateToOrigin(float duration)
        {
            _wheelRoot
                .DORotate(Vector3.zero, duration, RotateMode.Fast)
                .SetEase(Ease.InOutSine);
        }

        public void SpinTo(int targetSliceIndex, Action onComplete)
        {
            if (_currentSlices == null || _currentSlices.Length == 0)
            {
                onComplete?.Invoke();
                return;
            }

            float sliceAngle = 360f / _currentSlices.Length;
            float targetAngle = targetSliceIndex * sliceAngle;
            float currentZ = _wheelRoot.localEulerAngles.z;
            float normalizedZ = currentZ > 180f ? currentZ - 360f : currentZ;
            float delta = targetAngle - normalizedZ;
            if (delta < 0f) delta += 360f;

            _wheelRoot.DORotate(
                    new Vector3(0, 0, normalizedZ + delta + 360f * _extraSpins),
                    _spinDuration,
                    RotateMode.FastBeyond360)
                .SetEase(_spinEase)
                .OnComplete(() => onComplete?.Invoke());
        }

        public void PlayWinEffect(
            int winningSliceIndex,
            int multiplier,
            Sprite itemIcon,
            Transform rewardsPanelTarget,
            WinEffectConfig cfg,
            Action onReelBack,
            Action onComplete,
            Action<int> onIconArrived)
        {
            if (_liveSlices == null || _liveSlices.Length == 0 ||
                winningSliceIndex < 0 || winningSliceIndex >= _liveSlices.Length)
            {
                onReelBack?.Invoke();
                onComplete?.Invoke();
                return;
            }

            var winningSlice = _liveSlices[winningSliceIndex];
            if (winningSlice == null)
            {
                onReelBack?.Invoke();
                onComplete?.Invoke();
                return;
            }

            var effectRoot = _wheelRoot.parent != null ? _wheelRoot.parent : _wheelRoot;

            SlotZoomEffect.Play(
                effectRoot,
                winningSlice,
                _liveSlices,
                winningSliceIndex,
                multiplier,
                rewardsPanelTarget,
                itemIcon,
                cfg,
                onReelBack,
                onComplete,
                onIconArrived);
        }

        public void SnapSlicesToFullAlpha() => SlotZoomEffect.ResetSliceAlphas(_liveSlices);
    }
}