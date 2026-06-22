using System;
using WheelOfFortune.Interfaces;
using WheelOfFortune.StateMachine;
using WheelOfFortune.Events;

namespace WheelOfFortune.Commands
{
    public sealed class GiveUpCommand : ICommand
    {
        private readonly IZoneService _zoneService;
        private readonly IRewardService _rewardService;
        private readonly IEventBus _eventBus;
        private readonly Action _resetReviveCost;

        public GiveUpCommand(
            IZoneService zoneService,
            IRewardService rewardService,
            IEventBus eventBus,
            Action resetReviveCost)
        {
            _zoneService = zoneService;
            _rewardService = rewardService;
            _eventBus = eventBus;
            _resetReviveCost = resetReviveCost;
        }

        public void Execute()
        {
            _zoneService.Reset();
            _rewardService.Reset();
            _resetReviveCost();
            _eventBus.Publish(new OnStateTransition(new IdleState()));
        }
    }
}