using DG.Tweening;
using UnityEngine;

namespace WheelOfFortune.Data
{
    [CreateAssetMenu(fileName = "WinEffectConfig", menuName = "WheelOfFortune/Win Effect Config")]
    public sealed class WinEffectConfig : ScriptableObject
    {
        [Header("Zoom")]
        [SerializeField, Min(0.01f)] private float _zoomDuration = 0.75f;
        [SerializeField, Min(0.01f)] private float _zoomScalePeak = 4.5f;
        [SerializeField] private Ease _zoomEase = Ease.OutBack;
        [SerializeField] private Vector2 _zoomWorldOffset = Vector2.zero;

        [Header("Settle")]
        [SerializeField, Min(0.01f)] private float _settleScale = 3.9f;
        [SerializeField, Min(0.01f)] private float _settleDuration = 0.18f;

        [Header("Slice Fade")]
        [SerializeField, Min(0.01f)] private float _fadeDuration = 0.6f;
        [SerializeField] private Ease _fadeEase = Ease.OutQuad;
        [SerializeField][Range(0f, 1f)] private float _reelBackTriggerFraction = 0.25f;

        [Header("Spin Background")]
        [SerializeField] private Sprite _backgroundSprite;
        [SerializeField] private Color _backgroundFallbackColor = new Color(1f, 0.85f, 0.1f, 1f);
        [SerializeField, Min(1f)] private float _backgroundSize = 420f;
        [SerializeField, Min(0.01f)] private float _backgroundSpinDuration = 1.8f;
        [SerializeField, Min(0.01f)] private float _backgroundFadeInDuration = 0.2f;
        [SerializeField, Min(0.01f)] private float _backgroundFadeOutDuration = 0.15f;

        [Header("Icon Burst")]
        [SerializeField, Min(1f)] private float _iconSize = 64f;
        [SerializeField, Min(1f)] private float _burstRadius = 60f;
        [SerializeField, Min(0.01f)] private float _popDuration = 0.15f;
        [SerializeField, Min(0.01f)] private float _burstMoveDuration = 0.12f;
        [SerializeField, Min(0.01f)] private float _flyDuration = 0.55f;
        [SerializeField, Min(0.001f)] private float _flyStagger = 0.07f;
        [SerializeField, Min(0f)] private float _flyArcStrength = 180f; // Allowed 0 in case linear path is desired
        [SerializeField] private Ease _flyEase = Ease.InBack;

        public float ZoomDuration => _zoomDuration;
        public float ZoomScalePeak => _zoomScalePeak;
        public Ease ZoomEase => _zoomEase;
        public Vector2 ZoomWorldOffset => _zoomWorldOffset;

        public float SettleScale => _settleScale;
        public float SettleDuration => _settleDuration;

        public float FadeDuration => _fadeDuration;
        public Ease FadeEase => _fadeEase;
        public float ReelBackTriggerFraction => _reelBackTriggerFraction;

        public Sprite BackgroundSprite => _backgroundSprite;
        public Color BackgroundFallbackColor => _backgroundFallbackColor;
        public float BackgroundSize => _backgroundSize;
        public float BackgroundSpinDuration => _backgroundSpinDuration;
        public float BackgroundFadeInDuration => _backgroundFadeInDuration;
        public float BackgroundFadeOutDuration => _backgroundFadeOutDuration;

        public float IconSize => _iconSize;
        public float BurstRadius => _burstRadius;
        public float PopDuration => _popDuration;
        public float BurstMoveDuration => _burstMoveDuration;
        public float FlyDuration => _flyDuration;
        public float FlyStagger => _flyStagger;
        public float FlyArcStrength => _flyArcStrength;
        public Ease FlyEase => _flyEase;

        public float TotalBurstDuration(int count) =>
            _popDuration + _burstMoveDuration + _flyDuration + _flyStagger * (Mathf.Max(1, count) - 1) + 0.3f;

        private void OnValidate()
        {
            if (_zoomDuration < 0.01f) _zoomDuration = 0.01f;
            if (_zoomScalePeak < 0.01f) _zoomScalePeak = 0.01f;
            if (_settleScale < 0.01f) _settleScale = 0.01f;
            if (_settleDuration < 0.01f) _settleDuration = 0.01f;
            if (_fadeDuration < 0.01f) _fadeDuration = 0.01f;
            if (_backgroundSize < 1f) _backgroundSize = 1f;
            if (_backgroundSpinDuration < 0.01f) _backgroundSpinDuration = 0.01f;
            if (_backgroundFadeInDuration < 0.01f) _backgroundFadeInDuration = 0.01f;
            if (_backgroundFadeOutDuration < 0.01f) _backgroundFadeOutDuration = 0.01f;
            if (_iconSize < 1f) _iconSize = 1f;
            if (_burstRadius < 1f) _burstRadius = 1f;
            if (_popDuration < 0.01f) _popDuration = 0.01f;
            if (_burstMoveDuration < 0.01f) _burstMoveDuration = 0.01f;
            if (_flyDuration < 0.01f) _flyDuration = 0.01f;
            if (_flyStagger < 0.001f) _flyStagger = 0.001f;
            if (_flyArcStrength < 0f) _flyArcStrength = 0f;
        }
    }
}