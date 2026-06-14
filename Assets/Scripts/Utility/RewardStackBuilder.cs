using System.Collections.Generic;
using WheelOfFortune.Data;
using WheelOfFortune.Domain;

namespace WheelOfFortune.Utility
{
    public static class RewardStackBuilder
    {
        public static List<RewardStack> Build(IReadOnlyList<RewardItemSO> items)
        {
            var countMap = new Dictionary<RewardItemSO, int>();
            var valueMap = new Dictionary<RewardItemSO, float>();
            var order = new List<RewardItemSO>();

            foreach (var item in items)
            {
                if (item == null) continue;

                if (!countMap.ContainsKey(item))
                {
                    countMap[item] = 0;
                    valueMap[item] = 0f;
                    order.Add(item);
                }

                countMap[item]++;
                valueMap[item] += item.Value;
            }

            var result = new List<RewardStack>(order.Count);
            foreach (var item in order)
                result.Add(new RewardStack(item, countMap[item], valueMap[item]));

            return result;
        }
    }
}