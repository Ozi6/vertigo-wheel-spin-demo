using UnityEngine;
using WheelOfFortune.Data;

using WheelOfFortune.Interfaces;

namespace WheelOfFortune.Factory
{
    public sealed class BombInjector : IBombInjector
    {
        public int InjectBomb(SliceDefinition[] slices)
        {
            int bombIndex = Random.Range(0, slices.Length);
            slices[bombIndex] = new SliceDefinition(null, 0, 1f);
            return bombIndex;
        }
    }
}