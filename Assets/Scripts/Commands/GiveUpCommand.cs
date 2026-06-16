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
        private readonly Action _resetReviveCost;

        public GiveUpCommand(
            IZoneService zoneService,
            IRewardService rewardService,
            Action<IGameState> transitionTo,
            Action resetReviveCost)
        {
            _zoneService = zoneService;
            _rewardService = rewardService;
            _transitionTo = transitionTo;
            _resetReviveCost = resetReviveCost;
        }

        public void Execute()
        {
            _zoneService.Reset();
            _rewardService.Reset();
            _resetReviveCost();
            _transitionTo(new IdleState());
        }
    }
}