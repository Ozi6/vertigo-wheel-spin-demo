using UnityEngine;
using WheelOfFortune.Data;

namespace WheelOfFortune.Factory
{
    public sealed class SliceDrawer
    {
        public SliceDefinition[] DrawSlices(WheelConfigSO config)
        {
            var pool = config.RewardPool;
            int count = config.SliceCount;
            var slices = new SliceDefinition[count];

            float totalWeight = 0f;
            foreach (var entry in pool)
                totalWeight += entry.Weight;

            for (int i = 0; i < count; i++)
            {
                float roll = Random.Range(0f, totalWeight);
                float accumulated = 0f;
                RewardPoolEntry chosen = pool[pool.Length - 1];

                foreach (var entry in pool)
                {
                    accumulated += entry.Weight;
                    if (roll < accumulated)
                    {
                        chosen = entry;
                        break;
                    }
                }

                int multiplier = Random.Range(config.MinMultiplier, config.MaxMultiplier + 1);
                slices[i] = new SliceDefinition(chosen.RewardItem, multiplier);
            }

            return slices;
        }
    }
}