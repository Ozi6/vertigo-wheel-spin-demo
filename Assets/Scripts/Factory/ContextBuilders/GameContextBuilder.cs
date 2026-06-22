using System;
using WheelOfFortune.Commands;
using WheelOfFortune.Data;
using WheelOfFortune.Interfaces;
using WheelOfFortune.StateMachine;

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
        private Action<IGameState> _transitionTo;
        private IWheelSpinStrategy _randomStrategy;
        private WinEffectConfig _winEffectConfig;
        private IEventBus _eventBus;

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

        public GameContextBuilder WithInfrastructure(IWheelFactory factory, Action<IGameState> transitionTo, IWheelSpinStrategy strategy, WinEffectConfig config)
        {
            _wheelFactory = factory;
            _transitionTo = transitionTo;
            _randomStrategy = strategy;
            _winEffectConfig = config;
            return this;
        }

        public GameContext Build(int reviveStartingCost)
        {
            GameContext context = null;

            var revive = new ReviveCommand(() => context!, reviveStartingCost);
            var giveUp = new GiveUpCommand(_zoneService, _rewardService, _eventBus, revive.Reset);

            context = new GameContext(
                _zoneService, _spinService, _rewardService, _currencyService,
                _wheelView, _hudView, _dialogView, _buttonView, _wheelFactory,
                _transitionTo, _randomStrategy,
                revive, giveUp, _winEffectConfig);

            return context;
        }
    }
}