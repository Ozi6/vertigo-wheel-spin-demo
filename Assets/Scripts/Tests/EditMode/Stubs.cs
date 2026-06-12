using System;
using System.Collections.Generic;
using WheelOfFortune.Data;
using WheelOfFortune.Domain;
using WheelOfFortune.Interfaces;

namespace WheelOfFortune.Tests.EditMode.Stubs
{
    internal sealed class StubZoneService : IZoneService
    {
        public int CurrentZone = 1;
        public ZoneType ZoneTypeToReturn = ZoneType.Normal;
        public bool CanLeave = false;
        public int AdvanceCallCount;
        public int ResetCallCount;

        public ZoneType GetCurrentZoneType() => ZoneTypeToReturn;
        public int GetCurrentZoneNumber() => CurrentZone;

        public ZoneProgressModel Advance()
        {
            AdvanceCallCount++;
            CurrentZone++;
            return new ZoneProgressModel(CurrentZone, ZoneTypeToReturn);
        }

        public bool CanPlayerLeave() => CanLeave;

        public void Reset()
        {
            ResetCallCount++;
            CurrentZone = 1;
        }
    }

    internal sealed class StubSpinService : ISpinService
    {
        public SpinResult ResultToReturn;
        public IWheelSpinStrategy LastStrategySet;
        public int SpinCallCount;

        public SpinResult Spin(WheelConfigSO config)
        {
            SpinCallCount++;
            return ResultToReturn;
        }

        public void SetStrategy(IWheelSpinStrategy strategy)
        {
            LastStrategySet = strategy;
        }
    }

    internal sealed class StubRewardService : IRewardService
    {
        public readonly List<RewardItemSO> CollectedItems = new List<RewardItemSO>();
        public int ClearAllCallCount;
        public int ResetCallCount;

        public void Collect(RewardItemSO item) => CollectedItems.Add(item);

        public void ClearAll()
        {
            ClearAllCallCount++;
            CollectedItems.Clear();
        }

        public void Reset()
        {
            ResetCallCount++;
            CollectedItems.Clear();
        }

        public CollectedRewards GetCurrentRewards()
        {
            var r = new CollectedRewards();
            foreach (var item in CollectedItems)
                r.Add(item);
            return r;
        }
    }

    internal sealed class StubWheelView : IWheelView
    {
        public int LastTargetIndex = -1;
        private Action _pendingCallback;
        public bool AutoInvokeCallback = true;

        public void SetupSlices(SliceDefinition[] slices) { }

        public void SpinTo(int targetSliceIndex, Action onComplete)
        {
            LastTargetIndex = targetSliceIndex;
            _pendingCallback = onComplete;
            if (AutoInvokeCallback)
                _pendingCallback?.Invoke();
        }

        public void InvokeCallback() => _pendingCallback?.Invoke();
    }

    internal sealed class StubHudView : IHudView
    {
        public ZoneProgressModel LastZoneProgress;
        public CollectedRewards LastRewards;

        public void UpdateZoneDisplay(ZoneProgressModel progress) => LastZoneProgress = progress;
        public void UpdateRewardsDisplay(CollectedRewards rewards) => LastRewards = rewards;
    }

    internal sealed class StubDialogView : IDialogView
    {
        public bool BombScreenShown;
        public bool CollectScreenShown;
        public bool HideCallCount;
        private Action _onRevive;
        private Action _onGiveUp;
        private Action _onConfirm;
        private Action _onCancel;

        public void ShowBombScreen(Action onRevive, Action onGiveUp)
        {
            BombScreenShown = true;
            _onRevive = onRevive;
            _onGiveUp = onGiveUp;
        }

        public void ShowCollectConfirmScreen(Action onConfirm, Action onCancel)
        {
            CollectScreenShown = true;
            _onConfirm = onConfirm;
            _onCancel = onCancel;
        }

        public void Hide() => HideCallCount = true;

        public void SimulateRevive() => _onRevive?.Invoke();
        public void SimulateGiveUp() => _onGiveUp?.Invoke();
        public void SimulateConfirmCollect() => _onConfirm?.Invoke();
        public void SimulateCancelCollect() => _onCancel?.Invoke();
    }

    internal sealed class StubSpinStrategy : IWheelSpinStrategy
    {
        public int IndexToReturn;
        public int CallCount;

        public int GetWinningIndex(WheelConfigSO config)
        {
            CallCount++;
            return IndexToReturn;
        }
    }
}