using System.Linq;
using NUnit.Framework;
using OpenNet.Orm.Sync.Entity;

// ReSharper disable UseStringInterpolation
// ReSharper disable RedundantStringFormatCall

namespace OpenNet.Orm.Sync.UnitTests
{
    [TestFixture]
    public class SqlCeDataStoreTest : SyncOneRemoteTest
    {
        [Test]
        public void ValidSyncAttribute()
        {
            var entityInfo = Desktop.DataStore.Entities.ElementAt(1);
            var syncableEntity = SyncEntity.Create(entityInfo);

            Assert.AreEqual("entity", syncableEntity.GetNameInStore());
            Assert.AreEqual("created_at", syncableEntity.CreationTrackingColumn);
            Assert.AreEqual("updated_at", syncableEntity.UpdateTrackingColumn);
            Assert.AreEqual("last_sync_at", syncableEntity.LastSyncTrackingColumn);
            Assert.AreEqual("deleted_at", syncableEntity.EntityTombstoneInfo.DeletionTrackingColumn);
            Assert.AreEqual("entity_tombstone", syncableEntity.EntityTombstoneInfo.GetNameInStore());
        }
    }
}
