using System;
using System.Linq;
using WheelOfFortune.Domain;
using WheelOfFortune.Interfaces;

namespace WheelOfFortune.Services
{
    public class WeightedSpinStrategy : IWheelSpinStrategy
    {
        private readonly Random _rng = new Random();

        public int GetWinningIndex(RuntimeWheelData wheelData)
        {
            if (wheelData.Slices == null || wheelData.Slices.Length == 0)
                return 0;

            float totalWeight = wheelData.Slices.Sum(slice => slice.Weight);
            float randomValue = (float)(_rng.NextDouble() * totalWeight);

            float weightValue = 0f;
            for (int i = 0; i < wheelData.Slices.Length; i++)
            {
                weightValue += wheelData.Slices[i].Weight;
                if (randomValue <= weightValue)
                    return i;
            }

            return wheelData.Slices.Length - 1;
        }
    }
}