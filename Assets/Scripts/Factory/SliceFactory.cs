using UnityEngine;
using WheelOfFortune.Data;
using WheelOfFortune.Utility;
using WheelOfFortune.Views;

using WheelOfFortune.Interfaces;

namespace WheelOfFortune.Factory
{
    public sealed class SliceFactory : ISliceFactory
    {
        private readonly ComponentPool<WheelSlice> _pool;
        private readonly Sprite _bombIcon;

        public SliceFactory(WheelSlice slicePrefab, Sprite bombIcon = null)
        {
            _pool = new ComponentPool<WheelSlice>(slicePrefab, "Pool_WheelSlice", 8, null, OnReleaseSlice);
            _bombIcon = bombIcon;
        }

        public WheelSlice[] CreateSlices(SliceDefinition[] slices, SlotDefinition[] slots)
        {
            var instances = new WheelSlice[slices.Length];

            for (int i = 0; i < slices.Length; i++)
            {
                var slotTransform = slots[i].Position;
                var instance = _pool.Get(slotTransform);
                instance.name = $"ui_slice_{i:D2}_value";

                var reward = slices[i].RewardItem;

                if (reward == null && _bombIcon != null)
                    instance.Setup(_bombIcon, "LOSE");
                else if (reward != null)
                {
                    var icon = reward.Icon;
                    var label = MultiplierFormatter.Format(slices[i].Multiplier);
                    instance.Setup(icon, label);
                }
                else
                    instance.Setup(null, "LOSE");

                instances[i] = instance;
            }

            return instances;
        }

        public void ReturnSlices(WheelSlice[] slices)
        {
            if (slices == null) return;
            foreach (var slice in slices)
                if (slice != null)
                    _pool.Release(slice);
        }

        public void Clear()
        {
            _pool.Clear();
        }

        private void OnReleaseSlice(WheelSlice slice)
        {
            if (slice == null) return;
            var cg = slice.GetComponent<CanvasGroup>();
            if (cg != null) cg.alpha = 1f;
        }
    }
}