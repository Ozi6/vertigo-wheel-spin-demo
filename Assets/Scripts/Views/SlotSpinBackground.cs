using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
using WheelOfFortune.Data;

namespace WheelOfFortune.Views
{
    public sealed class SlotSpinBackground : MonoBehaviour
    {
        private WinEffectConfig _cfg;
        private CanvasGroup _cg;
        private Tweener _rotateTween;
        private Tweener _fadeTween;

        public static SlotSpinBackground Create(Transform parent, Vector3 worldPosition, WinEffectConfig cfg)
        {
            var go = new GameObject("ui_spin_bg_value", typeof(RectTransform));
            go.transform.SetParent(parent, false);
            go.transform.SetSiblingIndex(0);

            var comp = go.AddComponent<SlotSpinBackground>();
            comp.Init(worldPosition, cfg);
            return comp;
        }

        private void Init(Vector3 worldPosition, WinEffectConfig cfg)
        {
            _cfg = cfg;

            var rt = (RectTransform)transform;
            rt.position = worldPosition;
            rt.sizeDelta = new Vector2(cfg.BackgroundSize, cfg.BackgroundSize);
            rt.localScale = Vector3.one;

            var img = gameObject.AddComponent<Image>();
            img.sprite = cfg.BackgroundSprite;
            img.color = cfg.BackgroundSprite != null ? Color.white : cfg.BackgroundFallbackColor;
            img.preserveAspect = false;

            _cg = gameObject.AddComponent<CanvasGroup>();
            _cg.alpha = 0f;

            _fadeTween = DOTween.To(() => _cg.alpha, v => _cg.alpha = v, 1f, cfg.BackgroundFadeInDuration)
                   .SetEase(Ease.OutQuad);

            _rotateTween = rt.DORotate(new Vector3(0f, 0f, -360f), cfg.BackgroundSpinDuration, RotateMode.FastBeyond360)
              .SetEase(Ease.Linear)
              .SetLoops(-1, LoopType.Restart);
        }

        public void FadeOutAndDestroy()
        {
            _rotateTween?.Kill();
            _fadeTween?.Kill();

            _fadeTween = DOTween.To(() => _cg.alpha, v => _cg.alpha = v, 0f, _cfg.BackgroundFadeOutDuration)
                   .SetEase(Ease.OutQuad)
                   .OnComplete(() => { if (this != null) Destroy(gameObject); });
        }

        private void OnDestroy()
        {
            _rotateTween?.Kill();
            _fadeTween?.Kill();
        }
    }
}