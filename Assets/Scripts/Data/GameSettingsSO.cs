using UnityEngine;

namespace WheelOfFortune.Data
{
    [CreateAssetMenu(fileName = "GameSettings", menuName = "WheelOfFortune/Game Settings")]
    public class GameSettingsSO : ScriptableObject
    {
        [SerializeField, Min(1)] private int _safeZoneInterval = 5;
        [SerializeField, Min(1)] private int _superZoneInterval = 30;
        [SerializeField, Min(1)] private int _startingReviveCost = 50;
        [SerializeField, Min(1)] private int _startingCurrencyBalance = 1000;

        public int SafeZoneInterval => _safeZoneInterval;
        public int SuperZoneInterval => _superZoneInterval;
        public int StartingReviveCost => _startingReviveCost;
        public int StartingCurrencyBalance => _startingCurrencyBalance;

        private void OnValidate()
        {
            if (_safeZoneInterval < 1) _safeZoneInterval = 1;
            if (_superZoneInterval < 1) _superZoneInterval = 1;
            if (_startingReviveCost < 1) _startingReviveCost = 1;
            if (_startingCurrencyBalance < 1) _startingCurrencyBalance = 1;
        }
    }
}