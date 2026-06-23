using System;
using System.Collections.Generic;
using WheelOfFortune.Data;

namespace WheelOfFortune.Domain
{
    public sealed class CollectedRewards
    {
        public readonly struct Entry
        {
            public readonly RewardData Item;
            public readonly int Multiplier;

            public Entry(RewardData item, int multiplier)
            {
                Item = item;
                Multiplier = multiplier;
            }
        }

        private readonly List<Entry> _entries = new List<Entry>();

        public IReadOnlyList<Entry> Entries => _entries;

        public void Add(RewardData item, int multiplier)
        {
            _entries.Add(new Entry(item, multiplier));
        }

        public void Clear()
        {
            _entries.Clear();
        }

        public CollectedRewards Clone()
        {
            var clone = new CollectedRewards();
            foreach (var e in _entries)
                clone._entries.Add(e);
            return clone;
        }
    }
}