using WheelOfFortune.Interfaces;
using WheelOfFortune.StateMachine;

namespace WheelOfFortune.Commands
{
    public sealed class ReviveCommand : ICommand
    {
        private readonly GameContext _ctx;
        private readonly int _startingCost;
        private int _nextCost;

        public ReviveCommand(GameContext ctx, int startingCost)
        {
            _ctx = ctx;
            _startingCost = startingCost;
            _nextCost = startingCost;
        }

        public void Execute()
        {
            if (!_ctx.CurrencyService.TryDeduct(_nextCost))
                return;

            _nextCost *= 2;
            _ctx.ButtonView.UpdateReviveCost(_nextCost);
            _ctx.ButtonView.SetReviveInteractable(_ctx.CurrencyService.CanAfford(_nextCost));
            _ctx.TransitionTo(new IdleState());
        }

        public void Reset()
        {
            _nextCost = _startingCost;
            _ctx.ButtonView.UpdateReviveCost(_nextCost);
            _ctx.ButtonView.SetReviveInteractable(_ctx.CurrencyService.CanAfford(_nextCost));
        }
    }
}