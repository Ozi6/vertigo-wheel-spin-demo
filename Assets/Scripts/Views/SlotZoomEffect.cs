using System;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using WheelOfFortune.Utility;
using WheelOfFortune.Domain;
using WheelOfFortune.Events;
using WheelOfFortune.Interfaces;

namespace WheelOfFortune.Views
{
    public sealed class SlotZoomEffect : MonoBehaviour
    {
        #region Constants
        private const int MaxFlyingMultipliers = 50;
        #endregion

        #region Private Fields
        private GameObject _clone;
        private SlotSpinBackground _spinBg;
        private List<CanvasGroup> _allFadeGroups = new List<CanvasGroup>();
        private WinEffectPayload _payload;
        private Sequence _zoomSequence;
        private ComponentPool<UnityEngine.UI.Image> _iconPool;
        #endregion

        #region Public Interface
        public static SlotZoomEffect Play(
            Transform uiRoot,
            WheelSlice winningSlice,
            WheelSlice[] allSlices,
            WinEffectPayload payload,
            ComponentPool<UnityEngine.UI.Image> iconPool)
        {
            var go = new GameObject("SlotZoomEffect");
            go.transform.SetParent(uiRoot, false);

            var effect = go.AddComponent<SlotZoomEffect>();
            effect.Begin(winningSlice, allSlices, payload, iconPool);
            return effect;
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
        #endregion

        #region Execution
        private void Begin(
            WheelSlice winningSlice,
            WheelSlice[] allSlices,
            WinEffectPayload payload,
            ComponentPool<UnityEngine.UI.Image> iconPool)
        {
            _payload = payload;
            _iconPool = iconPool;

            foreach (var slice in allSlices)
            {
                if (slice == null) continue;
                _allFadeGroups.Add(GetOrAddCanvasGroup(slice.gameObject));
            }

            var srcRect = winningSlice.GetComponent<RectTransform>();
            var worldCenter = srcRect != null ? srcRect.position : winningSlice.transform.position;
            worldCenter += (Vector3)_payload.Config.ZoomWorldOffset;

            _spinBg = SlotSpinBackground.Create(transform, worldCenter, _payload.Config);
            _clone = BuildClone(winningSlice, srcRect, worldCenter);

            float reelBackAt = _payload.Config.ZoomDuration * _payload.Config.ReelBackTriggerFraction;
            int cappedMultiplier = Mathf.Min(_payload.Multiplier, MaxFlyingMultipliers);

            _zoomSequence = DOTween.Sequence()
                .Append(ZoomToPeak())
                .Append(SettleDown())
                .InsertCallback(reelBackAt, () => _payload.EventBus?.Publish(new OnWinEffectReelBack()))
                .Insert(reelBackAt, FadeAllSlices())
                .AppendCallback(() =>
                {
                    if (_spinBg != null)
                    {
                        _spinBg.FadeOutAndDestroy();
                        _spinBg = null;
                    }
                    DestroyClone();
                    SlotIconBurst.Play(transform, worldCenter, cappedMultiplier, _payload, _iconPool);
                })
                .AppendInterval(_payload.Config.TotalBurstDuration(cappedMultiplier))
                .OnComplete(OnSequenceComplete);
        }
        #endregion

        #region Helpers
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
                .DOScale(Vector3.one * _payload.Config.ZoomScalePeak, _payload.Config.ZoomDuration)
                .SetEase(_payload.Config.ZoomEase);

        private Tween SettleDown() =>
            _clone.transform
                .DOScale(Vector3.one * _payload.Config.SettleScale, _payload.Config.SettleDuration)
                .SetEase(Ease.OutQuad);

        private Tween FadeAllSlices()
        {
            var seq = DOTween.Sequence();
            foreach (var cg in _allFadeGroups)
            {
                var target = cg;
                seq.Join(
                    DOVirtual.Float(1f, 0f, _payload.Config.FadeDuration, v => {
                        if (target != null) target.alpha = v;
                    })
                    .SetEase(_payload.Config.FadeEase));
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
            _payload.EventBus?.Publish(new OnWinEffectCompleted());
            Destroy(gameObject, 0.2f);
        }

        private static CanvasGroup GetOrAddCanvasGroup(GameObject go)
        {
            var cg = go.GetComponent<CanvasGroup>();
            if (cg == null) cg = go.AddComponent<CanvasGroup>();
            return cg;
        }
        #endregion

        #region Unity Lifecycle
        private void OnDestroy()
        {
            if (_zoomSequence != null && _zoomSequence.IsActive())
            {
                _zoomSequence.Kill();
            }
            _zoomSequence = null;
            if (!string.IsNullOrEmpty(_payload.ItemId))
                _payload.EventBus?.Publish(new OnRewardBurstFinished(_payload.ItemId, _payload.RewardMultiplier));
        }
        #endregion
    }
}
