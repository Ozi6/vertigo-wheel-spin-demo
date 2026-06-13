using UnityEngine;
using WheelOfFortune.Data;
using WheelOfFortune.Domain;
using WheelOfFortune.Interfaces;

namespace WheelOfFortune.Factory
{
    public sealed class WheelFactory : IWheelFactory
    {
        private readonly ZoneConfigSO[] _zoneConfigs;
        private readonly SliceFactory _sliceFactory;
        private readonly SlotDefinition[] _slots;

        public WheelFactory(
            ZoneConfigSO[] zoneConfigs,
            SliceFactory sliceFactory,
            ISlotFactory slotFactory,
            Transform slotParent,
            int slotCount)
        {
            _zoneConfigs = zoneConfigs;
            _sliceFactory = sliceFactory;
            _slots = slotFactory.CreateSlots(slotParent, slotCount);
        }

        public RuntimeWheelData BuildWheel(ZoneType zoneType, int zoneNumber, IWheelView wheelView)
        {
            var zoneConfig = GetZoneConfig(zoneType);
            if (zoneConfig == null)
            {
                Debug.LogError($"[WheelFactory] No ZoneConfigSO found for ZoneType {zoneType}");
                return default;
            }

            var config = zoneConfig.WheelConfig;
            if (config == null)
            {
                Debug.LogError($"[WheelFactory] ZoneConfigSO for {zoneType} has no WheelConfig assigned.");
                return default;
            }

            wheelView.SetZoneVisuals(zoneConfig.WheelSprite, zoneConfig.ArrowSprite);

            var slices = DrawSlices(config, zoneNumber);
            int bombSlotIndex = -1;

            if (config.HasBomb)
                bombSlotIndex = InjectBomb(slices);

            ClearExistingSlices();
            _sliceFactory.CreateSlices(slices, _slots);
            wheelView.SetupSlices(slices);

            return new RuntimeWheelData(slices, bombSlotIndex, config.HasBomb);
        }

        private SliceDefinition[] DrawSlices(WheelConfigSO config, int zoneNumber)
        {
            var pool = config.RewardPool;
            int count = config.SliceCount;
            var slices = new SliceDefinition[count];

            float totalWeight = 0f;
            foreach (var entry in pool)
                totalWeight += entry.Weight;

            for (int i = 0; i < count; i++)
            {
                float roll = Random.Range(0f, totalWeight);
                float accumulated = 0f;
                RewardPoolEntry chosen = pool[pool.Length - 1];

                foreach (var entry in pool)
                {
                    accumulated += entry.Weight;
                    if (roll < accumulated)
                    {
                        chosen = entry;
                        break;
                    }
                }

                float scaledValue = chosen.RewardItem != null
                    ? chosen.RewardItem.Value * (1f + (zoneNumber - 1) * chosen.ZoneValueMultiplier)
                    : 0f;

                slices[i] = new SliceDefinition(chosen.RewardItem, chosen.Weight, scaledValue);
            }

            return slices;
        }

        private int InjectBomb(SliceDefinition[] slices)
        {
            int bombIndex = Random.Range(0, slices.Length);
            slices[bombIndex] = new SliceDefinition(null, 1f, 0f);
            return bombIndex;
        }

        private ZoneConfigSO GetZoneConfig(ZoneType zoneType)
        {
            foreach (var zoneConfig in _zoneConfigs)
            {
                if (zoneConfig.ZoneType == zoneType)
                    return zoneConfig;
            }
            return null;
        }

        private void ClearExistingSlices()
        {
            foreach (var slot in _slots)
            {
                for (int i = slot.Position.childCount - 1; i >= 0; i--)
                    Object.Destroy(slot.Position.GetChild(i).gameObject);
            }
        }
    }
}