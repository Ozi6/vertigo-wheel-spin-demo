using System;
using WheelOfFortune.Interfaces;
using WheelOfFortune.StateMachine;

namespace WheelOfFortune.Commands
{
    public sealed class GiveUpCommand : ICommand
    {
        private readonly IZoneService _zoneService;
        private readonly IRewardService _rewardService;
        private readonly Action<IGameState> _transitionTo;

        public GiveUpCommand(
            IZoneService zoneService,
            IRewardService rewardService,
            Action<IGameState> transitionTo)
        {
            _zoneService = zoneService;
            _rewardService = rewardService;
            _transitionTo = transitionTo;
        }

        public void Execute()
        {
            _zoneService.Reset();
            _rewardService.Reset();
            _transitionTo(new IdleState());
        }
    }
}
