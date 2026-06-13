using System;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace WheelOfFortune.Views
{

    public sealed class SlotZoomEffect : MonoBehaviour
    {

        private const float ZoomDuration = 1.1f;
        private const float FadeDuration = 0.6f;
        private const float FadeDelay = 0.15f;
        private const float ZoomScalePeak = 4.5f;
        private const Ease ZoomEase = Ease.InCubic;
        private const Ease FadeEase = Ease.OutQuad;

        private GameObject _clone;
        private List<CanvasGroup> _siblingGroups = new List<CanvasGroup>();
        private Action _onComplete;

        public static SlotZoomEffect Play(
            Transform uiRoot,
            WheelSlice winningSlice,
            WheelSlice[] allSlices,
            int winningIndex,
            Action onComplete)
        {
            var go = new GameObject("SlotZoomEffect");
            go.transform.SetParent(uiRoot, false);

            var effect = go.AddComponent<SlotZoomEffect>();
            effect.Begin(winningSlice, allSlices, winningIndex, onComplete);
            return effect;
        }

        private void Begin(
            WheelSlice winningSlice,
            WheelSlice[] allSlices,
            int winningIndex,
            Action onComplete)
        {
            _onComplete = onComplete;

            for (int i = 0; i < allSlices.Length; i++)
            {
                if (i == winningIndex || allSlices[i] == null) continue;
                var cg = GetOrAddCanvasGroup(allSlices[i].gameObject);
                _siblingGroups.Add(cg);
            }

            _clone = Instantiate(winningSlice.gameObject, transform);
            _clone.name = "ui_zoom_clone_value";

            var cloneRect = _clone.GetComponent<RectTransform>();
            var srcRect = winningSlice.GetComponent<RectTransform>();
            if (cloneRect != null && srcRect != null)
            {
                cloneRect.position = srcRect.position;
                cloneRect.sizeDelta = srcRect.sizeDelta;
                cloneRect.localScale = Vector3.one;
            }

            _clone.transform.SetAsLastSibling();

            var cloneCG = GetOrAddCanvasGroup(_clone);
            cloneCG.alpha = 1f;

            var seq = DOTween.Sequence();

            seq.Append(
                _clone.transform
                    .DOScale(Vector3.one * ZoomScalePeak, ZoomDuration)
                    .SetEase(ZoomEase));

            seq.Insert(FadeDelay, BuildSiblingFade());

            seq.OnComplete(OnSequenceComplete);
        }

        private Tween BuildSiblingFade()
        {
            var fadeSeq = DOTween.Sequence();
            foreach (var cg in _siblingGroups)
            {
                var target = cg;
                fadeSeq.Join(
                    DOVirtual.Float(1f, 0f, FadeDuration, v => target.alpha = v)
                             .SetEase(FadeEase));
            }
            return fadeSeq;
        }

        private void OnSequenceComplete()
        {

            if (_clone != null)
                Destroy(_clone);

            _onComplete?.Invoke();

            Destroy(gameObject);
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
    }
}