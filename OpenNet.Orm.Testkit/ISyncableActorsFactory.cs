using OpenNet.Orm.Sync;

namespace OpenNet.Orm.Testkit
{
    public interface ISyncableActorsFactory
    {
        SyncableClient CreateClient(IDateTimeManager dateTimeProvider, string name);
        SyncableServer CreateServer(IDateTimeManager dateTimeProvider, string name);
    }
}