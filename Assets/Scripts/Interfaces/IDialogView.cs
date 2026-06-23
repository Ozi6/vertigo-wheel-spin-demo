using WheelOfFortune.Domain;

namespace WheelOfFortune.Interfaces
{
    public interface IDialogView
    {
        void Initialize(IEventBus eventBus, IRewardRegistry registry);
        void ShowBombScreen(CollectedRewards lostRewards, int currentReviveCost, bool canAfford);
        void ShowCollectConfirmScreen(CollectedRewards rewards);
        void Hide();
    }
}