using System;
using System.Reflection;
using NUnit.Framework;
using UnityEngine;
using WheelOfFortune.Data;
using WheelOfFortune.Domain;
using WheelOfFortune.Events;
using WheelOfFortune.Interfaces;
using WheelOfFortune.Services;

namespace WheelOfFortune.Tests.EditMode
{
    [TestFixture]
    public class ServiceTests
    {
        private EventBus _eventBus;
        private GameSettingsSO _settings;

        [SetUp]
        public void SetUp()
        {
            _eventBus = new EventBus();
            _settings = ScriptableObject.CreateInstance<GameSettingsSO>();
            SetField(_settings, "_safeZoneInterval", 5);
            SetField(_settings, "_superZoneInterval", 30);
            SetField(_settings, "_startingReviveCost", 25);
        }

        [TearDown]
        public void TearDown()
        {
            UnityEngine.Object.DestroyImmediate(_settings);
        }

        private static void SetField(object target, string fieldName, object value)
        {
            target.GetType()
                  .GetField(fieldName, BindingFlags.NonPublic | BindingFlags.Instance)
                  .SetValue(target, value);
        }

        private static RuntimeWheelData MakeWheelData(RuntimeSlice[] slices, int bombSlotIndex, bool hasBomb, bool isWeighted = false)
        {
            return new RuntimeWheelData(slices, bombSlotIndex, hasBomb, isWeighted);
        }

        private static RuntimeSlice MakeSlice(RewardItemSO item, int multiplier)
        {
            var reward = item != null ? item.ToData() : default;
            var weight = item != null ? item.Weight : 1f;
            return new RuntimeSlice(reward, multiplier, item == null, weight);
        }

        private static RewardItemSO MakeReward(string id, float value)
        {
            var reward = ScriptableObject.CreateInstance<RewardItemSO>();
            SetField(reward, "_id", id);
            SetField(reward, "_value", value);
            return reward;
        }

        [Test]
        public void ZoneService_StartsAtZoneOne()
        {
            var service = new ZoneService(_settings, _eventBus);
            Assert.AreEqual(ZoneType.Normal, service.GetCurrentZoneType());
        }

        [Test]
        public void ZoneService_Zone5_IsSafe()
        {
            var service = new ZoneService(_settings, _eventBus);
            for (int i = 0; i < 4; i++) service.Advance();
            Assert.AreEqual(ZoneType.Safe, service.GetCurrentZoneType());
        }

        [Test]
        public void ZoneService_Zone30_IsSuper()
        {
            var service = new ZoneService(_settings, _eventBus);
            for (int i = 0; i < 29; i++) service.Advance();
            Assert.AreEqual(ZoneType.Super, service.GetCurrentZoneType());
        }

        [Test]
        public void ZoneService_Zone10_IsSafe_NotSuper()
        {
            var service = new ZoneService(_settings, _eventBus);
            for (int i = 0; i < 9; i++) service.Advance();
            Assert.AreEqual(ZoneType.Safe, service.GetCurrentZoneType());
        }

        [Test]
        public void ZoneService_NormalZone_CannotLeave()
        {
            var service = new ZoneService(_settings, _eventBus);
            Assert.IsFalse(service.CanPlayerLeave());
        }

        [Test]
        public void ZoneService_SafeZone_CanLeave()
        {
            var service = new ZoneService(_settings, _eventBus);
            for (int i = 0; i < 4; i++) service.Advance();
            Assert.IsTrue(service.CanPlayerLeave());
        }

        [Test]
        public void ZoneService_SuperZone_CanLeave()
        {
            var service = new ZoneService(_settings, _eventBus);
            for (int i = 0; i < 29; i++) service.Advance();
            Assert.IsTrue(service.CanPlayerLeave());
        }

        [Test]
        public void ZoneService_Advance_ReturnsCorrectModel()
        {
            var service = new ZoneService(_settings, _eventBus);
            ZoneProgressModel model = service.Advance();
            Assert.AreEqual(2, model.ZoneNumber);
            Assert.AreEqual(ZoneType.Normal, model.ZoneType);
        }

        [Test]
        public void ZoneService_Advance_PublishesOnZoneAdvanced()
        {
            var service = new ZoneService(_settings, _eventBus);
            ZoneProgressModel received = null;
            _eventBus.Subscribe<OnZoneAdvanced>(e => received = e.Progress);

            service.Advance();

            Assert.IsNotNull(received);
            Assert.AreEqual(2, received.ZoneNumber);
        }

        [Test]
        public void ZoneService_Reset_ReturnsToNormalAtZoneOne()
        {
            var service = new ZoneService(_settings, _eventBus);
            for (int i = 0; i < 10; i++) service.Advance();
            service.Reset();
            Assert.AreEqual(ZoneType.Normal, service.GetCurrentZoneType());
        }

