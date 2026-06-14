using UnityEngine;
using WheelOfFortune.Data;
using WheelOfFortune.Views;

namespace WheelOfFortune.Factory
{
    public sealed class SliceFactory
    {
        private readonly WheelSlice _slicePrefab;

        public SliceFactory(WheelSlice slicePrefab)
        {
            _slicePrefab = slicePrefab;
        }

        public WheelSlice[] CreateSlices(SliceDefinition[] slices, SlotDefinition[] slots)
        {
            var instances = new WheelSlice[slices.Length];

            for (int i = 0; i < slices.Length; i++)
            {
                var slotTransform = slots[i].Position;
                var instance = Object.Instantiate(_slicePrefab, slotTransform);
                instance.name = $"ui_slice_{i:D2}_value";

                var reward = slices[i].RewardItem;
                var icon = reward != null ? reward.Icon : null;
                var label = reward != null ? $"x{slices[i].Multiplier}" : string.Empty;

                instance.Setup(icon, label);
                instances[i] = instance;
            }

            return instances;
        }
    }
}