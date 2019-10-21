using System;
using System.Collections.Generic;
using OpenNet.Orm.Caches;
using OpenNet.Orm.Entity;
using OpenNet.Orm.Filters;
using OpenNet.Orm.Queries;

namespace OpenNet.Orm.Interfaces
{
    public interface IDataStore : IFilterFactory, IDisposable
    {
        event EventHandler<EntityInsertArgs> AfterInsert;
        event EventHandler<EntityUpdateArgs> AfterUpdate;
        event EventHandler<EntityDeleteArgs> AfterDelete;

        EntityInfoCollection Entities { get; }
        IEntityCache Cache { get; set; }
        ISqlFactory SqlFactory { get; }

        /// <summary>
        /// Add generic type to managed entities type in current datastore
        /// None check in physical datastore is do
        /// </summary>
        void AddType<T>();

        /// <summary>
        /// Add generic type to managed entities type in current datastore
        /// this method ensure that datastore is ready to manage entity type.
        /// If not all needed operation are made.
        /// </summary>
        void AddTypeSafe<T>();

        /// <summary>
        /// Get entity info for specified entity object
        /// </summary>
        /// <param name="entity">Entity to analyse</param>
        /// <returns>Associated Entity Info</returns>
        IEntityInfo GetEntityInfo(IEntity entity);

        /// <summary>
        /// Get entity info for specified entity object type
        /// </summary>
        /// <param name="entityType">Entity type to analyse</param>
        /// <returns>Associated Entity Info</returns>
        IEntityInfo GetEntityInfo(Type entityType);

        bool StoreExists { get; }
        void CreateStore();
        void DeleteStore();
        void EnsureCompatibility();
        void CloseConnections();

        T Select<T>(object primaryKey) where T : new();
        IJoinable<TEntity> Select<TEntity>() where TEntity : class;
        IJoinable<TIEntity> Select<TEntity, TIEntity>() where TEntity : TIEntity where TIEntity : class;
        IJoinable<object> Select(Type objectType);

        void Insert(object item);
        void Update(object item);
        void Delete(object item);

        /// <summary>
        /// Delete specified entity list in datastore by Id
        /// </summary>
        /// <param name="entities">Entity list to delete</param>
        void DeleteBulk<TEntity, TIEntity>(List<TIEntity> entities) where TIEntity : class, IDistinctableEntity where TEntity : class;

        IEnumerable<TIEntity> ExecuteQuery<TIEntity>(IClause sqlClause, IEntityBuilder<TIEntity> select) where TIEntity : class;
        IEnumerable<KeyValuePair<object[], long>> ExecuteQuery(IClause sqlAggregateClause);
        int ExecuteNonQuery(IClause sqlClause);
        int ExecuteScalar(IClause sqlClause);
    }
}
