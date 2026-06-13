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

        public void SetStrategy(IWheelSpinStrategy strategy) => _strategy = strategy;

        public SpinResult Spin(RuntimeWheelData wheelData)
        {
            int index = _strategy.GetWinningIndex(wheelData);
            bool isBomb = wheelData.HasBomb && index == wheelData.BombSlotIndex;
            var result = new SpinResult(wheelData.Slices[index].RewardItem, isBomb, index);
            _eventBus.Publish(new OnSpinCompleted(result));
            return result;
        }
    }
}