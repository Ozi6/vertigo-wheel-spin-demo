using System;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace WheelOfFortune.Views
{
    public sealed class SlotZoomEffect : MonoBehaviour
    {
        private const float ZoomDuration = 0.75f;
        private const float FadeDuration = 0.6f;
        private const float ZoomScalePeak = 4.5f;
        private const float SettleScale = 3.9f;
        private const float SettleDuration = 0.18f;
        private const float BgSpinDuration = 1.8f;
        private const float BgFadeInDuration = 0.2f;
        private const float BgSize = 420f;
        private const float PopIconSize = 64f;
        private const float FlyDuration = 0.55f;
        private const float FlyStagger = 0.07f;
        private const float FlyArcStrength = 180f;
        private const Ease ZoomEase = Ease.OutBack;
        private const Ease FadeEase = Ease.OutQuad;
        private const Ease FlyEase = Ease.InBack;
        private const float ReelBackTriggerFraction = 0.25f;

        private GameObject _clone;
        private GameObject _spinBg;
        private List<CanvasGroup> _allFadeGroups = new List<CanvasGroup>();
        private Action _onComplete;

        public static SlotZoomEffect Play(
            Transform uiRoot,
            WheelSlice winningSlice,
            WheelSlice[] allSlices,
            int winningIndex,
            int multiplier,
            Transform rewardsPanelTarget,
            Sprite itemIcon,
            Action onReelBack,
            Action onComplete)
        {
            var go = new GameObject("SlotZoomEffect");
            go.transform.SetParent(uiRoot, false);

            var effect = go.AddComponent<SlotZoomEffect>();
            effect.Begin(
                winningSlice, allSlices, winningIndex,
                multiplier, rewardsPanelTarget, itemIcon,
                onReelBack, onComplete);
            return effect;
        }

        private void Begin(
            WheelSlice winningSlice,
            WheelSlice[] allSlices,
            int winningIndex,
            int multiplier,
            Transform rewardsPanelTarget,
            Sprite itemIcon,
            Action onReelBack,
            Action onComplete)
        {
            _onComplete = onComplete;

            for (int i = 0; i < allSlices.Length; i++)
            {
                if (allSlices[i] == null) continue;
                _allFadeGroups.Add(GetOrAddCanvasGroup(allSlices[i].gameObject));
            }

            var srcRect = winningSlice.GetComponent<RectTransform>();
            Vector3 worldCenter = srcRect != null ? srcRect.position : winningSlice.transform.position;

            _spinBg = BuildSpinBackground(worldCenter);

            _clone = Instantiate(winningSlice.gameObject, transform);
            _clone.name = "ui_zoom_clone_value";

            var cloneRect = _clone.GetComponent<RectTransform>();
            if (cloneRect != null && srcRect != null)
            {
                cloneRect.position = srcRect.position;
                cloneRect.sizeDelta = srcRect.sizeDelta;
                cloneRect.localScale = Vector3.one;
            }

            _clone.transform.SetAsLastSibling();
            GetOrAddCanvasGroup(_clone).alpha = 1f;

            float reelBackAt = ZoomDuration * ReelBackTriggerFraction;

            var seq = DOTween.Sequence();

            seq.Append(
                _clone.transform
                    .DOScale(Vector3.one * ZoomScalePeak, ZoomDuration)
                    .SetEase(ZoomEase));

            seq.Append(
                _clone.transform
                    .DOScale(Vector3.one * SettleScale, SettleDuration)
                    .SetEase(Ease.OutQuad));

            seq.InsertCallback(reelBackAt, () => onReelBack?.Invoke());
            seq.Insert(reelBackAt, BuildFade());

            seq.AppendCallback(() =>
            {
                KillSpinBg();
                BurstAndFly(worldCenter, multiplier, rewardsPanelTarget, itemIcon);
            });

            seq.AppendInterval(FlyDuration + FlyStagger * (multiplier - 1) + 0.05f);
            seq.OnComplete(OnSequenceComplete);
        }

        private GameObject BuildSpinBackground(Vector3 worldCenter)
        {
            var bgGo = new GameObject("ui_spin_bg_value");
            bgGo.transform.SetParent(transform, false);
            bgGo.transform.SetSiblingIndex(0);

            var bgRect = bgGo.AddComponent<RectTransform>();
            bgRect.position = worldCenter;
            bgRect.sizeDelta = new Vector2(BgSize, BgSize);
            bgRect.localScale = Vector3.one;

            var img = bgGo.AddComponent<Image>();
            img.color = new Color(1f, 0.85f, 0.1f, 0f);

            var cg = bgGo.AddComponent<CanvasGroup>();
            cg.alpha = 0f;

            DOTween.To(() => cg.alpha, v => cg.alpha = v, 1f, BgFadeInDuration)
                   .SetEase(Ease.OutQuad);

            bgRect.DORotate(new Vector3(0f, 0f, -360f), BgSpinDuration, RotateMode.FastBeyond360)
                  .SetEase(Ease.Linear)
                  .SetLoops(-1, LoopType.Restart);

            return bgGo;
        }

        private void KillSpinBg()
        {
            if (_spinBg == null) return;
            DOTween.Kill(_spinBg.transform);
            var cg = _spinBg.GetComponent<CanvasGroup>();
            if (cg != null)
                DOTween.To(() => cg.alpha, v => cg.alpha = v, 0f, 0.15f)
                       .OnComplete(() => { if (_spinBg != null) Destroy(_spinBg); });
            else
                Destroy(_spinBg);
            _spinBg = null;
        }

        private void BurstAndFly(
            Vector3 fromWorld,
            int multiplier,
            Transform target,
            Sprite icon)
        {
            if (_clone != null)
            {
                Destroy(_clone);
                _clone = null;
            }

            int count = Mathf.Max(1, multiplier);
            Vector3 targetWorld = target != null ? target.position : fromWorld;

            for (int i = 0; i < count; i++)
            {
                var iconGo = new GameObject($"ui_fly_icon_{i}_value");
                iconGo.transform.SetParent(transform, false);

                var rt = iconGo.AddComponent<RectTransform>();
                rt.position = fromWorld;
                rt.sizeDelta = new Vector2(PopIconSize, PopIconSize);
                rt.localScale = Vector3.zero;

                var img = iconGo.AddComponent<Image>();
                img.sprite = icon;
                img.preserveAspect = true;

                iconGo.transform.SetAsLastSibling();

                float delay = i * FlyStagger;

                float angle = (360f / count) * i;
                float rad = angle * Mathf.Deg2Rad;
                Vector3 burstOffset = new Vector3(Mathf.Cos(rad), Mathf.Sin(rad), 0f) * 60f;

                var flySeq = DOTween.Sequence();
                flySeq.AppendInterval(delay);
                flySeq.Append(rt.DOScale(Vector3.one, 0.15f).SetEase(Ease.OutBack));
                flySeq.Append(rt.DOMove(fromWorld + burstOffset, 0.12f).SetEase(Ease.OutCubic));

                Vector3 midPoint = Vector3.Lerp(fromWorld + burstOffset, targetWorld, 0.5f)
                                   + new Vector3(
                                       UnityEngine.Random.Range(-FlyArcStrength, FlyArcStrength),
                                       FlyArcStrength * 0.6f, 0f);

                var capturedGo = iconGo;
                flySeq.Append(
                    DOTween.To(
                        () => 0f,
                        t =>
                        {
                            if (capturedGo == null) return;
                            Vector3 p0 = fromWorld + burstOffset;
                            Vector3 p1 = midPoint;
                            Vector3 p2 = targetWorld;
                            float u = 1f - t;
                            capturedGo.transform.position =
                                u * u * p0 + 2f * u * t * p1 + t * t * p2;
                        },
                        1f,
                        FlyDuration)
                    .SetEase(FlyEase));

                flySeq.AppendCallback(() =>
                {
                    if (capturedGo != null) Destroy(capturedGo);
                });
            }
        }

        private Tween BuildFade()
        {
            var fadeSeq = DOTween.Sequence();
            foreach (var cg in _allFadeGroups)
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
            if (_clone != null) Destroy(_clone);
            KillSpinBg();

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