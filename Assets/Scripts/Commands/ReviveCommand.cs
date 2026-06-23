using System;
using WheelOfFortune.Interfaces;
using WheelOfFortune.StateMachine;
using WheelOfFortune.Events;

namespace WheelOfFortune.Commands
{
    public sealed class ReviveCommand : ICommand
    {
        private readonly ICurrencyService _currencyService;
        private readonly IEventBus _eventBus;
        private readonly int _startingCost;
        private int _nextCost;

        public int CurrentCost => _nextCost;

        public ReviveCommand(
            ICurrencyService currencyService,
            IEventBus eventBus,
            int startingCost)
        {
            _currencyService = currencyService;
            _eventBus = eventBus;
            _startingCost = startingCost;
            _nextCost = startingCost;
        }

        public void Execute()
        {
            if (!_currencyService.TryDeduct(_nextCost))
                return;
            
            _nextCost *= 2;
            _eventBus.Publish(new OnReviveCostChanged(_nextCost));
            _eventBus.Publish(new OnStateTransition(new IdleState()));
        }

        public void Reset()
        {
            _nextCost = _startingCost;
            _eventBus.Publish(new OnReviveCostChanged(_nextCost));
        }
    }
}