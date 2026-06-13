using UnityEngine;
using WheelOfFortune.Data;

namespace WheelOfFortune.Factory
{
    public interface ISlotFactory
    {
        SlotDefinition[] CreateSlots(Transform slotParent, int slotCount);
    }
}