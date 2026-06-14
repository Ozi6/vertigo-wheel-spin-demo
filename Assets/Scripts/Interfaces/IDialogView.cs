using System;
using WheelOfFortune.Domain;

namespace WheelOfFortune.Interfaces
{
    public interface IDialogView
    {
        void ShowBombScreen(CollectedRewards lostRewards, Action onRevive, Action onGiveUp);
        void ShowCollectConfirmScreen(CollectedRewards rewards, Action onConfirm, Action onCancel);
        void Hide();
    }
}
