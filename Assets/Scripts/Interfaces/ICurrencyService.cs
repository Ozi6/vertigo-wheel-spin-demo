using System;

namespace WheelOfFortune.Interfaces
{
    public interface ICurrencyService
    {
        int GetBalance();
        bool CanAfford(int amount);
        bool TryDeduct(int amount);
        void Add(int amount);
    }
}