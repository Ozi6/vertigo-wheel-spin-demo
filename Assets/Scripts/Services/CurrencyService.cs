using System;
using WheelOfFortune.Interfaces;

namespace WheelOfFortune.Services
{
    public sealed class CurrencyService : ICurrencyService
    {
        private int _balance;
        public event Action<int> OnBalanceChanged;

        public CurrencyService(int initialBalance = 1000)
        {
            _balance = initialBalance;
        }

        public int GetBalance() => _balance;

        public bool TryDeduct(int amount)
        {
            if (_balance < amount) return false;
            _balance -= amount;
            OnBalanceChanged?.Invoke(_balance);
            return true;
        }

        public void Add(int amount)
        {
            _balance += amount;
            OnBalanceChanged?.Invoke(_balance);
        }
    }
}