using System;
using WheelOfFortune.Data;
using WheelOfFortune.Interfaces;
using WheelOfFortune.Services;

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
        public readonly ZoneConfigSO[] ZoneConfigs;
        public readonly Action<IGameState> TransitionTo;
        public readonly IWheelSpinStrategy RandomStrategy;

        public GameContext(
            IZoneService zoneService,
            ISpinService spinService,
            IRewardService rewardService,
            IWheelView wheelView,
            IHudView hudView,
            IDialogView dialogView,
            ZoneConfigSO[] zoneConfigs,
            Action<IGameState> transitionTo,
            IWheelSpinStrategy randomStrategy)
        {
            ZoneService = zoneService;
            SpinService = spinService;
            RewardService = rewardService;
            WheelView = wheelView;
            HudView = hudView;
            DialogView = dialogView;
            ZoneConfigs = zoneConfigs;
            TransitionTo = transitionTo;
            RandomStrategy = randomStrategy;
        }
    }
}