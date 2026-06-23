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
        [SerializeField, Min(0.01f)] private float _spinDuration = 3f;
        [SerializeField, Min(1)] private int _extraSpins = 5;
        [SerializeField] private Ease _spinEase = Ease.OutQuart;
        [SerializeField] private HudPresenter _hudPresenter;

        private SliceDefinition[] _currentSlices;
        private WheelSlice[] _liveSlices;
        private IEventBus _eventBus;
        private Utility.ComponentPool<Image> _burstIconPool;

        private void Awake()
        {
            var go = new GameObject("BurstIcon_Prefab");
            go.AddComponent<RectTransform>();
            var prefab = go.AddComponent<Image>();
            go.SetActive(false);
            go.transform.SetParent(transform, false);
            _burstIconPool = new Utility.ComponentPool<Image>(prefab, "Pool_BurstIcon", 50);
        }

        private void OnDestroy()
        {
            _wheelRoot?.DOKill();
            _burstIconPool?.Clear();
        }

        public void Initialize(IEventBus eventBus) => _eventBus = eventBus;
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
                .SetEase(Ease.InOutSine)
                .SetLink(_wheelRoot.gameObject);
        }

        public void SpinTo(int targetSliceIndex)
        {
            if (_currentSlices == null || _currentSlices.Length == 0)
            {
                _eventBus?.Publish(new WheelOfFortune.Events.OnSpinAnimationComplete());
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
                .SetLink(_wheelRoot.gameObject)
                .OnComplete(() => _eventBus?.Publish(new WheelOfFortune.Events.OnSpinAnimationComplete()));
        }

        public void PlayWinEffect(Domain.WinEffectPayload payload)
        {
            if (_liveSlices == null || _liveSlices.Length == 0 ||
                payload.WinningSliceIndex < 0 || payload.WinningSliceIndex >= _liveSlices.Length)
            {
                payload.EventBus?.Publish(new WheelOfFortune.Events.OnWinEffectReelBack());
                payload.EventBus?.Publish(new WheelOfFortune.Events.OnWinEffectCompleted());
                return;
            }

            var winningSlice = _liveSlices[payload.WinningSliceIndex];
            if (winningSlice == null)
            {
                payload.EventBus?.Publish(new WheelOfFortune.Events.OnWinEffectReelBack());
                payload.EventBus?.Publish(new WheelOfFortune.Events.OnWinEffectCompleted());
                return;
            }

            var effectRoot = _wheelRoot.parent != null ? _wheelRoot.parent : _wheelRoot;

            SlotZoomEffect.Play(
                effectRoot,
                winningSlice,
                _liveSlices,
                payload,
                _burstIconPool);
        }

        public void SnapSlicesToFullAlpha() => SlotZoomEffect.ResetSliceAlphas(_liveSlices);

        private void OnValidate()
        {
            if (_spinDuration < 0.01f) _spinDuration = 0.01f;
            if (_extraSpins < 1) _extraSpins = 1;

            var transforms = GetComponentsInChildren<Transform>(true);
            foreach (var t in transforms)
            {
                string nameLower = t.name.ToLower();
                if (nameLower.Contains("bg") && nameLower.Contains("wheel"))
                    _wheelRoot = t;
            }

            var images = GetComponentsInChildren<Image>(true);
            foreach (var img in images)
            {
                string nameLower = img.name.ToLower();
                if (nameLower.Contains("wheel") && nameLower.Contains("bg") && !nameLower.Contains("root"))
                    _wheelImage_value = img;
                else if (nameLower.Contains("arrow") || nameLower.Contains("pointer"))
                    _arrowImage_value = img;
            }
        }
    }
}