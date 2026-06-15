using NUnit.Framework;
using WheelOfFortune.Domain;

namespace WheelOfFortune.Tests.EditMode
{
    [TestFixture]
    public class DomainTests
    {
        [Test]
        public void CollectedRewards_StartsEmpty()
        {
            var rewards = new CollectedRewards();
            Assert.AreEqual(0, rewards.Entries.Count);
        }

        [Test]
        public void CollectedRewards_Add_IncreasesCount()
        {
            var rewards = new CollectedRewards();
            rewards.Add(null, 1);
            rewards.Add(null, 1);

            Assert.AreEqual(2, rewards.Entries.Count);
        }

        [Test]
        public void CollectedRewards_Clear_EmptiesInventory()
        {
            var rewards = new CollectedRewards();
            rewards.Add(null, 1);
            rewards.Add(null, 1);
            rewards.Clear();

            Assert.AreEqual(0, rewards.Entries.Count);
        }

        [Test]
        public void CollectedRewards_Clone_IsIndependent()
        {
            var original = new CollectedRewards();
            original.Add(null, 1);

            var clone = original.Clone();
            clone.Add(null, 1);

            Assert.AreEqual(1, original.Entries.Count);
            Assert.AreEqual(2, clone.Entries.Count);
        }

        [Test]
        public void CollectedRewards_Clone_ContainsSameItems()
        {
            var original = new CollectedRewards();
            original.Add(null, 1);
            original.Add(null, 1);

            var clone = original.Clone();

            Assert.AreEqual(original.Entries.Count, clone.Entries.Count);
        }

        [Test]
        public void ZoneProgressModel_StoresValuesCorrectly()
        {
            var model = new ZoneProgressModel(15, ZoneType.Safe);

            Assert.AreEqual(15, model.ZoneNumber);
            Assert.AreEqual(ZoneType.Safe, model.ZoneType);
        }

        [Test]
        public void SpinResult_IsBombFlag_ReflectsCorrectly()
        {
            var bombResult = new SpinResult(null, 1, true, 2);
            var rewardResult = new SpinResult(null, 1, false, 4);

            Assert.IsTrue(bombResult.IsBomb);
            Assert.IsFalse(rewardResult.IsBomb);
        }

        [Test]
        public void SpinResult_SliceIndex_StoredCorrectly()
        {
            var result = new SpinResult(null, 1, false, 7);
            Assert.AreEqual(7, result.SliceIndex);
        }

        [TestCase(0, ZoneType.Normal)]
        [TestCase(1, ZoneType.Safe)]
        [TestCase(2, ZoneType.Super)]
        public void ZoneType_EnumValues_AreDistinct(int index, ZoneType expected)
        {
            Assert.AreEqual(expected, (ZoneType)index);
        }
    }
}