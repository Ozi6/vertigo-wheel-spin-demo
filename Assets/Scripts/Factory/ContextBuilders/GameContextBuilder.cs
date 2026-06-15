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

        public GameContextBuilder WithServices(IZoneService zone, ISpinService spin, IRewardService reward, ICurrencyService currency)
        {
            _zoneService = zone;
            _spinService = spin;
            _rewardService = reward;
            _currencyService = currency;
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

        public GameContext Build(Func<GameContext, ReviveCommand> reviveFactory, Func<GameContext, GiveUpCommand> giveUpFactory)
        {
            var context = new GameContext(
                _zoneService, _spinService, _rewardService, _currencyService,
                _wheelView, _hudView, _dialogView, _buttonView, _wheelFactory,
                _transitionTo, _randomStrategy, null, null, _winEffectConfig);

            var fields = typeof(GameContext).GetFields(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
            foreach (var f in fields)
            {
                if (f.Name == nameof(GameContext.ReviveCommand)) f.SetValue(context, reviveFactory(context));
                if (f.Name == nameof(GameContext.GiveUpCommand)) f.SetValue(context, giveUpFactory(context));
            }

            return context;
        }
    }
}