using System;
using UnityEngine;

namespace WheelOfFortune.Data
{
    [Serializable]
    public struct SlotDefinition
    {
        [SerializeField] private int _index;
        [SerializeField] private Transform _position;
        
        public int Index => _index;
        public Transform Position => _position;

        public SlotDefinition(int index, Transform position)
        {
            _index = index;
            _position = position;
        }
    }
}