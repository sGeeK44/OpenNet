using NUnit.Framework;
using OpenNet.Orm.Testkit.Entities;

namespace OpenNet.Orm.Sync.UnitTests
{
    [TestFixture]
    public class SyncEntityTest : SyncOneRemoteTest
    {
        protected override void Setup()
        {
            base.Setup();
            AddEntityType<EntitySync, IEntitySync>();
            AddEntityType<UploadOnlyEntitySync, IUploadOnlyEntitySync>();
            AddEntityType<EntityRelated, IEntityRelated>();
        }

        [Test]
        public void Sync_InsertLocal_DoesNotExitInRemote()
        {
            Desktop.InsertNewEntity<EntitySync, IEntitySync>();

            SyncRemote();

            AssertSyncRepository(Desktop.Repository<EntitySync, IEntitySync>(), FirstRemote.Repository<EntitySync, IEntitySync>());
        }

        [Test]
        public void Sync_InsertRemote_DoesNotExitInLocal()
        {
            FirstRemote.InsertNewEntity<EntitySync, IEntitySync>();

            SyncRemote();

            AssertSyncRepository(Desktop.Repository<EntitySync, IEntitySync>(), FirstRemote.Repository<EntitySync, IEntitySync>());
        }

        [Test]
        public void Sync_InsertLocal_InsertRemote()
        {
            Desktop.InsertNewEntity<EntitySync, IEntitySync>();

            FirstRemote.InsertNewEntity<EntitySync, IEntitySync>();

            SyncRemote();

            AssertSyncRepository(Desktop.Repository<EntitySync, IEntitySync>(), FirstRemote.Repository<EntitySync, IEntitySync>());
        }

        [Test]
        public void Sync_InsertLocal_DeleteRemote()
        {
            Desktop.InsertNewEntity<EntitySync, IEntitySync>();

            var entity = FirstRemote.InsertNewEntity<EntitySync, IEntitySync>();
            FirstRemote.Delete<EntitySync, IEntitySync>(entity);

            SyncRemote();

            AssertSyncRepository(Desktop.Repository<EntitySync, IEntitySync>(), FirstRemote.Repository<EntitySync, IEntitySync>());
        }

        [Test]
        public void Sync_DeleteLocal_InsertRemote()
        {
            var entity = Desktop.InsertNewEntity<EntitySync, IEntitySync>();
            Desktop.Delete<EntitySync, IEntitySync>(entity);

            FirstRemote.InsertNewEntity<EntitySync, IEntitySync>();

            SyncRemote();

            AssertSyncRepository(Desktop.Repository<EntitySync, IEntitySync>(), FirstRemote.Repository<EntitySync, IEntitySync>());
        }

        [Test]
        public void Sync_DeleteLocal_InsertRemote_WithUploadOnlyType()
        {
            var entity1 = FirstRemote.InsertNewEntity<UploadOnlyEntitySync, IUploadOnlyEntitySync>();
            entity1.UniqueIdentifier = "F1E1";
            Desktop.Save<UploadOnlyEntitySync, IUploadOnlyEntitySync>(entity1);
            Desktop.Delete<UploadOnlyEntitySync, IUploadOnlyEntitySync>(entity1);
            FirstRemote.Save<UploadOnlyEntitySync, IUploadOnlyEntitySync>(entity1);

            SyncRemote();

            AssertFirstRemoteIsSync();
        }

        [Test]
        public void Sync_UpdateLocal_NoConflict()
        {
            var entity = Desktop.InsertNewEntity<EntitySync, IEntitySync>();

            SyncRemote();

            entity.StringField += "Updated!";
            Desktop.Repository<EntitySync, IEntitySync>().Save(entity);

            SyncRemote();

            AssertSyncRepository(Desktop.Repository<EntitySync, IEntitySync>(), FirstRemote.Repository<EntitySync, IEntitySync>());
        }

        [Test]
        public void Sync_UpdateLocal_UpdateRemote()
        {
            var entity = Desktop.InsertNewEntity<EntitySync, IEntitySync>();

            SyncRemote();

            entity.StringField += "Updated!";
            Desktop.Repository<EntitySync, IEntitySync>().Save(entity);

            entity.StringField += "Remote!";
            FirstRemote.Repository<EntitySync, IEntitySync>().Save(entity);

            SyncRemote();

            AssertSyncRepository(Desktop.Repository<EntitySync, IEntitySync>(), FirstRemote.Repository<EntitySync, IEntitySync>());
        }

