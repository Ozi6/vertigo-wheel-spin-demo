using TMPro;
using UnityEngine;
using WheelOfFortune.Domain;
using WheelOfFortune.Interfaces;

namespace WheelOfFortune.Views
{
    public sealed class HudPresenter : MonoBehaviour, IHudView
    {
        [SerializeField] private TextMeshProUGUI _zoneLabel_value;
        [SerializeField] private Transform _rewardsContainer_value;

        public void UpdateZoneDisplay(ZoneProgressModel progress)
        {
            if (_zoneLabel_value != null)
                _zoneLabel_value.text = $"Zone {progress.ZoneNumber}";
        }

        public void UpdateRewardsDisplay(CollectedRewards rewards)
        {
        }
    }
}
