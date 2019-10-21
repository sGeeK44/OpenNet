using OpenNet.Orm.Attributes;
using OpenNet.Orm.Sync.Entity;

namespace OpenNet.Orm.Testkit.Entities
{
    [Entity(NameInStore = TableName)]
    [SynchronizedEntity(EntityTombstoneType = typeof(EntityTombstone<DownloadOnlyEntitySync, DownloadOnlyEntitySync>), Direction = SyncDirection.DownloadOnly)]
    public class DownloadOnlyEntitySync : SyncableEntity<DownloadOnlyEntitySync>
    {
        public const string TableName = "entity_dl";

        public DownloadOnlyEntitySync()
        {
        }

        public DownloadOnlyEntitySync(ISyncable syncable)
            : base(syncable) { }
    }
}
