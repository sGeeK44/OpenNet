using System;
using System.Collections.Generic;
using NUnit.Framework;
using OpenNet.Orm.Entity;
using OpenNet.Orm.Repositories;
using OpenNet.Orm.Sql;
using OpenNet.Orm.SqlCe;
using OpenNet.Orm.Sync;
using OpenNet.Orm.Sync.Entity;
using OpenNet.Orm.Testkit.Entities;

namespace OpenNet.Orm.Testkit
{
    public class SyncableStore : DatastoreForTest
    {
        private readonly string _name;
        private readonly Dictionary<Type, object> _repositoryList = new Dictionary<Type, object>();

        public SyncableStore(IDateTimeManager timeManager, LocalBoundTransport transport, string name)
        {
            _name = name;
            TimeManager = timeManager;
            Transport = transport;
        }

        public IDateTimeManager TimeManager { get; set; }

        public LocalBoundTransport Transport { get; set; }

        protected override ISqlDataStore CreateStore(string datasource)
        {
            return new SyncDatastore(new SqlCeDbEngine(datasource, null), new SqlCeFactory());
        }

        public override string GetDbName()
        {
            return TestContext.CurrentContext.Test.MethodName + _name + ".sdf";
        }

        protected override void AddTypes()
        {
            DataStore.AddType<SyncSessionInfo>();
            DataStore.AddType<EntitySync>();
            DataStore.AddType<UploadOnlyEntitySync>();
            DataStore.AddType<EntityRelated>();
        }

        public void AddRepository<TEntity, TIEntity>()
            where TEntity : SyncableEntity<TIEntity>, TIEntity, new()
            where TIEntity : class, IDistinctableEntity
        {
            var repo = CreateRepository<TEntity, TIEntity>(this);
            _repositoryList.Add(typeof(TIEntity), repo);
        }

        public SyncableRepository<TEntity, TIEntity> Repository<TEntity, TIEntity>()
            where TEntity : SyncableEntity<TIEntity>, TIEntity, new()
            where TIEntity : class, IDistinctableEntity
        {
            return _repositoryList[typeof(TIEntity)] as SyncableRepository<TEntity, TIEntity>;
        }

        public TEntity InsertNewEntity<TEntity, TIEntity>()
            where TEntity : SyncableEntity<TIEntity>, TIEntity, new()
            where TIEntity : class, IDistinctableEntity
        {
            var entity = new TEntity();
            Repository<TEntity, TIEntity>().Save(entity);
            return entity;
        }

        public void Save<TEntity, TIEntity>(TIEntity entity)
            where TEntity : SyncableEntity<TIEntity>, TIEntity, new()
            where TIEntity : class, IDistinctableEntity
        {
            Repository<TEntity, TIEntity>().Save(entity);
        }

        public void Delete<TEntity, TIEntity>(TIEntity entity)
            where TEntity : SyncableEntity<TIEntity>, TIEntity, new()
            where TIEntity : class, IDistinctableEntity
        {
            Repository<TEntity, TIEntity>().Delete(entity);
        }

        public TIEntity GetById<TEntity, TIEntity>(long id)
            where TEntity : SyncableEntity<TIEntity>, TIEntity, new()
            where TIEntity : class, IDistinctableEntity
        {
            return Repository<TEntity, TIEntity>().GetById(id);
        }

        public int Count<T>() where T : class
        {
            return DataStore.Select<T>().Count();
        }

        public void Dispose()
        {
            DataStore.Dispose();
        }

        protected override void OnNewDatastore()
        {
            base.OnNewDatastore();
            foreach (var repo in _repositoryList)
            {
                ((IRepository) repo.Value).ChangeTarget(DataStore);
            }
        }

        private static SyncableRepository<T, TI> CreateRepository<T, TI>(SyncableStore syncableStore)
            where T : SyncableEntity<TI>, TI, new()
            where TI : class, IDistinctableEntity
        {
            var dataStore = syncableStore.DataStore;
            dataStore.AddTypeSafe<T>();
            return SyncableRepository<T, TI>.Create(dataStore, syncableStore.TimeManager);
        }
    }
}