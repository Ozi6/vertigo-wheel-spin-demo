using System;
using WheelOfFortune.Commands;
using WheelOfFortune.Factory;
using WheelOfFortune.Interfaces;

namespace WheelOfFortune.StateMachine
{
    public sealed class GameContext
    {
        public readonly IZoneService ZoneService;
        public readonly ISpinService SpinService;
        public readonly IRewardService RewardService;
        public readonly IWheelView WheelView;
        public readonly IHudView HudView;
        public readonly IDialogView DialogView;
        public readonly IButtonView ButtonView;
        public readonly IWheelFactory WheelFactory;
        public readonly Action<IGameState> TransitionTo;
        public readonly IWheelSpinStrategy RandomStrategy;
        public readonly ReviveCommand ReviveCommand;
        public readonly GiveUpCommand GiveUpCommand;

        public GameContext(
            IZoneService zoneService,
            ISpinService spinService,
            IRewardService rewardService,
            IWheelView wheelView,
            IHudView hudView,
            IDialogView dialogView,
            IButtonView buttonView,
            IWheelFactory wheelFactory,
            Action<IGameState> transitionTo,
            IWheelSpinStrategy randomStrategy,
            ReviveCommand reviveCommand,
            GiveUpCommand giveUpCommand)
        {
            ZoneService = zoneService;
            SpinService = spinService;
            RewardService = rewardService;
            WheelView = wheelView;
            HudView = hudView;
            DialogView = dialogView;
            ButtonView = buttonView;
            WheelFactory = wheelFactory;
            TransitionTo = transitionTo;
            RandomStrategy = randomStrategy;
            ReviveCommand = reviveCommand;
            GiveUpCommand = giveUpCommand;
        }
    }
}