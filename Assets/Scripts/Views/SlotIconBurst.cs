using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
using WheelOfFortune.Data;

namespace WheelOfFortune.Views
{
    public sealed class SlotIconBurst : MonoBehaviour
    {
        public static SlotIconBurst Play(
            Transform parent,
            Vector3 fromWorld,
            int count,
            Sprite icon,
            Transform panelTarget,
            WinEffectConfig cfg,
            Action<int> onIconArrived = null)
        {
            var go = new GameObject("ui_icon_burst_value");
            go.transform.SetParent(parent, false);

            var comp = go.AddComponent<SlotIconBurst>();
            comp.Begin(fromWorld, Mathf.Max(1, count), icon, panelTarget, cfg, onIconArrived);
            return comp;
        }

        private void Begin(Vector3 fromWorld, int count, Sprite icon, Transform panelTarget, WinEffectConfig cfg, Action<int> onIconArrived)
        {
            Vector3 targetWorld = fromWorld;
            if (panelTarget != null)
            {
                Transform t = panelTarget.parent != null ? panelTarget.parent : panelTarget;
                targetWorld = t is RectTransform rt
                    ? (Vector3)rt.rect.center + t.position
                    : t.position;
            }

            for (int i = 0; i < count; i++)
                SpawnOne(i, count, fromWorld, targetWorld, icon, cfg, onIconArrived);
        }

        private void SpawnOne(int index, int total, Vector3 from, Vector3 to, Sprite icon, WinEffectConfig cfg, Action<int> onIconArrived)
        {
            var go = new GameObject($"ui_fly_icon_{index}_value");
            go.transform.SetParent(transform, false);
            go.transform.SetAsLastSibling();

            var rt = go.AddComponent<RectTransform>();
            rt.position = from;
            rt.sizeDelta = new Vector2(cfg.IconSize, cfg.IconSize);
            rt.localScale = Vector3.zero;

            var img = go.AddComponent<Image>();
            img.sprite = icon;
            img.preserveAspect = true;

            float angle = (360f / total) * index;
            Vector3 burstOffset = new Vector3(
                Mathf.Cos(angle * Mathf.Deg2Rad),
                Mathf.Sin(angle * Mathf.Deg2Rad), 0f) * cfg.BurstRadius;

            Vector3 mid = Vector3.Lerp(from + burstOffset, to, 0.5f)
                          + new Vector3(
                              UnityEngine.Random.Range(-cfg.FlyArcStrength, cfg.FlyArcStrength),
                              cfg.FlyArcStrength * 0.6f, 0f);

            var captured = go;
            int arrivalIndex = index + 1;

            var seq = DOTween.Sequence();
            seq.AppendInterval(index * cfg.FlyStagger);
            seq.Append(rt.DOScale(Vector3.one, cfg.PopDuration).SetEase(Ease.OutBack));
            seq.Append(rt.DOMove(from + burstOffset, cfg.BurstMoveDuration).SetEase(Ease.OutCubic));
            seq.Append(BezierFly(captured.transform, from + burstOffset, mid, to, cfg));
            seq.AppendCallback(() =>
            {
                onIconArrived?.Invoke(arrivalIndex);
                if (captured != null) Destroy(captured);
            });
            seq.OnComplete(() => { if (index == total - 1) Destroy(gameObject); });
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
    }
}