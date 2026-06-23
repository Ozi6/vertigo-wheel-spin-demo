using System.Collections.Generic;
using WheelOfFortune.Interfaces;

namespace WheelOfFortune.Data
{
    public sealed class RewardRegistry : IRewardRegistry
    {
        private readonly Dictionary<string, RewardItemSO> _map = new Dictionary<string, RewardItemSO>();

        public RewardRegistry(ZoneConfigSO[] configs)
        {
            if (configs == null) return;
            foreach (var c in configs)
            {
                if (c == null || c.WheelConfig == null) continue;
                if (c.WheelConfig.RewardPool == null) continue;
                foreach (var poolEntry in c.WheelConfig.RewardPool)
                {
                    if (poolEntry.RewardItem != null && !_map.ContainsKey(poolEntry.RewardItem.Id))
                        _map[poolEntry.RewardItem.Id] = poolEntry.RewardItem;
                }
            }
        }

        public RewardItemSO GetReward(string id)
        {
            return !string.IsNullOrEmpty(id) && _map.TryGetValue(id, out var item) ? item : null;
        }
    }
}
