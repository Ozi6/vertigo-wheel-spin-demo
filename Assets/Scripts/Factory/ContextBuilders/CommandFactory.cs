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
    }
}