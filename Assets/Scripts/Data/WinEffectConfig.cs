using DG.Tweening;
using UnityEngine;

namespace WheelOfFortune.Data
{
    [CreateAssetMenu(fileName = "WinEffectConfig", menuName = "WheelOfFortune/Win Effect Config")]
    public sealed class WinEffectConfig : ScriptableObject
    {
        [Header("Zoom")]
        [SerializeField] private float _zoomDuration = 0.75f;
        [SerializeField] private float _zoomScalePeak = 4.5f;
        [SerializeField] private Ease _zoomEase = Ease.OutBack;
        [SerializeField] private Vector2 _zoomWorldOffset = Vector2.zero;

        [Header("Settle")]
        [SerializeField] private float _settleScale = 3.9f;
        [SerializeField] private float _settleDuration = 0.18f;

        [Header("Slice Fade")]
        [SerializeField] private float _fadeDuration = 0.6f;
        [SerializeField] private Ease _fadeEase = Ease.OutQuad;
        [SerializeField]
        [Range(0f, 1f)]
        private float _reelBackTriggerFraction = 0.25f;

        [Header("Spin Background")]
        [SerializeField] private Sprite _backgroundSprite;
        [SerializeField] private Color _backgroundFallbackColor = new Color(1f, 0.85f, 0.1f, 1f);
        [SerializeField] private float _backgroundSize = 420f;
        [SerializeField] private float _backgroundSpinDuration = 1.8f;
        [SerializeField] private float _backgroundFadeInDuration = 0.2f;
        [SerializeField] private float _backgroundFadeOutDuration = 0.15f;

        [Header("Icon Burst")]
        [SerializeField] private float _iconSize = 64f;
        [SerializeField] private float _burstRadius = 60f;
        [SerializeField] private float _popDuration = 0.15f;
        [SerializeField] private float _burstMoveDuration = 0.12f;
        [SerializeField] private float _flyDuration = 0.55f;
        [SerializeField] private float _flyStagger = 0.07f;
        [SerializeField] private float _flyArcStrength = 180f;
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
            _flyDuration + _flyStagger * (Mathf.Max(1, count) - 1) + 0.05f;
    }
}