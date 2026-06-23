using WheelOfFortune.Data;
using WheelOfFortune.Domain;
using WheelOfFortune.Events;
using WheelOfFortune.Interfaces;

namespace WheelOfFortune.Services
{
    public class SpinService : ISpinService
    {
        private IWheelSpinStrategy _currentStrategy;
        private readonly IEventBus _eventBus;

        public SpinService(IWheelSpinStrategy defaultStrategy, IEventBus eventBus)
        {
            _currentStrategy = defaultStrategy;
            _eventBus = eventBus;
        }

        public void SetStrategy(IWheelSpinStrategy strategy) => _currentStrategy = strategy;

        public SpinResult Spin(RuntimeWheelData wheelData)
        {
            int winningIndex = _currentStrategy.GetWinningIndex(wheelData);
            RuntimeSlice winningSlice = wheelData.Slices[winningIndex];
            var result = new SpinResult(
                winningSlice.Reward,
                winningSlice.Multiplier,
                winningSlice.IsBomb,
                winningIndex
            );
            _eventBus.Publish(new OnSpinCompleted(result));
            return result;
        }
    }
}