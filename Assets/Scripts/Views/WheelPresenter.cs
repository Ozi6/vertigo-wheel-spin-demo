using System;
using UnityEngine;
using WheelOfFortune.Data;
using WheelOfFortune.Interfaces;

namespace WheelOfFortune.Views
{
    public sealed class WheelPresenter : MonoBehaviour, IWheelView
    {
        [SerializeField] private Transform _wheelRoot;

        private SliceDefinition[] _currentSlices;

        public void SetupSlices(SliceDefinition[] slices)
        {
            _currentSlices = slices;
        }

        public void SpinTo(int targetSliceIndex, Action onComplete)
        {
            onComplete?.Invoke();
        }
    }
}
