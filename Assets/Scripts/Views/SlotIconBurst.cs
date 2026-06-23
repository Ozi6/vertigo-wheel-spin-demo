using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
using WheelOfFortune.Data;
using WheelOfFortune.Domain;
using WheelOfFortune.Events;
using WheelOfFortune.Utility;

namespace WheelOfFortune.Views
{
    public sealed class SlotIconBurst : MonoBehaviour
    {
        private Sequence _burstSequence;
        private WinEffectPayload _payload;
        private ComponentPool<Image> _pool;

        public static SlotIconBurst Play(
            Transform parent,
            Vector3 fromWorld,
            int count,
            WinEffectPayload payload,
            ComponentPool<Image> pool)
        {
            var go = new GameObject("ui_icon_burst_value");
            go.transform.SetParent(parent, false);

            var comp = go.AddComponent<SlotIconBurst>();
            comp.Begin(fromWorld, Mathf.Max(1, count), payload, pool);
            return comp;
        }

        private void Begin(Vector3 fromWorld, int count, WinEffectPayload payload, ComponentPool<Image> pool)
        {
            _payload = payload;
            _pool = pool;
            Vector3 targetWorld = fromWorld;
            if (payload.RewardsPanelTarget != null)
            {
                Transform t = payload.RewardsPanelTarget.parent != null ? payload.RewardsPanelTarget.parent : payload.RewardsPanelTarget;
                targetWorld = t is RectTransform rt
                    ? (Vector3)rt.rect.center + t.position
                    : t.position;
            }

            _burstSequence = DOTween.Sequence();

            for (int i = 0; i < count; i++)
                SpawnOne(i, count, fromWorld, targetWorld, payload);
        }

        private void SpawnOne(int index, int total, Vector3 from, Vector3 to, WinEffectPayload payload)
        {
            var img = _pool.Get(transform);
            var go = img.gameObject;
            go.name = $"ui_fly_icon_{index}_value";
            go.transform.SetAsLastSibling();

            var rt = go.GetComponent<RectTransform>();
            rt.position = from;
            rt.sizeDelta = new Vector2(payload.Config.IconSize, payload.Config.IconSize);
            rt.localScale = Vector3.zero;

            img.sprite = payload.ItemIcon;
            img.preserveAspect = true;

            float angle = (360f / total) * index;
            Vector3 burstOffset = new Vector3(
                Mathf.Cos(angle * Mathf.Deg2Rad),
                Mathf.Sin(angle * Mathf.Deg2Rad), 0f) * payload.Config.BurstRadius;

            Vector3 mid = Vector3.Lerp(from + burstOffset, to, 0.5f)
                          + new Vector3(
                              UnityEngine.Random.Range(-payload.Config.FlyArcStrength, payload.Config.FlyArcStrength),
                              payload.Config.FlyArcStrength * 0.6f, 0f);

            var capturedImg = img;
            int arrivalIndex = index + 1;

            var seq = DOTween.Sequence();
            seq.AppendInterval(index * payload.Config.FlyStagger);
            seq.Append(rt.DOScale(Vector3.one, payload.Config.PopDuration).SetEase(Ease.OutBack));
            seq.Append(rt.DOMove(from + burstOffset, payload.Config.BurstMoveDuration).SetEase(Ease.OutCubic));
            seq.Append(BezierFly(rt, from + burstOffset, mid, to, payload.Config));
            seq.AppendCallback(() =>
            {
                int mapped = arrivalIndex >= total
                    ? payload.Multiplier
                    : Mathf.RoundToInt((float)arrivalIndex / total * payload.Multiplier);

                if (!string.IsNullOrEmpty(payload.ItemId))
                {
                    int increment = mapped - Mathf.RoundToInt((float)(arrivalIndex - 1) / total * payload.Multiplier);
                    if (arrivalIndex == total) increment = payload.Multiplier - Mathf.RoundToInt((float)(total - 1) / total * payload.Multiplier);

                    payload.EventBus?.Publish(new OnRewardIconArrived(payload.ItemId, increment));
                }

                if (capturedImg != null) _pool.Release(capturedImg);
            });
            seq.OnComplete(() => { if (index == total - 1) Destroy(gameObject); });

            _burstSequence.Join(seq);
        }

        private static Tween BezierFly(Transform t, Vector3 p0, Vector3 p1, Vector3 p2, WinEffectConfig cfg)
        {
            return DOTween.To(
                () => 0f,
                progress =>
                {
                    if (t == null) return;
                    float u = 1f - progress;
                    t.position = u * u * p0 + 2f * u * progress * p1 + progress * progress * p2;
                },
                1f,
                cfg.FlyDuration)
            .SetEase(cfg.FlyEase);
        }

        private void OnDestroy()
        {
            if (_burstSequence != null && _burstSequence.IsActive())
                _burstSequence.Kill();
            _burstSequence = null;
        }
    }
}