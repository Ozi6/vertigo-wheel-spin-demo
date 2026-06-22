using System;
using UnityEngine;

namespace WheelOfFortune.Data
{
    [Serializable]
    public struct SlotDefinition
    {
        [SerializeField, Min(0)] private int _index;
        [SerializeField] private Transform _position;

        public int Index => _index;
        public Transform Position => _position;

        public SlotDefinition(int index, Transform position)
        {
            _index = Mathf.Max(0, index);
            _position = position;
        }
    }
}