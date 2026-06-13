using UnityEngine;
using WheelOfFortune.Domain;

namespace WheelOfFortune.Data
{
    [CreateAssetMenu(fileName = "ZoneConfig", menuName = "WheelOfFortune/Zone Config")]
    public class ZoneConfigSO : ScriptableObject
    {
        [SerializeField] private ZoneType _zoneType;
        [SerializeField] private WheelConfigSO _wheelConfig;
        [SerializeField] private Sprite _wheelSprite;
        [SerializeField] private Sprite _arrowSprite;

        public ZoneType ZoneType => _zoneType;
        public WheelConfigSO WheelConfig => _wheelConfig;
        public Sprite WheelSprite => _wheelSprite;
        public Sprite ArrowSprite => _arrowSprite;
    }
}