        [Test]
        public void Sync_EntityCreatedDeleteOnRemote_UpdateNoGenerateConflict()
        {
            var entity = new EntitySync();
            FirstRemote.Repository<EntitySync, IEntitySync>().Save(entity);
            FirstRemote.Repository<EntitySync, IEntitySync>().Delete(entity);

            SyncRemote();

            AssertSyncRepository(Desktop.Repository<EntitySync, IEntitySync>(), FirstRemote.Repository<EntitySync, IEntitySync>());
        }

        [Test]
        public void Sync_AfterRententionTimeIsReach_DataOnRemoteShoudBeCleaned()
        {
            var entity = new EntitySync();
            FirstRemote.Repository<EntitySync, IEntitySync>().Save(entity);

            SyncRemote();

            Assert.AreEqual(1, Desktop.Repository<EntitySync, IEntitySync>().Count());
            Assert.AreEqual(1, FirstRemote.Repository<EntitySync, IEntitySync>().Count());

            SomeDayLater(EntitySync.RentionTimeClient);

            SyncRemote();

            Assert.AreEqual(1, Desktop.Repository<EntitySync, IEntitySync>().Count());
            Assert.AreEqual(0, FirstRemote.Repository<EntitySync, IEntitySync>().Count());
        }

        [Test]
        public void ConflictOnUpdatedEntityLinkedToAnInsertedEntity()
        {
            var entity = new EntitySync { UniqueIdentifier = "AA" };
            Desktop.Repository<EntitySync, IEntitySync>().Save(entity);
            var related = new EntityRelated{ RelatedId = entity.Id};
            Desktop.Repository<EntityRelated, IEntityRelated>().Save(related);

            SyncRemote();

            FirstRemote.Repository<EntityRelated, IEntityRelated>().Save(related);

            entity = new EntitySync { UniqueIdentifier = "BB" };
            Desktop.Repository<EntitySync, IEntitySync>().Save(entity);
            related.RelatedId = entity.Id;
            Desktop.Repository<EntityRelated, IEntityRelated>().Save(related);

            SyncRemote();

            AssertSyncRepository(Desktop.Repository<EntitySync, IEntitySync>(), FirstRemote.Repository<EntitySync, IEntitySync>());
        }

        [Test]
        public void TwiceInsertNotInSameOrderWithUnicityConstraint_RemoteShouldUpdatedWithDesktopIdentity()
        {
            var entityDesktopA = new EntitySync { UniqueIdentifier = "A" };
            var entityDesktopB = new EntitySync { UniqueIdentifier = "B" };
            Desktop.Repository<EntitySync, IEntitySync>().Save(entityDesktopA);
            Desktop.Repository<EntitySync, IEntitySync>().Save(entityDesktopB);

            var entityRemoteA = new EntitySync { UniqueIdentifier = "A" };
            var entityRemoteB = new EntitySync { UniqueIdentifier = "B" };
            FirstRemote.Repository<EntitySync, IEntitySync>().Save(entityRemoteB);
            FirstRemote.Repository<EntitySync, IEntitySync>().Save(entityRemoteA);

            SyncRemote();

            AssertSyncRepository(Desktop.Repository<EntitySync, IEntitySync>(), FirstRemote.Repository<EntitySync, IEntitySync>());
        }

        [Test]
        public void TwiceInsertNotInSameOrderWithUnicityConstraint_RelationShouldBeUpdated()
        {
            var entityDesktopA = new EntitySync { UniqueIdentifier = "A" };
            var entityDesktopB = new EntitySync { UniqueIdentifier = "B" };
            Desktop.Repository<EntitySync, IEntitySync>().Save(entityDesktopA);
            Desktop.Repository<EntitySync, IEntitySync>().Save(entityDesktopB);

            var entityRemoteA = new EntitySync { UniqueIdentifier = "A" };
            var entityRemoteB = new EntitySync { UniqueIdentifier = "B" };
            FirstRemote.Repository<EntitySync, IEntitySync>().Save(entityRemoteB);
            FirstRemote.Repository<EntitySync, IEntitySync>().Save(entityRemoteA);

            var entityRelatedOnA = new EntityRelated { RelatedId = entityRemoteA.Id };
            FirstRemote.Repository<EntityRelated, IEntityRelated>().Save(entityRelatedOnA);

            SyncRemote();

            AssertSyncRepository(Desktop.Repository<EntitySync, IEntitySync>(), FirstRemote.Repository<EntitySync, IEntitySync>());
        }

