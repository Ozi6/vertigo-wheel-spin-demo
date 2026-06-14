using System.Collections.Generic;
using WheelOfFortune.Data;
using WheelOfFortune.Domain;

namespace WheelOfFortune.Utility
{
    public static class RewardStackBuilder
    {
        public static List<RewardStack> Build(IReadOnlyList<CollectedRewards.Entry> entries)
        {
            var multiplierMap = new Dictionary<RewardItemSO, int>();
            var order = new List<RewardItemSO>();

            foreach (var entry in entries)
            {
                if (entry.Item == null) continue;

                if (!multiplierMap.ContainsKey(entry.Item))
                {
                    multiplierMap[entry.Item] = 0;
                    order.Add(entry.Item);
                }

                multiplierMap[entry.Item] += entry.Multiplier;
            }

            var result = new List<RewardStack>(order.Count);
            foreach (var item in order)
                result.Add(new RewardStack(item, multiplierMap[item]));

            return result;
        }
    }
}