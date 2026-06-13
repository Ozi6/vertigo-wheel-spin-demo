using UnityEngine;
using WheelOfFortune.Data;

namespace WheelOfFortune.Factory
{
    public sealed class SlotFactory : ISlotFactory
    {
        public SlotDefinition[] CreateSlots(Transform slotParent, int slotCount)
        {
            var slots = new SlotDefinition[slotCount];

            for (int i = 0; i < slotCount; i++)
            {
                var slotTransform = slotParent.GetChild(i);
                slots[i] = new SlotDefinition(i, slotTransform);
            }

            return slots;
        }
    }
}