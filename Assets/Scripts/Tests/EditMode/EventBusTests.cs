using System;
using NUnit.Framework;
using WheelOfFortune.Events;
using WheelOfFortune.Domain;

namespace WheelOfFortune.Tests.EditMode
{
    [TestFixture]
    public class EventBusTests
    {
        private EventBus _eventBus;

        [SetUp]
        public void SetUp()
        {
            _eventBus = new EventBus();
        }

        [Test]
        public void Publish_WithNoSubscribers_DoesNotThrow()
        {
            Assert.DoesNotThrow(() => _eventBus.Publish(new OnBombHit()));
        }

        [Test]
        public void Subscribe_ThenPublish_HandlerIsInvoked()
        {
            var received = false;
            _eventBus.Subscribe<OnBombHit>(_ => received = true);

            _eventBus.Publish(new OnBombHit());

            Assert.IsTrue(received);
        }

        [Test]
        public void Subscribe_MultipleHandlers_AllAreInvoked()
        {
            var count = 0;
            _eventBus.Subscribe<OnBombHit>(_ => count++);
            _eventBus.Subscribe<OnBombHit>(_ => count++);
            _eventBus.Subscribe<OnBombHit>(_ => count++);

            _eventBus.Publish(new OnBombHit());

            Assert.AreEqual(3, count);
        }

        [Test]
        public void Publish_PayloadIsDeliveredCorrectly()
        {
            ZoneProgressModel received = null;
            var expected = new ZoneProgressModel(5, ZoneType.Safe);

            _eventBus.Subscribe<OnZoneAdvanced>(e => received = e.Progress);
            _eventBus.Publish(new OnZoneAdvanced(expected));

            Assert.IsNotNull(received);
            Assert.AreEqual(5, received.ZoneNumber);
            Assert.AreEqual(ZoneType.Safe, received.ZoneType);
        }

        [Test]
        public void Unsubscribe_HandlerIsNoLongerInvoked()
        {
            var count = 0;
            Action<OnBombHit> handler = _ => count++;

            _eventBus.Subscribe(handler);
            _eventBus.Publish(new OnBombHit());
            _eventBus.Unsubscribe(handler);
            _eventBus.Publish(new OnBombHit());

            Assert.AreEqual(1, count);
        }

        [Test]
        public void Unsubscribe_OneOfManyHandlers_OthersStillInvoked()
        {
            var countA = 0;
            var countB = 0;
            Action<OnBombHit> handlerA = _ => countA++;
            Action<OnBombHit> handlerB = _ => countB++;

            _eventBus.Subscribe(handlerA);
            _eventBus.Subscribe(handlerB);
            _eventBus.Unsubscribe(handlerA);
            _eventBus.Publish(new OnBombHit());

            Assert.AreEqual(0, countA);
            Assert.AreEqual(1, countB);
        }

        [Test]
        public void Unsubscribe_HandlerNotSubscribed_DoesNotThrow()
        {
            Action<OnBombHit> handler = _ => { };
            Assert.DoesNotThrow(() => _eventBus.Unsubscribe(handler));
        }

        [Test]
        public void Subscribe_DifferentEventTypes_DoNotCrossfire()
        {
            var bombCount = 0;
            var zoneCount = 0;

            _eventBus.Subscribe<OnBombHit>(_ => bombCount++);
            _eventBus.Subscribe<OnZoneAdvanced>(_ => zoneCount++);

            _eventBus.Publish(new OnBombHit());
            _eventBus.Publish(new OnBombHit());
            _eventBus.Publish(new OnZoneAdvanced(new ZoneProgressModel(1, ZoneType.Normal)));

            Assert.AreEqual(2, bombCount);
            Assert.AreEqual(1, zoneCount);
        }

        [Test]
        public void Publish_SpinCompletedWithBombResult_PayloadIntact()
        {
            SpinResult received = default;
            var expected = new SpinResult(null, 0, true, 3);

            _eventBus.Subscribe<OnSpinCompleted>(e => received = e.Result);
            _eventBus.Publish(new OnSpinCompleted(expected));

            Assert.IsTrue(received.IsBomb);
            Assert.AreEqual(3, received.SliceIndex);
        }

        [Test]
        public void Publish_RewardCollected_SnapshotDelivered()
        {
            CollectedRewards received = null;
            var rewards = new CollectedRewards();

            _eventBus.Subscribe<OnRewardCollected>(e => received = e.Snapshot);
            _eventBus.Publish(new OnRewardCollected(rewards));

            Assert.IsNotNull(received);
            Assert.AreSame(rewards, received);
        }

        [Test]
        public void Unsubscribe_LastHandler_PublishStillSafe()
        {
            Action<OnBombHit> handler = _ => { };
            _eventBus.Subscribe(handler);
            _eventBus.Unsubscribe(handler);

            Assert.DoesNotThrow(() => _eventBus.Publish(new OnBombHit()));
        }

        [Test]
        public void Subscribe_SameHandlerTwice_InvokedTwice()
        {
            var count = 0;
            Action<OnBombHit> handler = _ => count++;

            _eventBus.Subscribe(handler);
            _eventBus.Subscribe(handler);
            _eventBus.Publish(new OnBombHit());

            Assert.AreEqual(2, count);
        }
    }
}
