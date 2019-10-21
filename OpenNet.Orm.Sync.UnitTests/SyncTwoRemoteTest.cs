using NUnit.Framework;
using OpenNet.Orm.Entity;
using OpenNet.Orm.Sync.Entity;
using OpenNet.Orm.Testkit;
using OpenNet.Orm.Testkit.Entities;

namespace OpenNet.Orm.Sync.UnitTests
{
    [TestFixture]
    public class SyncTwoRemoteTest : SyncOneRemoteTest
    {
        private const int SecondRemoteId = 1;
        private SyncableClient SecondRemote { get { return Remote(SecondRemoteId); } }

        [SetUp]
        protected override void Setup()
        {
            base.Setup();
            SyncManager.AddRemote();
            AddEntityType<EntitySync, IEntitySync>();
            AddEntityType<UploadOnlyEntitySync, IUploadOnlyEntitySync>();
        }

        public override void AddEntityType<TEntity, TIEntity>()
        {
            base.AddEntityType<TEntity, TIEntity>();
            SecondRemote.AddRepository<TEntity, TIEntity>();
        }

        public void Save<TEntity, TIEntity>(TIEntity entity)
            where TEntity : SyncableEntity<TIEntity>, TIEntity, new()
            where TIEntity : class, IDistinctableEntity
        {
            Desktop.Save<TEntity, TIEntity>(entity);
            SyncManager.SaveOnAllRemotes<TEntity, TIEntity>(entity);
        }

        [Test]
        public void InsertOnFirstRemote_DoesNotExitInSecondRemote_ShouldInsertInSecond()
        {
            FirstRemote.InsertNewEntity<EntitySync, IEntitySync>();

            SyncRemote();
            SyncOtherRemote();

            AssertSyncRepository(FirstRemote.Repository<EntitySync, IEntitySync>(), SecondRemote.Repository<EntitySync, IEntitySync>());
        }

        [Test]
        public void DeletedOnFirstRemote_ExitInSecondRemote_ShouldDeleteInSecond()
        {
            var entity = FirstRemote.InsertNewEntity<EntitySync, IEntitySync>();

            SyncRemote();
            SyncOtherRemote();

            FirstRemote.Delete<EntitySync, IEntitySync>(entity);

            SyncRemote();
            SyncOtherRemote();

            AssertSyncRepository(FirstRemote.Repository<EntitySync, IEntitySync>(), SecondRemote.Repository<EntitySync, IEntitySync>());
        }

        [Test]
        public void CreateAndDeleteOnFirstRemoteWithoutSync_NotExitInSecondRemote_ShouldNotExistInSecond()
        {
            var entity = FirstRemote.InsertNewEntity<EntitySync, IEntitySync>();
            FirstRemote.Delete<EntitySync, IEntitySync>(entity);

            SyncRemote();
            SyncOtherRemote();

            AssertSyncRepository(FirstRemote.Repository<EntitySync, IEntitySync>(), SecondRemote.Repository<EntitySync, IEntitySync>());
        }


        [Test]
        public void CreateOneOnFirstAndOneOnSecond_AfterRightSync_BothShouldBeExistOnTwoRemote()
        {
            var entity = FirstRemote.InsertNewEntity<EntitySync, IEntitySync>();
            entity.UniqueIdentifier = "X";
            FirstRemote.Save<EntitySync, IEntitySync>(entity);

            SyncManager.SomeTimeLater();

            var entity2 = SecondRemote.InsertNewEntity<EntitySync, IEntitySync>();
            entity2.UniqueIdentifier = "Y";
            SecondRemote.Save<EntitySync, IEntitySync>(entity2);

            SyncOnBothRemote();

            Assert.AreEqual(2, FirstRemote.Count<EntitySync>());
            Assert.AreEqual(2, SecondRemote.Count<EntitySync>());
            AssertSyncRepository(FirstRemote.Repository<EntitySync, IEntitySync>(), SecondRemote.Repository<EntitySync, IEntitySync>());
        }

        [Test]
        public void OneOnFirst_AfterRightSync_ShouldBeDeletedOnBoth()
        {
            var entity = FirstRemote.InsertNewEntity<EntitySync, IEntitySync>();
            FirstRemote.Save<EntitySync, IEntitySync>(entity);
            SyncOnBothRemote();

            FirstRemote.Delete<EntitySync, IEntitySync>(entity);

            SyncOnBothRemote();

            Assert.AreEqual(1, Desktop.Count<EntityTombstone<EntitySync, IEntitySync>>());
            Assert.AreEqual(1, FirstRemote.Count<EntityTombstone<EntitySync, IEntitySync>>());
            Assert.AreEqual(0, FirstRemote.Count<EntitySync>());
            Assert.AreEqual(1, SecondRemote.Count<EntityTombstone<EntitySync, IEntitySync>>());
            Assert.AreEqual(0, SecondRemote.Count<EntitySync>());
        }

        [Test]
        public void UpdateInSameTimeOnBothRemoteNotInSameEntity_ShouldBeAllSync()
        {
            var entity1 = Desktop.InsertNewEntity<EntitySync, IEntitySync>();
            entity1.UniqueIdentifier = "X";
            Desktop.Save<EntitySync, IEntitySync>(entity1);
            var entity2 = Desktop.InsertNewEntity<EntitySync, IEntitySync>();
            entity2.UniqueIdentifier = "Y";
            Desktop.Save<EntitySync, IEntitySync>(entity2);

            SyncRemote();
            SyncOtherRemote();
            AssertAllRepoIsSync();

            entity1.StringField = "F1E1";
            FirstRemote.Save<EntitySync, IEntitySync>(entity1);
            
            entity2.StringField = "F2E2";
            SecondRemote.Save<EntitySync, IEntitySync>(entity2);

            SyncRemote();
            AssertFirstRemoteIsSync();

            SyncOtherRemote();
            AssertSecondRemoteIsSync();

            SyncRemote();

            AssertAllRepoIsSync();
        }



        [Test]
        public void EntityCreatedBeforeLastSyncAndUpdatedBeforeSync()
        {
            var entity1 = FirstRemote.InsertNewEntity<EntitySync, IEntitySync>();
            entity1.UniqueIdentifier = "F1E1";
            FirstRemote.Save<EntitySync, IEntitySync>(entity1);

            var entity2 = SecondRemote.InsertNewEntity<EntitySync, IEntitySync>();
            entity2.UniqueIdentifier = "F2E2";
            SecondRemote.Save<EntitySync, IEntitySync>(entity2);

            SyncRemote();
            SyncOtherRemote();

            entity2 = entity2.GetOnSpecifiedDatastore(Desktop.DataStore);
            entity2.StringField = "Updated !";
            Desktop.Save<EntitySync, IEntitySync>(entity2);

            SyncRemote();
            AssertFirstRemoteIsSync();
        }

        private void AssertAllRepoIsSync()
        {
            AssertFirstRemoteIsSync();
            AssertSecondRemoteIsSync();
            AssertBothRemoteIsSync();
        }

        private void AssertBothRemoteIsSync()
        {
            AssertIsSync(FirstRemote, SecondRemote);
        }

        private void AssertSecondRemoteIsSync()
        {
            AssertIsSync(Desktop, SecondRemote);
        }

        private void SyncOnBothRemote()
        {
            SyncRemote();
            SyncOtherRemote();
            SyncRemote();
        }

        private void SyncOtherRemote()
        {
            SyncManager.SyncRemote(SecondRemoteId);
        }
    }
}