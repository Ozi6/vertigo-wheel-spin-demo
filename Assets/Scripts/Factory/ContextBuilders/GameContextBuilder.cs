using System;
using WheelOfFortune.Commands;
using WheelOfFortune.Data;
using WheelOfFortune.Interfaces;
using WheelOfFortune.StateMachine;
using WheelOfFortune.Events;

namespace WheelOfFortune.Factory
{
    public sealed class GameContextBuilder
    {
        private IZoneService _zoneService;
        private ISpinService _spinService;
        private IRewardService _rewardService;
        private ICurrencyService _currencyService;
        private IWheelView _wheelView;
        private IHudView _hudView;
        private IDialogView _dialogView;
        private IButtonView _buttonView;
        private IWheelFactory _wheelFactory;
        private IWheelSpinStrategy _randomStrategy;
        private WinEffectConfig _winEffectConfig;
        private IEventBus _eventBus;
        private IRewardRegistry _rewardRegistry;

        public GameContextBuilder WithServices(IZoneService zone, ISpinService spin, IRewardService reward, ICurrencyService currency, IEventBus eventBus)
        {
            _zoneService = zone;
            _spinService = spin;
            _rewardService = reward;
            _currencyService = currency;
            _eventBus = eventBus;
            return this;
        }

        public GameContextBuilder WithViews(IWheelView wheel, IHudView hud, IDialogView dialog, IButtonView button)
        {
            _wheelView = wheel;
            _hudView = hud;
            _dialogView = dialog;
            _buttonView = button;
            return this;
        }

        public GameContextBuilder WithInfrastructure(IWheelFactory factory, IWheelSpinStrategy strategy, IRewardRegistry registry, WinEffectConfig config)
        {
            _wheelFactory = factory;
            _randomStrategy = strategy;
            _rewardRegistry = registry;
            _winEffectConfig = config;
            return this;
        }

        public GameContext Build(int reviveStartingCost)
        {
            GameContext context = null;

            var revive = new ReviveCommand(() => context!, reviveStartingCost);
            var giveUp = new GiveUpCommand(_zoneService, _rewardService, _eventBus, revive.Reset);

            Action<IGameState> stateTransitionBridge = state => _eventBus.Publish(new OnStateTransition(state));

            context = new GameContext(
                _zoneService, _spinService, _rewardService, _currencyService,
                _wheelView, _hudView, _dialogView, _buttonView, _wheelFactory,
                stateTransitionBridge, _randomStrategy, _rewardRegistry,
                revive, giveUp, _winEffectConfig);

            return context;
        }
    }
}
