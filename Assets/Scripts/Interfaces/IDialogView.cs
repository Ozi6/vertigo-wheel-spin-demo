using System;
using WheelOfFortune.Domain;

namespace WheelOfFortune.Interfaces
{
    public interface IDialogView
    {
        void Initialize(IEventBus eventBus, IRewardRegistry registry);
        void ShowBombScreen(CollectedRewards lostRewards, int currentReviveCost, bool canAfford, Action onRevive, Action onGiveUp);
        void ShowCollectConfirmScreen(CollectedRewards rewards, Action onConfirm, Action onCancel);
        void Hide();
    }
}