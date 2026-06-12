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

        public WheelSlice[] CreateSlices(SliceDefinition[] slices, Transform parent)
        {
            var instances = new WheelSlice[slices.Length];

            for (int i = 0; i < slices.Length; i++)
            {
                var instance = Object.Instantiate(_slicePrefab, parent);
                instance.name = $"ui_slice_{i:D2}_value";

                var reward = slices[i].RewardItem;
                var icon = reward != null ? reward.Icon : null;
                var label = reward != null ? reward.Value.ToString("F0") : string.Empty;

                instance.Setup(icon, label);
                instances[i] = instance;
            }

            return instances;
        }
    }
}
