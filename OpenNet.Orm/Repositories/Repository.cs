using System;
using System.Collections.Generic;
using System.Linq;
using OpenNet.Orm.Entity;
using OpenNet.Orm.Interfaces;

// ReSharper disable AutoPropertyCanBeMadeGetOnly.Local
// ReSharper disable MergeConditionalExpression

namespace OpenNet.Orm.Repositories
{
    public class Repository<TEntity, TIEntity> : IRepository<TIEntity>
        where TEntity : EntityBase<TIEntity>, TIEntity, new()
        where TIEntity : class, IDistinctableEntity
    {
        public IDataStore DataStore { get; set; }

        public Repository() { }

        public Repository(IDataStore dataStore)
        {
            DataStore = dataStore;
        }

        /// <summary>
        /// Save specified entity from repository
        /// </summary>
        /// <param name="entity">Entity to save</param>
        public virtual void Save(TIEntity entity)
        {
            if (entity == null)
                return;

            if (entity.Id == EntityBase<TIEntity>.NullId
             || GetById(entity.Id) == null)
            {
                DataStore.Insert(entity);
            }
            else
            {
                DataStore.Update(entity);
            }
        }

        /// <summary>
        /// Delete specifie specified entity from repository
        /// </summary>
        /// <param name="entity">Entity to delete</param>
        public virtual void Delete(TIEntity entity)
        {
            if (entity == null)
                return;

            DataStore.Delete(entity);
        }

        /// <summary>
        /// Delete specified entity list in repository
        /// </summary>
        /// <param name="entities">Entity list to delete</param>
        public virtual void Delete(List<TIEntity> entities)
        {
            if (entities == null || entities.Count == 0)
                return;

            DataStore.DeleteBulk<TEntity, TIEntity>(entities);
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

            observer.ReportProgess(0);
            for (var i = 0; i < entities.Count; i += bundleSize)
            {
                var bundleToDelete = entities.Skip(i).Take(bundleSize).ToList();
                Delete(bundleToDelete);
                var progress = Convert.ToInt32((double)(i + bundleToDelete.Count) / entities.Count * 100);
                observer.ReportProgess(progress);
            }
        }

        /// <summary>
        /// Get a entity object with the id
        /// </summary>
        /// <param name="id">The id of the entity to get</param>
        /// <returns>The entity if exists in datastore, else null</returns>
        public virtual TIEntity GetById(long id)
        {
            if (id <= 0)
                return null;

            var result = DataStore.Select<TEntity>(id);
            return result == null ? null : result;
        }

        public int Count()
        {
            return DataStore.Select<TEntity>().Count();
        }

        /// <summary>
        /// Get all TEntity linked to specified foreign key
        /// </summary>
        /// <typeparam name="TForeignEntity">Type of foreign entity</typeparam>
        /// <param name="id">Foreign key value</param>
        /// <returns>A collection with all entity linked</returns>
        public virtual List<TIEntity> GetAllReference<TForeignEntity>(long id)
        {
            return new List<TIEntity>();
        }

        /// <summary>
        /// Return count of all TEntity linked to specified foreign key
        /// </summary>
        /// <typeparam name="TForeignEntity">Type of foreign entity</typeparam>
        /// <param name="id">Foreign key value</param>
        /// <returns>Count of all entity linked</returns>
        public virtual long CountAllReference<TForeignEntity>(long id)
        {
            return 0;
        }

        /// <summary>
        /// Search all entity in database
        /// </summary>
        /// <returns>All Entity found or empty list</returns>
        public virtual List<TIEntity> GetAll()
        {
            return DataStore.Select<TEntity, TIEntity>().GetValues().ToList();
        }

        /// <summary>
        /// Change datasource for current repository
        /// </summary>
        /// <param name="newDataStore">New Data store to take in source</param>
        public void ChangeTarget(IDataStore newDataStore)
        {
            DataStore = newDataStore;
        }

        /// <summary>
        /// Create a new repository with specified generique arg
        /// </summary>
        /// <param name="entityType">Type of entity</param>
        /// <param name="entityTypeInterface">Type of entity abstraction</param>
        /// <param name="datastore">Repo datastore</param>
        /// <returns>A new instance of repository</returns>
        public static object Create(Type entityType, Type entityTypeInterface, IDataStore datastore)
        {
            var repoType = typeof(Repository<,>).MakeGenericType(entityType, entityTypeInterface);
            var result = (IRepository)Activator.CreateInstance(repoType);
            result.DataStore = datastore;
            return result;
        }
    }
}