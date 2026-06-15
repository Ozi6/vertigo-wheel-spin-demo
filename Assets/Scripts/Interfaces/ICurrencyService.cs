using System;

namespace WheelOfFortune.Interfaces
{
    public interface ICurrencyService
    {
        int GetBalance();
        bool TryDeduct(int amount);
        void Add(int amount);
        event Action<int> OnBalanceChanged;
    }
}