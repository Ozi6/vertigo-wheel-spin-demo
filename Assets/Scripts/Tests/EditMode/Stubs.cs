using System;
using System.Collections.Generic;
using UnityEngine;
using WheelOfFortune.Commands;
using WheelOfFortune.Data;
using WheelOfFortune.Domain;
using WheelOfFortune.Factory;
using WheelOfFortune.Interfaces;
using WheelOfFortune.Views;
using WheelOfFortune.Events;

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

        public SpinResult Spin(RuntimeWheelData wheelData)
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
        public readonly List<RewardData> CollectedItems = new List<RewardData>();
        public int ClearAllCallCount;
        public int ResetCallCount;

        public void Collect(RewardData item) => Collect(item, 1);

        public void Collect(RewardData item, int multiplier)
            => CollectedItems.Add(item);

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
                r.Add(item, 5);
            return r;
        }
    }

    internal sealed class StubWheelView : IWheelView
    {
        public int LastTargetIndex = -1;
        public Sprite LastWheelSprite;
        public Sprite LastArrowSprite;
        private Action _pendingCallback;
        public bool AutoInvokeCallback = true;

        public WinEffectPayload LastWinEffectPayload;
        public int PlayWinEffectCallCount;
        public int RotateToOriginCallCount;
        public float LastRotateToOriginDuration;
        public WheelSlice[] LiveSlices;

        public void SetupSlices(SliceDefinition[] slices) { }

        public void SetZoneVisuals(Sprite wheelSprite, Sprite arrowSprite)
        {
            LastWheelSprite = wheelSprite;
            LastArrowSprite = arrowSprite;
        }

        private IEventBus _eventBus;
        public void Initialize(IEventBus eventBus) => _eventBus = eventBus;

        public void SpinTo(int targetSliceIndex)
        {
            LastTargetIndex = targetSliceIndex;
            if (AutoInvokeCallback)
                _eventBus?.Publish(new WheelOfFortune.Events.OnSpinAnimationComplete());
        }

        public void InvokeCallback() => _eventBus?.Publish(new WheelOfFortune.Events.OnSpinAnimationComplete());

        public void RotateToOrigin(float duration)
        {
            RotateToOriginCallCount++;
            LastRotateToOriginDuration = duration;
        }

        public void SetLiveSlices(WheelSlice[] slices)
        {
            LiveSlices = slices;
        }

        public void PlayWinEffect(WinEffectPayload payload)
        {
            PlayWinEffectCallCount++;
            LastWinEffectPayload = payload;
        }
    }

    internal sealed class StubHudView : IHudView
    {
        public ZoneProgressModel LastZoneProgress;
        public CollectedRewards LastRewards;
        public int LastCurrencyBalance;
        public string LastInitializedNewRewardCardId;

        public void UpdateZoneDisplay(ZoneProgressModel progress) => LastZoneProgress = progress;
        public void UpdateRewardsDisplay(CollectedRewards rewards) => LastRewards = rewards;
        public void UpdateCurrencyDisplay(int balance) => LastCurrencyBalance = balance;

        public Transform GetRewardsPanelTarget() => null;

        public void InitializeNewRewardCard(CollectedRewards rewards, string newItemId)
        {
            LastInitializedNewRewardCardId = newItemId;
        }

        public void Initialize(IEventBus eventBus, IRewardRegistry registry) { }
    }

    internal sealed class StubDialogView : IDialogView
    {
        public bool BombScreenShown;
        public CollectedRewards LastLostRewards;
        public bool CollectScreenShown;
        public bool HideCallCount;
        public CollectedRewards LastRewardsPassedToCollect;
        public int LastReviveCost;
        public bool ReviveInteractable;
        private IEventBus _eventBus;

        public void ShowBombScreen(CollectedRewards lostRewards, int currentReviveCost, bool canAfford)
        {
            BombScreenShown = true;
            LastLostRewards = lostRewards;
            LastReviveCost = currentReviveCost;
            ReviveInteractable = canAfford;
        }

        public void ShowCollectConfirmScreen(CollectedRewards rewards)
        {
            CollectScreenShown = true;
            LastRewardsPassedToCollect = rewards;
        }

        public void Hide() => HideCallCount = true;

        public void SimulateRevive() => _eventBus?.Publish(new WheelOfFortune.Events.OnReviveRequested());
        public void SimulateGiveUp() => _eventBus?.Publish(new WheelOfFortune.Events.OnGiveUpRequested());
        public void SimulateConfirmCollect() => _eventBus?.Publish(new WheelOfFortune.Events.OnCollectConfirmed());
        public void SimulateCancelCollect() => _eventBus?.Publish(new WheelOfFortune.Events.OnCollectCanceled());

        public void Initialize(IEventBus eventBus, IRewardRegistry registry)
        {
            _eventBus = eventBus;
            _eventBus.Subscribe<OnReviveCostChanged>(OnReviveCostChanged);
            _eventBus.Subscribe<OnBalanceChange>(OnBalanceChange);
        }

        private void OnReviveCostChanged(OnReviveCostChanged evt)
        {
            LastReviveCost = evt.NextCost;
        }

        private void OnBalanceChange(OnBalanceChange evt)
        {
            ReviveInteractable = evt.NewBalance >= LastReviveCost;
        }
    }

    internal sealed class StubButtonView : IButtonView
    {
        public bool SpinInteractable = true;
        public bool CollectVisible = false;

        public int SetSpinInteractableCallCount;
        public int SetCollectVisibleCallCount;

        public ICommand SpinCommand { get; private set; }
        public ICommand CollectCommand { get; private set; }
        public int SetCommandsCallCount { get; private set; }

        public void SetSpinInteractable(bool interactable)
        {
            SpinInteractable = interactable;
            SetSpinInteractableCallCount++;
        }

        public void SetCollectVisible(bool visible)
        {
            CollectVisible = visible;
            SetCollectVisibleCallCount++;
        }

        public void SetCommands(ICommand spinCommand, ICommand collectCommand)
        {
            SpinCommand = spinCommand;
            CollectCommand = collectCommand;
            SetCommandsCallCount++;
        }
    }

    internal sealed class StubSpinStrategy : IWheelSpinStrategy
    {
        public int IndexToReturn;
        public int CallCount;

        public int GetWinningIndex(RuntimeWheelData wheelData)
        {
            CallCount++;
            return IndexToReturn;
        }
    }

    internal sealed class StubWheelFactory : IWheelFactory
    {
        public RuntimeWheelData DataToReturn;
        public int BuildCallCount;
        public ZoneType LastZoneType;
        public int LastZoneNumber;

        public RuntimeWheelData BuildWheel(ZoneType zoneType, int zoneNumber, IWheelView wheelView)
        {
            BuildCallCount++;
            LastZoneType = zoneType;
            LastZoneNumber = zoneNumber;
            return DataToReturn;
        }
    }

    internal sealed class StubCurrencyService : ICurrencyService
    {
        private int _balance;
        public event System.Action<int> OnBalanceChanged;

        public StubCurrencyService(int initialBalance = 10000)
        {
            _balance = initialBalance;
            OnBalanceChanged?.Invoke(_balance);
        }

        public int GetBalance() => _balance;

        public bool TryDeduct(int amount)
        {
            if (_balance < amount) return false;
            _balance -= amount;
            OnBalanceChanged?.Invoke(_balance);
            return true;
        }

        public void Add(int amount)
        {
            _balance += amount;
            OnBalanceChanged?.Invoke(_balance);
        }

        public bool CanAfford(int amount) => _balance >= amount;
    }

    internal sealed class StubRewardRegistry : IRewardRegistry
    {
        public RewardItemSO RewardToReturn;
        public RewardItemSO GetReward(string id) => RewardToReturn;
    }
}
