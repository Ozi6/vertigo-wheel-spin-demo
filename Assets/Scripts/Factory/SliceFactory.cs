using UnityEngine;
using WheelOfFortune.Data;
using WheelOfFortune.Views;

namespace WheelOfFortune.Factory
{
    public sealed class SliceFactory
    {
        private readonly WheelSlice _slicePrefab;
        private readonly Sprite _bombIcon;

        public SliceFactory(WheelSlice slicePrefab, Sprite bombIcon = null)
        {
            _slicePrefab = slicePrefab;
            _bombIcon = bombIcon;
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

                if (reward == null && _bombIcon != null)
                    instance.Setup(_bombIcon, "LOSE");
                else if (reward != null)
                {
                    var icon = reward.Icon;
                    var label = $"x{slices[i].Multiplier}";
                    instance.Setup(icon, label);
                }
                else
                    instance.Setup(null, "LOSE");

                instances[i] = instance;
            }

            return instances;
        }
    }
}