using UnityEngine;
using WheelOfFortune.Data;

namespace WheelOfFortune.Factory
{
    public sealed class BombInjector
    {
        public int InjectBomb(SliceDefinition[] slices)
        {
            int bombIndex = Random.Range(0, slices.Length);
            slices[bombIndex] = new SliceDefinition(null, 0, 1f);
            return bombIndex;
        }
    }
}