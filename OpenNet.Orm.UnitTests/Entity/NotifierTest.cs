using Moq;
using NUnit.Framework;
using OpenNet.Orm.Entity;

namespace OpenNet.Orm.UnitTests.Entity
{
    [TestFixture]
    public class NotifierTest
    {
        private enum EventTypes
        {
            EventType1
        }

        [Test]
        public void Notify_AddSubcriberDuringNofify_ShouldWork()
        {
            var notifier = new Notifier();
            var subscriber = new Mock<IEntityObserver>();
            subscriber.Setup(_ => _.Notify(EventTypes.EventType1, null))
                .Callback(() => notifier.Subscribe(new Mock<IEntityObserver>().Object));

            notifier.Subscribe(subscriber.Object);

            notifier.Notify(EventTypes.EventType1, null);

            subscriber.Verify(_ => _.Notify(EventTypes.EventType1, null), Times.Once());
        }

        [Test]
        public void Subscribe_SameObject_ShouldAddOnlyOnce()
        {
            var notifier = new Notifier();
            var subscriber = new Mock<IEntityObserver>();

            notifier.Subscribe(subscriber.Object);
            notifier.Subscribe(subscriber.Object);

            notifier.Notify(EventTypes.EventType1, null);

            subscriber.Verify(_ => _.Notify(EventTypes.EventType1, null), Times.Once());
        }

        [Test]
        public void Unsubscribe_SameObject_ShouldNotThrowException()
        {
            var notifier = new Notifier();
            var subscriber = new Mock<IEntityObserver>();
            notifier.Subscribe(subscriber.Object);

            notifier.Unsubscribe(subscriber.Object);
            notifier.Unsubscribe(subscriber.Object);
        }
    }
}
