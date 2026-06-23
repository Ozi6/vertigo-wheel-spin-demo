using UnityEngine;
using WheelOfFortune.Data;
using WheelOfFortune.Domain;
using WheelOfFortune.Interfaces;
using WheelOfFortune.Views;

namespace WheelOfFortune.Factory
{
    public sealed class WheelFactory : IWheelFactory
    {
        private readonly SlotDefinition[] _slots;
        private readonly SliceFactory _sliceFactory;
        private readonly ZoneConfigSelector _configSelector;
        private readonly SliceDrawer _sliceDrawer;
        private readonly BombInjector _bombInjector;
        private WheelSlice[] _currentSlices;

        public WheelFactory(
            ZoneConfigSO[] zoneConfigs,
            SliceFactory sliceFactory,
            ISlotFactory slotFactory,
            Transform slotParent,
            int slotCount)
        {
            _sliceFactory = sliceFactory;
            _slots = slotFactory.CreateSlots(slotParent, slotCount);
            _configSelector = new ZoneConfigSelector(zoneConfigs);
            _sliceDrawer = new SliceDrawer();
            _bombInjector = new BombInjector();
        }

        public RuntimeWheelData BuildWheel(ZoneType zoneType, int zoneNumber, IWheelView wheelView)
        {
            var zoneConfig = _configSelector.GetZoneConfig(zoneType);
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

            var slices = _sliceDrawer.DrawSlices(config);
            int bombSlotIndex = -1;

            if (config.HasBomb)
                bombSlotIndex = _bombInjector.InjectBomb(slices);

            ClearExistingSlices();
            var sliceInstances = _sliceFactory.CreateSlices(slices, _slots);
            _currentSlices = sliceInstances;
            wheelView.SetupSlices(slices);
            wheelView.SetLiveSlices(sliceInstances);

            var runtimeSlices = new RuntimeSlice[slices.Length];
            for (int i = 0; i < slices.Length; i++)
            {
                var def = slices[i];
                var data = def.RewardItem != null ? def.RewardItem.ToData() : default;
                runtimeSlices[i] = new RuntimeSlice(data, def.Multiplier, def.IsBomb, def.Weight);
            }

            return new RuntimeWheelData(runtimeSlices, bombSlotIndex, config.HasBomb, config.IsWeighted);
        }

        private void ClearExistingSlices()
        {
            if (_currentSlices != null)
            {
                _sliceFactory.ReturnSlices(_currentSlices);
                _currentSlices = null;
            }

            foreach (var slot in _slots)
            {
                for (int i = slot.Position.childCount - 1; i >= 0; i--)
                    Object.Destroy(slot.Position.GetChild(i).gameObject);
            }
        }
    }
}