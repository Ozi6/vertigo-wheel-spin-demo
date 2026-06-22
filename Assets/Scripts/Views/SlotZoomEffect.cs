using System;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using WheelOfFortune.Data;

namespace WheelOfFortune.Views
{
    public sealed class SlotZoomEffect : MonoBehaviour
    {
        private const int MaxFlyingMultipliers = 50;

        private GameObject _clone;
        private SlotSpinBackground _spinBg;
        private List<CanvasGroup> _allFadeGroups = new List<CanvasGroup>();
        private Action _onComplete;
        private Action _onBurstFinished;
        private WinEffectConfig _cfg;
        private Sequence _zoomSequence;

        public static SlotZoomEffect Play(
            Transform uiRoot,
            WheelSlice winningSlice,
            WheelSlice[] allSlices,
            int winningIndex,
            int multiplier,
            Transform rewardsPanelTarget,
            Sprite itemIcon,
            WinEffectConfig cfg,
            Action onReelBack,
            Action onComplete,
            Action<int> onIconArrived = null,
            Action onBurstFinished = null)
        {
            var go = new GameObject("SlotZoomEffect");
            go.transform.SetParent(uiRoot, false);

            var effect = go.AddComponent<SlotZoomEffect>();
            effect.Begin(
                winningSlice, allSlices, winningIndex,
                multiplier, rewardsPanelTarget, itemIcon, cfg,
                onReelBack, onComplete, onIconArrived, onBurstFinished);
            return effect;
        }

        private void Begin(
            WheelSlice winningSlice,
            WheelSlice[] allSlices,
            int winningIndex,
            int multiplier,
            Transform rewardsPanelTarget,
            Sprite itemIcon,
            WinEffectConfig cfg,
            Action onReelBack,
            Action onComplete,
            Action<int> onIconArrived = null,
            Action onBurstFinished = null)
        {
            _cfg = cfg;
            _onComplete = onComplete;
            _onBurstFinished = onBurstFinished;

            foreach (var slice in allSlices)
            {
                if (slice == null) continue;
                _allFadeGroups.Add(GetOrAddCanvasGroup(slice.gameObject));
            }

            var srcRect = winningSlice.GetComponent<RectTransform>();
            var worldCenter = srcRect != null ? srcRect.position : winningSlice.transform.position;
            worldCenter += (Vector3)cfg.ZoomWorldOffset;

            _spinBg = SlotSpinBackground.Create(transform, worldCenter, cfg);
            _clone = BuildClone(winningSlice, srcRect, worldCenter);

            float reelBackAt = cfg.ZoomDuration * cfg.ReelBackTriggerFraction;
            int cappedMultiplier = Mathf.Min(multiplier, MaxFlyingMultipliers);

            Action<int> remappedIconArrived = null;
            if (onIconArrived != null)
            {
                int realMultiplier = multiplier;
                int capped = cappedMultiplier;
                remappedIconArrived = arrived =>
                {
                    int mapped = arrived >= capped
                        ? realMultiplier
                        : Mathf.RoundToInt((float)arrived / capped * realMultiplier);
                    onIconArrived(mapped);
                };
            }

            _zoomSequence = DOTween.Sequence()
                .Append(ZoomToPeak())
                .Append(SettleDown())
                .InsertCallback(reelBackAt, () => onReelBack?.Invoke())
                .Insert(reelBackAt, FadeAllSlices())
                .AppendCallback(() =>
                {
                    if (_spinBg != null)
                    {
                        _spinBg.FadeOutAndDestroy();
                        _spinBg = null;
                    }
                    DestroyClone();
                    SlotIconBurst.Play(transform, worldCenter, cappedMultiplier, itemIcon, rewardsPanelTarget, cfg, remappedIconArrived);
                })
                .AppendInterval(cfg.TotalBurstDuration(cappedMultiplier))
                .OnComplete(OnSequenceComplete);
        }

        private GameObject BuildClone(WheelSlice source, RectTransform srcRect, Vector3 worldCenter)
        {
            var clone = Instantiate(source.gameObject, transform);
            clone.name = "ui_zoom_clone_value";
            clone.transform.SetAsLastSibling();

            var cloneRect = clone.GetComponent<RectTransform>();
            if (cloneRect != null)
            {
                cloneRect.position = worldCenter;
                cloneRect.sizeDelta = srcRect != null ? srcRect.sizeDelta : cloneRect.sizeDelta;
                cloneRect.localScale = Vector3.one;
            }

            GetOrAddCanvasGroup(clone).alpha = 1f;
            return clone;
        }

        private Tween ZoomToPeak() =>
            _clone.transform
                .DOScale(Vector3.one * _cfg.ZoomScalePeak, _cfg.ZoomDuration)
                .SetEase(_cfg.ZoomEase);

        private Tween SettleDown() =>
            _clone.transform
                .DOScale(Vector3.one * _cfg.SettleScale, _cfg.SettleDuration)
                .SetEase(Ease.OutQuad);

        private Tween FadeAllSlices()
        {
            var seq = DOTween.Sequence();
            foreach (var cg in _allFadeGroups)
            {
                var target = cg;
                seq.Join(
                    DOVirtual.Float(1f, 0f, _cfg.FadeDuration, v => target.alpha = v)
                             .SetEase(_cfg.FadeEase));
            }
            return seq;
        }

        private void DestroyClone()
        {
            if (_clone == null) return;
            Destroy(_clone);
            _clone = null;
        }

        private void OnSequenceComplete()
        {
            DestroyClone();
            _onComplete?.Invoke();
            Destroy(gameObject, 0.2f);
        }

        public static void ResetSliceAlphas(WheelSlice[] slices)
        {
            if (slices == null) return;
            foreach (var slice in slices)
            {
                if (slice == null) continue;
                var cg = slice.GetComponent<CanvasGroup>();
                if (cg != null) cg.alpha = 1f;
            }
        }

        private static CanvasGroup GetOrAddCanvasGroup(GameObject go)
        {
            var cg = go.GetComponent<CanvasGroup>();
            if (cg == null) cg = go.AddComponent<CanvasGroup>();
            return cg;
        }

        private void OnDestroy()
        {
            if (_zoomSequence != null)
            {
                _zoomSequence.Kill();
                _zoomSequence = null;
            }
            _onBurstFinished?.Invoke();
        }
    }
}