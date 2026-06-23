using WheelOfFortune.Commands;
using WheelOfFortune.Interfaces;

namespace WheelOfFortune.Factory
{
    public sealed class CommandFactory
    {
        public SpinCommand CreateSpinCommand(IGameStateGuard guard, IEventBus eventBus)
        {
            return new SpinCommand(guard, eventBus);
        }

        public CollectCommand CreateCollectCommand(IGameStateGuard guard, IEventBus eventBus)
        {
            return new CollectCommand(guard, eventBus);
        }

        public ReviveCommand CreateReviveCommand(ICurrencyService currency, IEventBus eventBus, int startingCost)
        {
            return new ReviveCommand(currency, eventBus, startingCost);
        }

        public GiveUpCommand CreateGiveUpCommand(IEventBus eventBus)
        {
            return new GiveUpCommand(eventBus);
        }
    }
}