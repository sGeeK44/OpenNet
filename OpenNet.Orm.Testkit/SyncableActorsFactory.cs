using OpenNet.Orm.Sync;

namespace OpenNet.Orm.Testkit
{
    public class SyncableActorsFactory : ISyncableActorsFactory
    {
        public SyncableClient CreateClient(IDateTimeManager dateTimeProvider, string name)
        {
            return new SyncableClient(dateTimeProvider, new LocalBoundTransport(), name);
        }

        public SyncableServer CreateServer(IDateTimeManager dateTimeProvider, string name)
        {
            return new SyncableServer(dateTimeProvider, new LocalBoundTransport(), name);
        }
    }
}