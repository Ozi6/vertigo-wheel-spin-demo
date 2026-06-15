using UnityEngine;

namespace WheelOfFortune.Data
{
    [CreateAssetMenu(fileName = "GameSettings", menuName = "WheelOfFortune/Game Settings")]
    public class GameSettingsSO : ScriptableObject
    {
        [SerializeField] private int _safeZoneInterval = 5;
        [SerializeField] private int _superZoneInterval = 30;
        [SerializeField] private int _startingReviveCost = 50;
        [SerializeField] private int _startingCurrencyBalance = 1000;

        public int SafeZoneInterval => _safeZoneInterval;
        public int SuperZoneInterval => _superZoneInterval;
        public int StartingReviveCost => _startingReviveCost;
        public int StartingCurrencyBalance => _startingCurrencyBalance;
    }
}