        [Test]
        public void Sync_EntityReferencesInFKDeletedInLocal_IsDeleteOnRemote()
        {
            var entity = new EntitySync { UniqueIdentifier = "AA" };
            Desktop.Repository<EntitySync, IEntitySync>().Save(entity);
            var related = new EntityRelated { RelatedId = entity.Id };
            Desktop.Repository<EntityRelated, IEntityRelated>().Save(related);

            SyncRemote();

            related.RelatedId = null;
            Desktop.Repository<EntityRelated, IEntityRelated>().Save(related);
            Desktop.Repository<EntitySync, IEntitySync>().Delete(entity);

            SyncRemote();

            Assert.AreEqual(Desktop.Repository<EntitySync, IEntitySync>().Count(), 0);
            AssertSyncRepository(Desktop.Repository<EntitySync, IEntitySync>(), FirstRemote.Repository<EntitySync, IEntitySync>());
        }

        [Test]
        public void DeleteEntityOnCLientAndInsertRelatedOnRemote_DeleteRelatedOnRemote()
        {
            var entity = new EntitySync { UniqueIdentifier = "AA" };
            Desktop.Repository<EntitySync, IEntitySync>().Save(entity);

            SyncRemote();

            //INsert related on remote
            var related = new EntityRelated { RelatedId = entity.Id };
            FirstRemote.Repository<EntityRelated, IEntityRelated>().Save(related);

            //Delete foreign key reference on desktop
            Desktop.Repository<EntitySync, IEntitySync>().Delete(entity);

            SyncRemote();

            AssertSyncRepository(Desktop.Repository<EntityRelated, IEntityRelated>(), FirstRemote.Repository<EntityRelated, IEntityRelated>());
            AssertSyncRepository(Desktop.Repository<EntitySync, IEntitySync>(), FirstRemote.Repository<EntitySync, IEntitySync>());
        }

        [Test]
        public void DeleteEntityOnClientAndInsertNotRelatedRemote_InsertOnRemote()
        {
            var entity = new EntitySync { UniqueIdentifier = "AA" };
            var entityToDelete = new EntitySync { UniqueIdentifier = "AB" };
            Desktop.Repository<EntitySync, IEntitySync>().Save(entity);
            Desktop.Repository<EntitySync, IEntitySync>().Save(entityToDelete);

            SyncRemote();

            //INsert related on remote
            var related = new EntityRelated { RelatedId = entity.Id };
            FirstRemote.Repository<EntityRelated, IEntityRelated>().Save(related);

            //Delete foreign key reference on desktop
            Desktop.Repository<EntitySync, IEntitySync>().Delete(entityToDelete);

            SyncRemote();

            Assert.AreEqual(Desktop.Repository<EntitySync, IEntitySync>().Count(), 1);
            Assert.AreEqual(Desktop.Repository<EntityRelated, IEntityRelated>().Count(), 1);
            AssertSyncRepository(Desktop.Repository<EntityRelated, IEntityRelated>(), FirstRemote.Repository<EntityRelated, IEntityRelated>());
            AssertSyncRepository(Desktop.Repository<EntitySync, IEntitySync>(), FirstRemote.Repository<EntitySync, IEntitySync>());
        }

        [Test]
        public void EntityCreatedBeforeUsingSyncFrameworkHasNoCreatedAtValue()
        {
            var entity = new EntityRelated();
            Desktop.DataStore.Insert(entity);
            FirstRemote.DataStore.Insert(entity);
            AssertSyncRepository(Desktop.Repository<EntityRelated, IEntityRelated>(), FirstRemote.Repository<EntityRelated, IEntityRelated>());

            entity.IntField = 2;
            Desktop.Save<EntityRelated, IEntityRelated>(entity);

            SyncRemote();

            AssertSyncRepository(Desktop.Repository<EntityRelated, IEntityRelated>(), FirstRemote.Repository<EntityRelated, IEntityRelated>());
        }

        [Test]
        public void EntityRelatedCanHaveCustomEntityTombstone()
        {
            var entity = new EntityRelated();
            var repo = Desktop.Repository<EntityRelated, IEntityRelated>();
            repo.Save(entity);

            repo.Delete(entity);

            var columnExist = Desktop.DataStore.ExecuteScalar("SELECT COUNT(*) FROM information_schema.columns WHERE [TABLE_NAME] = 'entity_related_tombstone' AND [COLUMN_NAME] = 'CustomProp'");
            Assert.AreEqual(1, columnExist);

            SyncRemote();

            columnExist = FirstRemote.DataStore.ExecuteScalar("SELECT COUNT(*) FROM information_schema.columns WHERE [TABLE_NAME] = 'entity_related_tombstone' AND [COLUMN_NAME] = 'CustomProp'");
            Assert.AreEqual(1, columnExist);

            var rometeRows = FirstRemote.DataStore.ExecuteScalar("SELECT Count(*) FROM entity_related_tombstone");
            Assert.AreEqual(1, rometeRows);
        }
    }
}