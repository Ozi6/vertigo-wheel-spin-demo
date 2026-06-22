using WheelOfFortune.Interfaces;
using WheelOfFortune.Events;

namespace WheelOfFortune.Services
{
    public sealed class CurrencyService : ICurrencyService
    {
        private readonly IEventBus _eventBus;
        private int _balance;

        public CurrencyService(IEventBus eventBus, int initialBalance = 1000)
        {
            _eventBus = eventBus;
            _balance = initialBalance;
        }

        public int GetBalance() => _balance;

        public bool CanAfford(int amount) => _balance >= amount;

        public bool TryDeduct(int amount)
        {
            if (_balance < amount) return false;
            _balance -= amount;
            _eventBus.Publish(new OnBalanceChange(_balance));
            return true;
        }

        public void Add(int amount)
        {
            _balance += amount;
            _eventBus.Publish(new OnBalanceChange(_balance));
        }
    }
}