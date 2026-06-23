using WheelOfFortune.Commands;
using WheelOfFortune.Interfaces;

namespace WheelOfFortune.Factory
{
    public sealed class CommandFactory : ICommandFactory
    {
        public ICommand CreateSpinCommand(IGameStateGuard guard, IEventBus eventBus)
        {
            return new SpinCommand(guard, eventBus);
        }

        public ICommand CreateCollectCommand(IGameStateGuard guard, IEventBus eventBus)
        {
            return new CollectCommand(guard, eventBus);
        }

        public IReviveCommand CreateReviveCommand(ICurrencyService currency, IEventBus eventBus, int startingCost)
        {
            return new ReviveCommand(currency, eventBus, startingCost);
        }

        public ICommand CreateGiveUpCommand(IEventBus eventBus)
        {
            return new GiveUpCommand(eventBus);
        }
    }
}