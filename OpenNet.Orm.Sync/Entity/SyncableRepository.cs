using System;
using System.Collections.Generic;
using System.Linq;
using OpenNet.Orm.Entity;
using OpenNet.Orm.Interfaces;
using OpenNet.Orm.Repositories;

// ReSharper disable AutoPropertyCanBeMadeGetOnly.Local
// ReSharper disable MergeConditionalExpression

namespace OpenNet.Orm.Sync.Entity
{
    public class SyncableRepository<TEntity, TIEntity> : ISyncableRepository<TIEntity>
        where TEntity : SyncableEntity<TIEntity>, TIEntity, new()
        where TIEntity : class, IDistinctableEntity
    {
        public IRepository<TIEntity> Repository { get; set; }

        public IDateTimeManager DateTimeProvider { get; set; }

        public SyncableRepository() { }

        // ReSharper disable once MemberCanBePrivate.Global
        public SyncableRepository(IRepository<TIEntity> repository, IDateTimeManager dateTimeProvider)
        {
            Repository = repository;
            DateTimeProvider = dateTimeProvider;
        }

        /// <summary>
        /// Get Repository user to Inhumed Entity
        /// </summary>
        public IRepository<IEntityTombstone> TombstoneRepository { get; private set; }

        /// <summary>
        /// Get associated datastore
        /// </summary>
        public IDataStore DataStore
        {
            get { return Repository.DataStore; }
            set { Repository.DataStore = value; }
        }

        /// <summary>
        /// Indicate number of entities existing in datastore
        /// </summary>
        /// <returns>Entities's Count</returns>
        public int Count()
        {
            return Repository.Count();
        }

        /// <summary>
        /// Save specified entity in repository
        /// </summary>
        /// <param name="entity">Entity to save</param>
        public void Save(TIEntity entity)
        {
            var syncEntity = entity as SyncableEntity<TIEntity>;
            if (syncEntity == null)
                return;

            if (entity.Id == EntityBase<TIEntity>.NullId)
                syncEntity.CreatedAt = DateTimeProvider.UtcNow;
            else if (!syncEntity.IsTombstone)
                syncEntity.UpdatedAt = DateTimeProvider.UtcNow;

            Repository.Save(entity);
        }

        /// <summary>
        /// Delete specified entity in repository
        /// </summary>
        /// <param name="entity">Entity to delete</param>
        public void Delete(TIEntity entity)
        {
            InhumedEntity(entity);
            Repository.Delete(entity);
        }

        /// <summary>
        /// Delete specified entity list in repository
        /// </summary>
        /// <param name="entities">Entity list to delete</param>
        public void Delete(List<TIEntity> entities)
        {
            InhumedEntities(entities);
            Repository.Delete(entities);
        }

        /// <summary>
        /// Delete entities by bundle
        /// </summary>
        /// <param name="entities">all entities to delete</param>
        /// <param name="bundleSize">bundle size of each entities deleted each delete request</param>
        /// <param name="observer">observer to report progression</param>
        public void DeleteByBundle(List<TIEntity> entities, int bundleSize, IOrmObserver observer)
        {
            if (entities == null || !entities.Any())
                return;

            for (var i = 0 ; i < entities.Count ; i += bundleSize)
            {
                var progress = Convert.ToInt32((double)i / entities.Count * 100);
                observer.ReportProgess(progress);
                var bundleToDelete = entities.Skip(i).Take(bundleSize).ToList();
                Delete(bundleToDelete);
            }
            observer.ReportProgess(100);
        }

        private void InhumedEntities(List<TIEntity> entities)
        {
            foreach (var entity in entities)
            {
                InhumedEntity(entity);
            }
        }

        /// <summary>
        /// Search TEntity with specified id in repository
        /// </summary>
        /// <param name="id">Id to search</param>
        /// <returns>Entity if found, else null</returns>
        public TIEntity GetById(long id)
        {
            return Repository.GetById(id);
        }

        /// <summary>
        /// Return count of all TEntity linked to specified foreign key
        /// </summary>
        /// <typeparam name="TForeignEntity">Type of foreign entity</typeparam>
        /// <param name="id">Foreign key value</param>
        /// <returns>Count of all entity linked</returns>
        public long CountAllReference<TForeignEntity>(long id)
        {
            return Repository.CountAllReference<TForeignEntity>(id);
        }

        /// <summary>
        /// Change datasource for current repository
        /// </summary>
        /// <param name="newDataStore">New Data store to take in source</param>
        public void ChangeTarget(IDataStore newDataStore)
        {
            Repository.ChangeTarget(newDataStore);
        }

        /// <summary>
        /// Get all TEntity linked to specified foreign key
        /// </summary>
        /// <typeparam name="TForeignEntity">Type of foreign entity</typeparam>
        /// <param name="id">Foreign key value</param>
        /// <returns>A collection with all entity linked</returns>
        public List<TIEntity> GetAllReference<TForeignEntity>(long id)
        {
            return Repository.GetAllReference<TForeignEntity>(id);
        }

        /// <summary>
        /// Search all entity in database
        /// </summary>
        /// <returns>All Entity found or empty list</returns>
        public List<TIEntity> GetAll()
        {
            return Repository.GetAll();
        }

        /// <summary>
        /// Create a new instance of Syncable repository
        /// </summary>
        /// <param name="dataStore">Datastore where data is located</param>
        /// <param name="dateTimeProvider">Instance to get system time</param>
        /// <returns>Syncable repo</returns>
        public static SyncableRepository<TEntity, TIEntity> Create(IDataStore dataStore, IDateTimeManager dateTimeProvider)
        {
            return Create(new Repository<TEntity, TIEntity>(dataStore), dateTimeProvider);
        }

        /// <summary>
        /// Decorate specified repo to make it syncable
        /// </summary>
        /// <param name="repo">Repository to decorate</param>
        /// <param name="dateTimeProvider">Instance to get system time</param>
        /// <returns>Decorated repo</returns>
        public static SyncableRepository<TEntity, TIEntity> Create(IRepository<TIEntity> repo, IDateTimeManager dateTimeProvider)
        {
            var result = new SyncableRepository<TEntity, TIEntity>(repo, dateTimeProvider);

            var tombstoneType = GetEntityTombstoneType(repo.DataStore);
            if (tombstoneType == null)
                return result;

            result.TombstoneRepository = CreateTombstoneRepository(repo, dateTimeProvider, tombstoneType);
            return result;
        }

        private static ISyncableRepository<IEntityTombstone> CreateTombstoneRepository(IRepository<TIEntity> repo, IDateTimeManager dateTimeProvider,
            Type tombstoneType)
        {
            var tombstoneRepository = (ISyncableRepository<IEntityTombstone>) Create(tombstoneType, typeof (IEntityTombstone));
            tombstoneRepository.Repository =
                (IRepository<IEntityTombstone>)
                    Repository<TEntity, TIEntity>.Create(tombstoneType, typeof (IEntityTombstone), repo.DataStore);
            tombstoneRepository.DateTimeProvider = dateTimeProvider;
            return tombstoneRepository;
        }

        /// <summary>
        /// Create a new SyncableRepository with specified generique arg
        /// </summary>
        /// <param name="entityType">Type of entity</param>
        /// <param name="entityTypeInterface">Type of entity abstraction</param>
        /// <returns>A new instance of SyncableRepository</returns>
        private static object Create(Type entityType, Type entityTypeInterface)
        {
            var syncTombstoneRepoType = typeof(SyncableRepository<,>).MakeGenericType(entityType, entityTypeInterface);
            return Activator.CreateInstance(syncTombstoneRepoType);
        }

        private static Type GetEntityTombstoneType(IDataStore dataStore)
        {
            var entityInfo = dataStore.GetEntityInfo(typeof(TEntity));
            var syncEntity = SyncEntity.Create(entityInfo);

            return syncEntity == null ? null : syncEntity.EntityTombstoneType;
        }

        private void InhumedEntity(TIEntity entity)
        {
            var tombstoneType = GetEntityTombstoneType(DataStore);
            var tombstone = (EntityTombstone<TEntity, TIEntity>)Activator.CreateInstance(tombstoneType);
            var deathTime = DateTimeProvider.UtcNow;
            tombstone.Repository = TombstoneRepository;
            tombstone.Id = entity.Id;
            tombstone.CreatedAt = deathTime;
            tombstone.DeletedAt = deathTime;
            tombstone.Save();
        }
    }

    public interface ISyncableRepository<TIEntity> : IRepository<TIEntity>
    {
        IRepository<TIEntity> Repository { get; set; }
        IDateTimeManager DateTimeProvider { get; set; }
    }
}