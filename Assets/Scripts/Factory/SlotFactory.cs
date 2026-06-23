using UnityEngine;
using WheelOfFortune.Data;
using WheelOfFortune.Interfaces;

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
                Vector3 directionFromCenter = slotTransform.position - slotParent.position;
                if (directionFromCenter != Vector3.zero)
                    slotTransform.rotation = Quaternion.LookRotation(slotParent.forward, directionFromCenter);
                slots[i] = new SlotDefinition(i, slotTransform);
            }

            return slots;
        }
    }
}