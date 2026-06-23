using System;
using WheelOfFortune.Domain;
using WheelOfFortune.Interfaces;

namespace WheelOfFortune.Services
{
    public class RandomSpinStrategy : IWheelSpinStrategy
    {
        private readonly Random _rng = new Random();

        public int GetWinningIndex(RuntimeWheelData wheelData)
        {
            return _rng.Next(0, wheelData.Slices.Length);
        }
    }
}