using System;
using OpenNet.Orm.Sync;

namespace OpenNet.Orm.Testkit
{
    public interface IDateTimeSynchronizer
    {
        IDateTimeManager DateTimeManager { get; }
        void SetNewDate(DateTime dateTime);
    }
}