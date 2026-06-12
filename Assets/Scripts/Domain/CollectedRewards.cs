using System.Collections.Generic;
using WheelOfFortune.Data;

namespace WheelOfFortune.Domain
{
    public sealed class CollectedRewards
    {
        private readonly List<RewardItemSO> _items = new List<RewardItemSO>();

        public IReadOnlyList<RewardItemSO> Items => _items;

        public void Add(RewardItemSO item)
        {
            _items.Add(item);
        }

        public void Clear()
        {
            _items.Clear();
        }

        public CollectedRewards Clone()
        {
            CollectedRewards clone = new CollectedRewards();
            foreach (RewardItemSO item in _items)
                clone._items.Add(item);
            return clone;
        }
    }
}
