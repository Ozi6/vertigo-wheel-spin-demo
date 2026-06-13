using WheelOfFortune.Domain;
using WheelOfFortune.Interfaces;
using Random = UnityEngine.Random;

namespace WheelOfFortune.Services
{
    public class RandomSpinStrategy : IWheelSpinStrategy
    {
        public int GetWinningIndex(RuntimeWheelData wheelData)
        {
            return Random.Range(0, wheelData.Slices.Length);
        }
    }
}