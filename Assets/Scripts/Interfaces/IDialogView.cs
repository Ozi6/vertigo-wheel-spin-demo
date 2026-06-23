using System;
using WheelOfFortune.Domain;

namespace WheelOfFortune.Interfaces
{
    public interface IDialogView
    {
        void Initialize(IRewardRegistry registry);
        void ShowBombScreen(CollectedRewards lostRewards, Action onRevive, Action onGiveUp);
        void ShowCollectConfirmScreen(CollectedRewards rewards, Action onConfirm, Action onCancel);
        void Hide();

        void UpdateReviveCost(int cost);
        void SetReviveInteractable(bool interactable);
    }
}