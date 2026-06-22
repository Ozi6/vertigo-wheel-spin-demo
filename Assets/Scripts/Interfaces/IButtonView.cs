using System;

namespace WheelOfFortune.Interfaces
{
    public interface IButtonView
    {
        void SetSpinInteractable(bool interactable);
        void SetCollectVisible(bool visible);
    }
}