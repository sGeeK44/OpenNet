using OpenNet.Orm.Attributes;
using OpenNet.Orm.Sync.Entity;

namespace OpenNet.Orm.Testkit.Entities
{
    [Entity(NameInStore = TableName)]
    [SynchronizedEntity(EntityTombstoneType = typeof(EntityTombstone<UpdloadOnlyEntitySync, UpdloadOnlyEntitySync>), Direction = SyncDirection.UploadOnly)]
    public class UpdloadOnlyEntitySync : SyncableEntity<UpdloadOnlyEntitySync>
    {
        public const string TableName = "entity_up";
        public UpdloadOnlyEntitySync()
        {
        }

        public UpdloadOnlyEntitySync(ISyncable syncable)
            : base(syncable) { }
    }
}
