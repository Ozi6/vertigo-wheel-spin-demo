using System;
using WheelOfFortune.Commands;
using WheelOfFortune.Data;
using WheelOfFortune.Factory;
using WheelOfFortune.Interfaces;

namespace WheelOfFortune.StateMachine
{
    public sealed class GameContext
    {
        public readonly IZoneService ZoneService;
        public readonly ISpinService SpinService;
        public readonly IRewardService RewardService;
        public readonly ICurrencyService CurrencyService;
        public readonly IWheelView WheelView;
        public readonly IHudView HudView;
        public readonly IDialogView DialogView;
        public readonly IButtonView ButtonView;
        public readonly IWheelFactory WheelFactory;
        public readonly IEventBus EventBus;
        public readonly IWheelSpinStrategy RandomStrategy;
        public readonly IRewardRegistry RewardRegistry;
        public readonly ReviveCommand ReviveCommand;
        public readonly GiveUpCommand GiveUpCommand;
        public readonly WinEffectConfig WinEffectConfig;

        public GameContext(
            IZoneService zoneService,
            ISpinService spinService,
            IRewardService rewardService,
            ICurrencyService currencyService,
            IWheelView wheelView,
            IHudView hudView,
            IDialogView dialogView,
            IButtonView buttonView,
            IWheelFactory wheelFactory,
            IEventBus eventBus,
            IWheelSpinStrategy randomStrategy,
            IRewardRegistry rewardRegistry,
            ReviveCommand reviveCommand,
            GiveUpCommand giveUpCommand,
            WinEffectConfig winEffectConfig)
        {
            ZoneService = zoneService;
            SpinService = spinService;
            RewardService = rewardService;
            CurrencyService = currencyService;
            WheelView = wheelView;
            HudView = hudView;
            DialogView = dialogView;
            ButtonView = buttonView;
            WheelFactory = wheelFactory;
            EventBus = eventBus;
            RandomStrategy = randomStrategy;
            RewardRegistry = rewardRegistry;
            ReviveCommand = reviveCommand;
            GiveUpCommand = giveUpCommand;
            WinEffectConfig = winEffectConfig;
        }
    }
}