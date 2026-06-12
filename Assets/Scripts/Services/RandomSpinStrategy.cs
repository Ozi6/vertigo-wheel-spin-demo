using WheelOfFortune.Data;
using WheelOfFortune.Interfaces;
using Random = UnityEngine.Random;

namespace WheelOfFortune.Services
{
    public class RandomSpinStrategy : IWheelSpinStrategy
    {
        public int GetWinningIndex(WheelConfigSO config)
        {
            return Random.Range(0, config.Slices.Length);
        }
    }
}