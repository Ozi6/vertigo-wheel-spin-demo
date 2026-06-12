using UnityEngine;
using WheelOfFortune.Data;
using WheelOfFortune.Domain;
using WheelOfFortune.Interfaces;

namespace WheelOfFortune.Factory
{
    public sealed class WheelFactory
    {
        private readonly ZoneConfigSO[] _zoneConfigs;
        private readonly SliceFactory _sliceFactory;
        private readonly Transform _sliceParent;

        public WheelFactory(ZoneConfigSO[] zoneConfigs, SliceFactory sliceFactory, Transform sliceParent)
        {
            _zoneConfigs = zoneConfigs;
            _sliceFactory = sliceFactory;
            _sliceParent = sliceParent;
        }

        public void BuildWheel(ZoneType zoneType, IWheelView wheelView)
        {
            var config = GetConfig(zoneType);
            if (config == null)
            {
                Debug.LogError($"[WheelFactory] No WheelConfigSO found for ZoneType {zoneType}");
                return;
            }

            ClearExistingSlices();
            _sliceFactory.CreateSlices(config.Slices, _sliceParent);
            wheelView.SetupSlices(config.Slices);
        }

        private WheelConfigSO GetConfig(ZoneType zoneType)
        {
            foreach (var zoneConfig in _zoneConfigs)
            {
                if (zoneConfig.ZoneType == zoneType)
                    return zoneConfig.WheelConfig;
            }
            return null;
        }

        private void ClearExistingSlices()
        {
            for (int i = _sliceParent.childCount - 1; i >= 0; i--)
                Object.Destroy(_sliceParent.GetChild(i).gameObject);
        }
    }
}
