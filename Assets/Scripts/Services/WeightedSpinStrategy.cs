using System.Linq;
using WheelOfFortune.Domain;
using WheelOfFortune.Interfaces;
using Random = UnityEngine.Random;

namespace WheelOfFortune.Services
{
    public class WeightedSpinStrategy : IWheelSpinStrategy
    {
        public int GetWinningIndex(RuntimeWheelData wheelData)
        {
            if (wheelData.Slices == null || wheelData.Slices.Length == 0)
                return 0;

            float totalWeight = wheelData.Slices.Sum(slice => slice.Weight);
            float randomValue = Random.Range(0f, totalWeight);

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