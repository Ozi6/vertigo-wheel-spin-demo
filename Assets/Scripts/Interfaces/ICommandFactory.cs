namespace WheelOfFortune.Interfaces
{
    public interface ICommandFactory
    {
        ICommand CreateSpinCommand(IGameStateGuard guard, IEventBus eventBus);
        ICommand CreateCollectCommand(IGameStateGuard guard, IEventBus eventBus);
        IReviveCommand CreateReviveCommand(ICurrencyService currency, IEventBus eventBus, int startingCost);
        ICommand CreateGiveUpCommand(IEventBus eventBus);
    }
}