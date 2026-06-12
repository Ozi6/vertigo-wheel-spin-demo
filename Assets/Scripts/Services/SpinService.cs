using WheelOfFortune.Data;
using WheelOfFortune.Domain;
using WheelOfFortune.Events;
using WheelOfFortune.Interfaces;

namespace WheelOfFortune.Services
{
    public class SpinService : ISpinService
    {
        private readonly IEventBus _eventBus;
        private IWheelSpinStrategy _strategy;

        public SpinService(IWheelSpinStrategy strategy, IEventBus eventBus)
        {
            _strategy = strategy;
            _eventBus = eventBus;
        }

        public void SetStrategy(IWheelSpinStrategy strategy)
        {
            _strategy = strategy;
        }

        public SpinResult Spin(WheelConfigSO config)
        {
            int index = _strategy.GetWinningIndex(config);
            bool isBomb = config.HasBomb && index == config.BombSlotIndex;
            SpinResult result = new SpinResult(config.Slices[index].RewardItem, isBomb, index);
            _eventBus.Publish(new OnSpinCompleted(result));
            return result;
        }
    }
}