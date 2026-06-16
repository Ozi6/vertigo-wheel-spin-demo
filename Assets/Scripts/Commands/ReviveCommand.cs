using System;
using WheelOfFortune.Interfaces;
using WheelOfFortune.StateMachine;

namespace WheelOfFortune.Commands
{
    public sealed class ReviveCommand : ICommand
    {
        private readonly Func<GameContext> _ctxFactory;
        private readonly int _startingCost;
        private int _nextCost;

        private GameContext Ctx => _ctxFactory();

        public ReviveCommand(Func<GameContext> ctxFactory, int startingCost)
        {
            _ctxFactory = ctxFactory;
            _startingCost = startingCost;
            _nextCost = startingCost;
        }

        public void Execute()
        {
            if (!Ctx.CurrencyService.TryDeduct(_nextCost))
                return;
            _nextCost *= 2;
            Ctx.ButtonView.UpdateReviveCost(_nextCost);
            Ctx.ButtonView.SetReviveInteractable(Ctx.CurrencyService.CanAfford(_nextCost));
            Ctx.TransitionTo(new IdleState());
        }

        public void Reset()
        {
            _nextCost = _startingCost;
            Ctx.ButtonView.UpdateReviveCost(_nextCost);
            Ctx.ButtonView.SetReviveInteractable(Ctx.CurrencyService.CanAfford(_nextCost));
        }
    }
}