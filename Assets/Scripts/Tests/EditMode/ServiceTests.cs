using System;
using System.Reflection;
using NUnit.Framework;
using UnityEngine;
using WheelOfFortune.Data;
using WheelOfFortune.Domain;
using WheelOfFortune.Events;
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

        private static WheelConfigSO MakeConfig(SliceDefinition[] slices, int bombSlotIndex)
        {
            var config = ScriptableObject.CreateInstance<WheelConfigSO>();
            SetField(config, "_slices", slices);
            SetField(config, "_bombSlotIndex", bombSlotIndex);
            return config;
        }

        private static SliceDefinition MakeSlice(RewardItemSO item, float weight)
        {
            var slice = new SliceDefinition();
            SetField(slice, "_rewardItem", item);
            SetField(slice, "_weight", weight);
            return slice;
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
        public void RewardService_InitialRewards_AreEmpty()
        {
            var service = new RewardService(_eventBus);
            Assert.AreEqual(0, service.GetCurrentRewards().Items.Count);
        }

        [Test]
        public void RewardService_Collect_AddsItemToRewards()
        {
            var service = new RewardService(_eventBus);
            var item = MakeReward("gold", 100f);
            service.Collect(item);
            Assert.AreEqual(1, service.GetCurrentRewards().Items.Count);
        }

        [Test]
        public void RewardService_Collect_StoresCorrectItem()
        {
            var service = new RewardService(_eventBus);
            var item = MakeReward("gold", 100f);
            service.Collect(item);
            Assert.AreSame(item, service.GetCurrentRewards().Items[0]);
        }

        [Test]
        public void RewardService_Collect_PublishesOnRewardCollected()
        {
            var service = new RewardService(_eventBus);
            CollectedRewards received = null;
            _eventBus.Subscribe<OnRewardCollected>(e => received = e.Snapshot);

            service.Collect(MakeReward("gold", 100f));

            Assert.IsNotNull(received);
            Assert.AreEqual(1, received.Items.Count);
        }

        [Test]
        public void RewardService_Collect_PublishedSnapshotIsClone()
        {
            var service = new RewardService(_eventBus);
            CollectedRewards received = null;
            _eventBus.Subscribe<OnRewardCollected>(e => received = e.Snapshot);

            service.Collect(MakeReward("gold", 100f));
            service.Collect(MakeReward("silver", 50f));

            Assert.AreEqual(1, received.Items.Count);
        }

        [Test]
        public void RewardService_ClearAll_EmptiesRewards()
        {
            var service = new RewardService(_eventBus);
            service.Collect(MakeReward("gold", 100f));
            service.Collect(MakeReward("silver", 50f));
            service.ClearAll();
            Assert.AreEqual(0, service.GetCurrentRewards().Items.Count);
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
            service.Collect(MakeReward("a", 10f));
            service.Collect(MakeReward("b", 20f));
            service.Collect(MakeReward("c", 30f));
            Assert.AreEqual(3, service.GetCurrentRewards().Items.Count);
        }

        [Test]
        public void RewardService_ClearThenCollect_WorksCorrectly()
        {
            var service = new RewardService(_eventBus);
            service.Collect(MakeReward("a", 10f));
            service.ClearAll();
            service.Collect(MakeReward("b", 20f));
            Assert.AreEqual(1, service.GetCurrentRewards().Items.Count);
        }

        [Test]
        public void SpinService_Spin_ReturnsResultWithCorrectSliceIndex()
        {
            var strategy = new FixedIndexStrategy(2);
            var service = new SpinService(strategy, _eventBus);
            var reward = MakeReward("item", 10f);
            var config = MakeConfig(new[]
            {
                MakeSlice(reward, 1f),
                MakeSlice(reward, 1f),
                MakeSlice(reward, 1f)
            }, -1);

            SpinResult result = service.Spin(config);

            Assert.AreEqual(2, result.SliceIndex);
        }

        [Test]
        public void SpinService_Spin_IsBombFalse_WhenNoBombConfig()
        {
            var strategy = new FixedIndexStrategy(0);
            var service = new SpinService(strategy, _eventBus);
            var reward = MakeReward("item", 10f);
            var config = MakeConfig(new[] { MakeSlice(reward, 1f) }, -1);

            SpinResult result = service.Spin(config);

            Assert.IsFalse(result.IsBomb);
        }

        [Test]
        public void SpinService_Spin_IsBombTrue_WhenLandingOnBombSlot()
        {
            var strategy = new FixedIndexStrategy(1);
            var service = new SpinService(strategy, _eventBus);
            var reward = MakeReward("item", 10f);
            var config = MakeConfig(new[]
            {
                MakeSlice(reward, 1f),
                MakeSlice(reward, 1f)
            }, 1);

            SpinResult result = service.Spin(config);

            Assert.IsTrue(result.IsBomb);
        }

        [Test]
        public void SpinService_Spin_IsBombFalse_WhenNotLandingOnBombSlot()
        {
            var strategy = new FixedIndexStrategy(0);
            var service = new SpinService(strategy, _eventBus);
            var reward = MakeReward("item", 10f);
            var config = MakeConfig(new[]
            {
                MakeSlice(reward, 1f),
                MakeSlice(reward, 1f)
            }, 1);

            SpinResult result = service.Spin(config);

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
            var config = MakeConfig(new[] { MakeSlice(reward, 1f) }, -1);

            service.Spin(config);

            Assert.AreEqual(0, received.SliceIndex);
        }

        [Test]
        public void SpinService_Spin_RewardItemMatchesSlice()
        {
            var strategy = new FixedIndexStrategy(1);
            var service = new SpinService(strategy, _eventBus);
            var rewardA = MakeReward("a", 10f);
            var rewardB = MakeReward("b", 20f);
            var config = MakeConfig(new[]
            {
                MakeSlice(rewardA, 1f),
                MakeSlice(rewardB, 1f)
            }, -1);

            SpinResult result = service.Spin(config);

            Assert.AreSame(rewardB, result.RewardItem);
        }

        [Test]
        public void SpinService_SetStrategy_ChangesSpinBehavior()
        {
            var service = new SpinService(new FixedIndexStrategy(0), _eventBus);
            var reward = MakeReward("item", 10f);
            var config = MakeConfig(new[]
            {
                MakeSlice(reward, 1f),
                MakeSlice(reward, 1f)
            }, -1);

            service.SetStrategy(new FixedIndexStrategy(1));
            SpinResult result = service.Spin(config);

            Assert.AreEqual(1, result.SliceIndex);
        }

        [Test]
        public void RandomSpinStrategy_ReturnsIndexInRange()
        {
            var strategy = new RandomSpinStrategy();
            var reward = MakeReward("item", 10f);
            var config = MakeConfig(new[]
            {
                MakeSlice(reward, 1f),
                MakeSlice(reward, 1f),
                MakeSlice(reward, 1f)
            }, -1);

            for (int i = 0; i < 100; i++)
            {
                int index = strategy.GetWinningIndex(config);
                Assert.IsTrue(index >= 0 && index < config.Slices.Length);
            }
        }

        [Test]
        public void RandomSpinStrategy_SingleSlice_AlwaysReturnsZero()
        {
            var strategy = new RandomSpinStrategy();
            var reward = MakeReward("item", 10f);
            var config = MakeConfig(new[] { MakeSlice(reward, 1f) }, -1);

            for (int i = 0; i < 20; i++)
                Assert.AreEqual(0, strategy.GetWinningIndex(config));
        }

        [Test]
        public void RandomSpinStrategy_OverManyRolls_AllIndicesHit()
        {
            var strategy = new RandomSpinStrategy();
            var reward = MakeReward("item", 10f);
            var config = MakeConfig(new[]
            {
                MakeSlice(reward, 1f),
                MakeSlice(reward, 1f),
                MakeSlice(reward, 1f)
            }, -1);

            var hit = new bool[3];
            for (int i = 0; i < 500; i++)
                hit[strategy.GetWinningIndex(config)] = true;

            Assert.IsTrue(hit[0] && hit[1] && hit[2]);
        }

        private class FixedIndexStrategy : IWheelSpinStrategy
        {
            private readonly int _index;
            public FixedIndexStrategy(int index) => _index = index;
            public int GetWinningIndex(WheelConfigSO config) => _index;
        }
    }
}