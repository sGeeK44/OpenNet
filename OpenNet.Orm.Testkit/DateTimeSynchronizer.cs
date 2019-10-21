using System;
using Moq;
using OpenNet.Orm.Sync;

namespace OpenNet.Orm.Testkit
{
    public class DateTimeSynchronizer : IDateTimeSynchronizer
    {
        private readonly Mock<IDateTimeManager> _dateTimeMock;

        public DateTimeSynchronizer()
        {
            _dateTimeMock = new Mock<IDateTimeManager>();
        }

        public IDateTimeManager DateTimeManager { get { return _dateTimeMock.Object; } }

        public void SetNewDate(DateTime dateTime)
        {
            _dateTimeMock.Setup(_ => _.UtcNow).Returns(dateTime.ToUniversalTime);
        }
    }
}