        [Test]
        public void ZoneService_Reset_CannotLeaveAfterReset()
        {
            var service = new ZoneService(_settings, _eventBus);
            for (int i = 0; i < 4; i++) service.Advance();
            service.Reset();
            Assert.IsFalse(service.CanPlayerLeave());
        }

        [Test]
        public void ZoneService_SuperZoneIntervalAlsoMultipleOfSafe_IsSuper()
        {
            var service = new ZoneService(_settings, _eventBus);
            for (int i = 0; i < 29; i++) service.Advance();
            Assert.AreEqual(ZoneType.Super, service.GetCurrentZoneType());
        }

        [Test]
        public void ZoneService_CustomIntervals_CorrectlyCalculatesTypes()
        {
            var customSettings = ScriptableObject.CreateInstance<GameSettingsSO>();
            SetField(customSettings, "_safeZoneInterval", 3);
            SetField(customSettings, "_superZoneInterval", 6);

            var service = new ZoneService(customSettings, _eventBus);
            
            // Zone 1: Normal
            Assert.AreEqual(ZoneType.Normal, service.GetCurrentZoneType());

            // Zone 3: Safe
            service.Advance(); // 2
            service.Advance(); // 3
            Assert.AreEqual(ZoneType.Safe, service.GetCurrentZoneType());

            // Zone 6: Super
            service.Advance(); // 4
            service.Advance(); // 5
            service.Advance(); // 6
            Assert.AreEqual(ZoneType.Super, service.GetCurrentZoneType());

            UnityEngine.Object.DestroyImmediate(customSettings);
        }

        [Test]
        public void RewardService_InitialRewards_AreEmpty()
        {
            var service = new RewardService(_eventBus);
            Assert.AreEqual(0, service.GetCurrentRewards().Entries.Count);
        }

        [Test]
        public void RewardService_Collect_AddsItemToRewards()
        {
            var service = new RewardService(_eventBus);
            var item = MakeReward("gold", 100f);
            service.Collect(item.ToData(), 1);
            Assert.AreEqual(1, service.GetCurrentRewards().Entries.Count);
        }

        [Test]
        public void RewardService_Collect_StoresCorrectItem()
        {
            var service = new RewardService(_eventBus);
            var item = MakeReward("gold", 100f);
            service.Collect(item.ToData(), 1);
            Assert.AreEqual(item.ToData(), service.GetCurrentRewards().Entries[0].Item);
        }

        [Test]
        public void RewardService_Collect_PublishesOnRewardCollected()
        {
            var service = new RewardService(_eventBus);
            CollectedRewards received = null;
            _eventBus.Subscribe<OnRewardCollected>(e => received = e.Snapshot);

            service.Collect(MakeReward("gold", 100f).ToData(), 1);

            Assert.IsNotNull(received);
            Assert.AreEqual(1, received.Entries.Count);
        }

        [Test]
        public void RewardService_Collect_PublishedSnapshotIsClone()
        {
            var service = new RewardService(_eventBus);
            CollectedRewards firstSnapshot = null;

            _eventBus.Subscribe<OnRewardCollected>(e =>
            {
                if (firstSnapshot == null)
                    firstSnapshot = e.Snapshot;
            });

            service.Collect(MakeReward("gold", 100f).ToData(), 1);
            service.Collect(MakeReward("silver", 50f).ToData(), 1);

            Assert.IsNotNull(firstSnapshot);
            Assert.AreEqual(1, firstSnapshot.Entries.Count);
        }

        [Test]
        public void RewardService_ClearAll_EmptiesRewards()
        {
            var service = new RewardService(_eventBus);
            service.Collect(MakeReward("gold", 100f).ToData(), 1);
            service.Collect(MakeReward("silver", 50f).ToData(), 1);
            service.ClearAll();
            Assert.AreEqual(0, service.GetCurrentRewards().Entries.Count);
        }

        [Test]
        public void RewardService_ClearAll_PublishesOnBombHit()
        {
            var service = new RewardService(_eventBus);
            var fired = false;
            _eventBus.Subscribe<OnBombHit>(_ => fired = true);

            service.ClearAll();

            Assert.IsTrue(fired);
        }

        [Test]
        public void RewardService_MultipleCollects_AccumulateCorrectly()
        {
            var service = new RewardService(_eventBus);
            service.Collect(MakeReward("a", 10f).ToData(), 1);
            service.Collect(MakeReward("b", 20f).ToData(), 1);
            service.Collect(MakeReward("c", 30f).ToData(), 1);
            Assert.AreEqual(3, service.GetCurrentRewards().Entries.Count);
        }

