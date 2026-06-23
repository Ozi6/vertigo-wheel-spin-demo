using UnityEngine;
using WheelOfFortune.Data;

namespace WheelOfFortune.Interfaces
{
    public interface ISlotFactory
    {
        SlotDefinition[] CreateSlots(Transform slotParent, int slotCount);
    }
}