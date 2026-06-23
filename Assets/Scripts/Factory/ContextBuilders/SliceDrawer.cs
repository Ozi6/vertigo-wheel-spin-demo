using UnityEngine;
using WheelOfFortune.Data;

using WheelOfFortune.Interfaces;

namespace WheelOfFortune.Factory
{
    public sealed class SliceDrawer : ISliceDrawer
    {
        public SliceDefinition[] DrawSlices(WheelConfigSO config)
        {
            var pool = config.RewardPool;
            int count = config.SliceCount;
            var slices = new SliceDefinition[count];

            if (pool == null || pool.Length == 0)
                return slices;

            float totalWeight = 0f;
            if (config.IsWeighted)
                foreach (var entry in pool)
                    totalWeight += entry.Weight;

            for (int i = 0; i < count; i++)
            {
                RewardPoolEntry chosen = pool[0];

                if (config.IsWeighted && totalWeight > 0f)
                {
                    float roll = Random.Range(0f, totalWeight);
                    float accumulated = 0f;
                    chosen = pool[pool.Length - 1];

                    foreach (var entry in pool)
                    {
                        accumulated += entry.Weight;
                        if (roll <= accumulated)
                        {
                            chosen = entry;
                            break;
                        }
                    }
                }
                else
                {
                    int roll = Random.Range(0, pool.Length);
                    chosen = pool[roll];
                }

                int multiplier = Random.Range(config.MinMultiplier, config.MaxMultiplier + 1);
                slices[i] = new SliceDefinition(chosen.RewardItem, multiplier, chosen.Weight);
            }

            return slices;
        }
    }
}