        [Test]
        public void RewardService_ClearThenCollect_WorksCorrectly()
        {
            var service = new RewardService(_eventBus);
            service.Collect(MakeReward("a", 10f).ToData(), 1);
            service.ClearAll();
            service.Collect(MakeReward("b", 20f).ToData(), 1);
            Assert.AreEqual(1, service.GetCurrentRewards().Entries.Count);
        }

        [Test]
        public void CurrencyService_InitialBalance_IsCorrect()
        {
            var service = new CurrencyService(_eventBus, 500);
            Assert.AreEqual(500, service.GetBalance());
        }

        [Test]
        public void CurrencyService_CanAfford_ReturnsTrueWhenSufficient()
        {
            var service = new CurrencyService(_eventBus, 500);
            Assert.IsTrue(service.CanAfford(300));
            Assert.IsTrue(service.CanAfford(500));
            Assert.IsFalse(service.CanAfford(501));
        }

        [Test]
        public void CurrencyService_TryDeduct_DeductsAmountAndPublishesEvent()
        {
            var service = new CurrencyService(_eventBus, 500);
            int receivedBalance = -1;
            _eventBus.Subscribe<OnBalanceChange>(e => receivedBalance = e.NewBalance);

            bool success = service.TryDeduct(200);

            Assert.IsTrue(success);
            Assert.AreEqual(300, service.GetBalance());
            Assert.AreEqual(300, receivedBalance);
        }

        [Test]
        public void CurrencyService_TryDeduct_FailsWhenInsufficient()
        {
            var service = new CurrencyService(_eventBus, 100);
            bool fired = false;
            _eventBus.Subscribe<OnBalanceChange>(_ => fired = true);

            bool success = service.TryDeduct(150);

            Assert.IsFalse(success);
            Assert.AreEqual(100, service.GetBalance());
            Assert.IsFalse(fired);
        }

        [Test]
        public void CurrencyService_Add_IncreasesBalanceAndPublishesEvent()
        {
            var service = new CurrencyService(_eventBus, 100);
            int receivedBalance = -1;
            _eventBus.Subscribe<OnBalanceChange>(e => receivedBalance = e.NewBalance);

            service.Add(50);

            Assert.AreEqual(150, service.GetBalance());
            Assert.AreEqual(150, receivedBalance);
        }

        [Test]
        public void SpinService_Spin_ReturnsResultWithCorrectSliceIndex()
        {
            var strategy = new FixedIndexStrategy(2);
            var service = new SpinService(strategy, _eventBus);
            var reward = MakeReward("item", 10f);
            var wheelData = MakeWheelData(new[]
            {
                MakeSlice(reward, 1),
                MakeSlice(reward, 1),
                MakeSlice(reward, 1)
            }, -1, false);

            SpinResult result = service.Spin(wheelData);

            Assert.AreEqual(2, result.SliceIndex);
        }

        [Test]
        public void SpinService_Spin_IsBombFalse_WhenNoBombConfig()
        {
            var strategy = new FixedIndexStrategy(0);
            var service = new SpinService(strategy, _eventBus);
            var reward = MakeReward("item", 10f);
            var wheelData = MakeWheelData(new[] { MakeSlice(reward, 1) }, -1, false);

            SpinResult result = service.Spin(wheelData);

            Assert.IsFalse(result.IsBomb);
        }

        [Test]
        public void SpinService_Spin_IsBombTrue_WhenLandingOnBombSlot()
        {
            var strategy = new FixedIndexStrategy(1);
            var service = new SpinService(strategy, _eventBus);
            var reward = MakeReward("item", 10f);
            var wheelData = MakeWheelData(new[]
            {
                MakeSlice(reward, 1),
                MakeSlice(null, 0)
            }, 1, true);

            SpinResult result = service.Spin(wheelData);

            Assert.IsTrue(result.IsBomb);
        }

        [Test]
        public void SpinService_Spin_IsBombFalse_WhenNotLandingOnBombSlot()
        {
            var strategy = new FixedIndexStrategy(0);
            var service = new SpinService(strategy, _eventBus);
            var reward = MakeReward("item", 10f);
            var wheelData = MakeWheelData(new[]
            {
                MakeSlice(reward, 1),
                MakeSlice(null, 0)
            }, 1, true);

            SpinResult result = service.Spin(wheelData);

            Assert.IsFalse(result.IsBomb);
        }

        [Test]
        public void SpinService_Spin_PublishesOnSpinCompleted()
        {
            var strategy = new FixedIndexStrategy(0);
            var service = new SpinService(strategy, _eventBus);
            SpinResult received = default;
            _eventBus.Subscribe<OnSpinCompleted>(e => received = e.Result);
            var reward = MakeReward("item", 10f);
            var wheelData = MakeWheelData(new[] { MakeSlice(reward, 1) }, -1, false);

            service.Spin(wheelData);

            Assert.AreEqual(0, received.SliceIndex);
        }

