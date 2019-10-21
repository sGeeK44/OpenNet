using System.Linq;
using NUnit.Framework;
using OpenNet.Orm.Entity;
using OpenNet.Orm.Interfaces;
using OpenNet.Orm.Repositories;
using OpenNet.Orm.Sync.Entity;
using OpenNet.Orm.Testkit;
using OpenNet.Orm.Testkit.Entities;

namespace OpenNet.Orm.Sync.UnitTests
{
    public class SyncOneRemoteTest
    {
        private const int FirstRemoteId = 0;
        protected Synchronizator SyncManager;

        protected IDateTimeManager SyncDateTimeProvider { get { return SyncManager.DateTimeManager; } }
        protected SyncableServer Desktop { get { return SyncManager.Desktop; } }
        protected SyncableClient FirstRemote { get { return Remote(FirstRemoteId); } }

        public SyncableClient Remote(int number)
        {
            return SyncManager.GetRemote(number);
        }

        public virtual void AddEntityType<TEntity, TIEntity>()
            where TEntity : SyncableEntity<TIEntity>, TIEntity, new()
            where TIEntity : class, IDistinctableEntity
        {
            Desktop.AddRepository<TEntity, TIEntity>();
            FirstRemote.AddRepository<TEntity, TIEntity>();
        }
        
        [SetUp]
        protected virtual void Setup()
        {
            SyncManager = new Synchronizator(new SyncableActorsFactory());
            SyncManager.AddRemote();
        }

        [TearDown]
        public void Clean()
        {
            SyncManager.Clean();
        }

        protected void SyncRemote()
        {
            SyncManager.SyncRemote(FirstRemoteId);
        }

        protected void AssertSyncRepository<T>(IRepository<T> localRepo, IRepository<T> remoteRepo)
        {
            var desktop = localRepo.GetAll().Cast<IDistinctableEntity>().OrderBy(_ => _.Id).Cast<IEntity>().ToList();
            var remote = remoteRepo.GetAll().Cast<IDistinctableEntity>().OrderBy(_ => _.Id).Cast<IEntity>().ToList();

            Assert.AreEqual(desktop.Count, remote.Count);
            for (var i = 0; i < desktop.Count; i++)
            {
                AssertDbFieldAreEqual(desktop[i], remote[i]);
            }
        }

        protected static void AssertDbFieldAreEqual(IEntity expected, IEntity result)
        {
            for (var i = 0; i < expected.DbField.Count; i++)
            {
                var property = expected.DbField[i];
                if (property.Name == "LastSyncAt")
                    continue;

                var expectedValue = property.GetValue(expected, null);
                var resultValue = property.GetValue(result, null);

                Assert.AreEqual(expectedValue, resultValue);
            }
        }

        protected void AssertFirstRemoteIsSync()
        {
            AssertIsSync(Desktop, FirstRemote);
        }

        protected void AssertIsSync(SyncableStore first, SyncableStore second)
        {
            AssertSyncRepository(
                first.Repository<EntitySync, IEntitySync>(),
                second.Repository<EntitySync, IEntitySync>()
            );
        }

        protected void SomeTimeLater()
        {
            SyncManager.SomeTimeLater();
        }

        protected void SomeDayLater(int days)
        {
            SyncManager.SomeDayLater(days);
        }
    }
}