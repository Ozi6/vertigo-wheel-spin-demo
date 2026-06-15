using WheelOfFortune.Interfaces;
using WheelOfFortune.StateMachine;

namespace WheelOfFortune.Commands
{
    public sealed class ReviveCommand : ICommand
    {
        private readonly GameContext _ctx;
        private int _nextCost;

        public ReviveCommand(GameContext ctx, int startingCost)
        {
            _ctx = ctx;
            _nextCost = startingCost;
        }

        public void Execute()
        {
            if (!_ctx.CurrencyService.TryDeduct(_nextCost))
                return;

            _nextCost *= 2;
            _ctx.ButtonView.UpdateReviveCost(_nextCost);
            _ctx.TransitionTo(new IdleState());
        }
    }
}