        [Test]
        public void SpinService_Spin_RewardItemMatchesSlice()
        {
            var strategy = new FixedIndexStrategy(1);
            var service = new SpinService(strategy, _eventBus);
            var rewardA = MakeReward("a", 10f);
            var rewardB = MakeReward("b", 20f);
            var wheelData = MakeWheelData(new[]
            {
                MakeSlice(rewardA, 1),
                MakeSlice(rewardB, 1)
            }, -1, false);

            SpinResult result = service.Spin(wheelData);

            Assert.AreEqual(rewardB.ToData(), result.RewardItem);
        }

        [Test]
        public void SpinService_SetStrategy_ChangesSpinBehavior()
        {
            var service = new SpinService(new FixedIndexStrategy(0), _eventBus);
            var reward = MakeReward("item", 10f);
            var wheelData = MakeWheelData(new[]
            {
                MakeSlice(reward, 1),
                MakeSlice(reward, 1)
            }, -1, false);

            service.SetStrategy(new FixedIndexStrategy(1));
            SpinResult result = service.Spin(wheelData);

            Assert.AreEqual(1, result.SliceIndex);
        }

        [Test]
        public void RandomSpinStrategy_ReturnsIndexInRange()
        {
            var strategy = new RandomSpinStrategy();
            var reward = MakeReward("item", 10f);
            var wheelData = MakeWheelData(new[]
            {
                MakeSlice(reward, 1),
                MakeSlice(reward, 1),
                MakeSlice(reward, 1)
            }, -1, false);

            for (int i = 0; i < 100; i++)
            {
                int index = strategy.GetWinningIndex(wheelData);
                Assert.IsTrue(index >= 0 && index < wheelData.Slices.Length);
            }
        }

        [Test]
        public void RandomSpinStrategy_SingleSlice_AlwaysReturnsZero()
        {
            var strategy = new RandomSpinStrategy();
            var reward = MakeReward("item", 10f);
            var wheelData = MakeWheelData(new[] { MakeSlice(reward, 1) }, -1, false);

            for (int i = 0; i < 20; i++)
                Assert.AreEqual(0, strategy.GetWinningIndex(wheelData));
        }

        [Test]
        public void RandomSpinStrategy_OverManyRolls_AllIndicesHit()
        {
            var strategy = new RandomSpinStrategy();
            var reward = MakeReward("item", 10f);
            var wheelData = MakeWheelData(new[]
            {
                MakeSlice(reward, 1),
                MakeSlice(reward, 1),
                MakeSlice(reward, 1)
            }, -1, false);

            var hit = new bool[3];
            for (int i = 0; i < 500; i++)
                hit[strategy.GetWinningIndex(wheelData)] = true;

            Assert.IsTrue(hit[0] && hit[1] && hit[2]);
        }

        [Test]
        public void WeightedSpinStrategy_SingleWeightedSlice_AlwaysWins()
        {
            var strategy = new WeightedSpinStrategy();
            var reward = MakeReward("item", 10f);
            
            var slices = new[]
            {
                new RuntimeSlice(reward.ToData(), 1, false, 0f),
                new RuntimeSlice(reward.ToData(), 1, false, 100f)
            };
            var wheelData = MakeWheelData(slices, -1, false, true);

            for (int i = 0; i < 50; i++)
            {
                Assert.AreEqual(1, strategy.GetWinningIndex(wheelData));
            }
        }

        [Test]
        public void WeightedSpinStrategy_RespectsWeightsOverManyRolls()
        {
            var strategy = new WeightedSpinStrategy();
            var reward = MakeReward("item", 10f);
            
            var slices = new[]
            {
                new RuntimeSlice(reward.ToData(), 1, false, 10f),
                new RuntimeSlice(reward.ToData(), 1, false, 90f)
            };
            var wheelData = MakeWheelData(slices, -1, false, true);

            int slice0Count = 0;
            int slice1Count = 0;

            for (int i = 0; i < 500; i++)
            {
                int win = strategy.GetWinningIndex(wheelData);
                if (win == 0) slice0Count++;
                else slice1Count++;
            }

            Assert.IsTrue(slice1Count > slice0Count);
            Assert.IsTrue(slice0Count > 0);
        }

        private class FixedIndexStrategy : IWheelSpinStrategy
        {
            private readonly int _index;
            public FixedIndexStrategy(int index) => _index = index;
            public int GetWinningIndex(RuntimeWheelData wheelData) => _index;
        }
    }
}