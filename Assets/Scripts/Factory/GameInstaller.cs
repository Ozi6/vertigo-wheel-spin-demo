using UnityEngine;
using WheelOfFortune.Controller;
using WheelOfFortune.Data;
using WheelOfFortune.Events;
using WheelOfFortune.Factory;
using WheelOfFortune.Interfaces;
using WheelOfFortune.Services;
using WheelOfFortune.Views;

namespace WheelOfFortune.Installer
{
    [DefaultExecutionOrder(-100)]
    public sealed class GameInstaller : MonoBehaviour
    {
        [SerializeField] private GameSettingsSO _gameSettings;
        [SerializeField] private ZoneConfigSO[] _zoneConfigs;
        [SerializeField] private WheelSlice _slicePrefab;
        [SerializeField] private Sprite _bombIcon;
        [SerializeField] private Transform _slotParent;
        [SerializeField, Min(1)] private int _slotCount = 8;
        [SerializeField] private GameController _gameController;
        [SerializeField] private Transform _uiRoot;

        private void Awake()
        {
            var eventBus = new EventBus();

            var zoneService = new ZoneService(_gameSettings, eventBus);
            var randomStrategy = new RandomSpinStrategy();
            var weightedStrategy = new WeightedSpinStrategy();
            var spinService = new SpinService(randomStrategy, eventBus);
            var rewardService = new RewardService(eventBus);
            var currencyService = new CurrencyService(eventBus, _gameSettings.StartingCurrencyBalance);

            var configSelector = new ZoneConfigSelector(_zoneConfigs);
            var sliceDrawer = new SliceDrawer();
            var bombInjector = new BombInjector();
            var commandFactory = new CommandFactory();

            var sliceFactory = new SliceFactory(_slicePrefab, _bombIcon);
            var slotFactory = new SlotFactory();
            var wheelFactory = new WheelFactory(sliceFactory, slotFactory, _slotParent, _slotCount, configSelector, sliceDrawer, bombInjector);

            var wheelView = _uiRoot.GetComponentInChildren<IWheelView>(true);
            var hudView = _uiRoot.GetComponentInChildren<IHudView>(true);
            var dialogView = _uiRoot.GetComponentInChildren<IDialogView>(true);
            var buttonView = _uiRoot.GetComponentInChildren<IButtonView>(true);
            var buttonPresenter = _uiRoot.GetComponentInChildren<ButtonPresenter>(true);

            ValidateDependencies(wheelView, hudView, dialogView, buttonView, buttonPresenter);

            var rewardRegistry = new RewardRegistry(_zoneConfigs);

            wheelView.Initialize(eventBus);
            hudView.Initialize(eventBus, rewardRegistry);
            dialogView.Initialize(eventBus, rewardRegistry);

            wheelFactory.BuildWheel(zoneService.GetCurrentZoneType(), zoneService.GetCurrentZoneNumber(), wheelView);

            _gameController.Init(
                zoneService,
                spinService,
                rewardService,
                currencyService,
                wheelView,
                hudView,
                dialogView,
                buttonView,
                wheelFactory,
                commandFactory,
                randomStrategy,
                weightedStrategy,
                rewardRegistry,
                _gameSettings,
                eventBus);
        }

        private void ValidateDependencies(
            IWheelView wheelView,
            IHudView hudView,
            IDialogView dialogView,
            IButtonView buttonView,
            ButtonPresenter buttonPresenter)
        {
            if (wheelView == null) Debug.LogError("[GameInstaller] IWheelView not found in scene.");
            if (hudView == null) Debug.LogError("[GameInstaller] IHudView not found in scene.");
            if (dialogView == null) Debug.LogError("[GameInstaller] IDialogView not found in scene.");
            if (buttonView == null) Debug.LogError("[GameInstaller] IButtonView not found in scene.");
            if (buttonPresenter == null) Debug.LogError("[GameInstaller] ButtonPresenter not found in scene.");
        }

        private void OnValidate()
        {
            if (_slotCount < 1) _slotCount = 1;
        }
    }
}