using System;

namespace WheelOfFortune.Interfaces
{
    public interface IDialogView
    {
        void ShowBombScreen(Action onRevive, Action onGiveUp);
        void ShowCollectConfirmScreen(Action onConfirm, Action onCancel);
        void Hide();
    }
}
