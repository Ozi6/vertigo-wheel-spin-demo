using System.Collections.Generic;
using WheelOfFortune.Data;
using WheelOfFortune.Domain;

namespace WheelOfFortune.Utility
{
    public static class RewardStackBuilder
    {
        public static List<RewardStack> Build(IReadOnlyList<CollectedRewards.Entry> entries)
        {
            var multiplierMap = new Dictionary<string, int>();
            var itemMap = new Dictionary<string, RewardData>();
            var order = new List<string>();

            foreach (var entry in entries)
            {
                if (string.IsNullOrEmpty(entry.Item.Id)) continue;

                if (!multiplierMap.ContainsKey(entry.Item.Id))
                {
                    multiplierMap[entry.Item.Id] = 0;
                    itemMap[entry.Item.Id] = entry.Item;
                    order.Add(entry.Item.Id);
                }

                multiplierMap[entry.Item.Id] += entry.Multiplier;
            }

            var result = new List<RewardStack>(order.Count);
            foreach (var id in order)
                result.Add(new RewardStack(itemMap[id], multiplierMap[id]));

            return result;
        }
    }
}