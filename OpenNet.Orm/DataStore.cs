using System;
using System.Collections.Generic;
using System.Linq;
using OpenNet.Orm.Caches;
using OpenNet.Orm.Entity;
using OpenNet.Orm.Entity.Fields;
using OpenNet.Orm.Filters;
using OpenNet.Orm.Interfaces;
using OpenNet.Orm.Queries;

// ReSharper disable UseNullPropagation
// ReSharper disable ConvertPropertyToExpressionBody
// ReSharper disable UseNameofExpression
// ReSharper disable AutoPropertyCanBeMadeGetOnly.Local

namespace OpenNet.Orm
{
    public abstract class DataStore : DisposableBase, IDataStore
    {
        private FilterFactory _filterFactory;

        public abstract IJoinable<object> Select(Type objectType);
        public abstract void CloseConnections();
        public abstract T Select<T>(object primaryKey) where T : new();
        public abstract IJoinable<TEntity> Select<TEntity>() where TEntity : class;
        public abstract IJoinable<TIEntity> Select<TEntity, TIEntity>() where TEntity : TIEntity where TIEntity : class;
        public abstract IEnumerable<TIEntity> ExecuteQuery<TIEntity>(IClause sqlClause, IEntityBuilder<TIEntity> select) where TIEntity : class;
        public abstract IEnumerable<KeyValuePair<object[], long>> ExecuteQuery(IClause sqlAggregateClause);
        public abstract int ExecuteNonQuery(IClause sqlClause);
        public abstract int ExecuteScalar(IClause sqlClause);

        public abstract ISqlFactory SqlFactory { get; }
        public abstract IFieldPropertyFactory FieldPropertyFactory { get; }
        public abstract IEntityCache Cache { get; set; }

        // TODO: maybe move these to another object since they're more "admin" related?
        public abstract void CreateStore();
        public abstract void DeleteStore();
        public abstract bool StoreExists { get; }
        public abstract void EnsureCompatibility();
        protected abstract void EnsureCompatibility(Type entityType);

        public event EventHandler<EntityInsertArgs> AfterInsert;
        protected abstract void OnInsert(object item);

        public event EventHandler<EntityUpdateArgs> AfterUpdate;
        protected abstract void OnUpdate(object item);

        public event EventHandler<EntityDeleteArgs> AfterDelete;
        protected abstract void OnDelete(object item);

        protected DataStore()
        {
            Entities = new EntityInfoCollection();
        }

        private IFilterFactory FilterFactory
        {
            get
            {
                if (_filterFactory != null)
                    return _filterFactory;
                return _filterFactory = new FilterFactory(this);
            }
        }

        public virtual string Name
        {
            get { return "Unamed Data Store"; }
        }

        public void Delete(object item)
        {
            var name = Entities.GetNameForType(item.GetType());
            OnDelete(item);
            OnAfterDelete(name, item);
        }

        /// <summary>
        /// Delete specified entity list in datastore by Id
        /// </summary>
        /// <param name="entities">Entity list to delete</param>
        public void DeleteBulk<TEntity, TIEntity>(List<TIEntity> entities)
            where TIEntity : class, IDistinctableEntity where TEntity : class
        {
            if (entities == null || entities.Count == 0)
                return;

            var condition = Condition<TEntity>(EntityBase<TIEntity>.IdColumnName, entities.Select(_ => _.Id).Distinct().ToList(), FilterOperator.In);
            Select<TEntity>().Where(condition).Delete();
        }

        private void OnAfterDelete(string entityName, object item)
        {
            var handler = AfterDelete;
            if (handler != null)
            {
                handler(this, new EntityDeleteArgs(entityName, item));
            }
        }

        /// <summary>
        /// Updates the backing DataStore with the values in the specified entity instance
        /// </summary>
        /// <param name="item"></param>
        /// <remarks>
        /// The instance provided must have a valid primary key value
        /// </remarks>
        public void Update(object item)
        {
            Update(item, null);
        }

        private void Update(object item, string fieldName)
        {
            var name = Entities.GetNameForType(item.GetType());
            OnUpdate(item);
            OnAfterUpdate(name, item, fieldName);
        }

        private void OnAfterUpdate(string entityName, object item, string fieldName)
        {
            var handler = AfterUpdate;
            if (handler != null)
            {
                handler(this, new EntityUpdateArgs(entityName, item, fieldName));
            }
        }

        public void Insert(object item)
        {
            if (item == null) throw new ArgumentNullException("item");

            var name = Entities.GetNameForType(item.GetType());

            OnInsert(item);
            OnAfterInsert(name, item);
        }

        private void OnAfterInsert(string entityName, object item)
        {
            var handler = AfterInsert;
            if (handler != null)
            {
                handler(this, new EntityInsertArgs(entityName, item));
            }
        }

        public EntityInfoCollection Entities { get; private set; }

        /// <summary>
        /// Add generic type to managed entities type in current datastore
        /// None check in physical datastore is do
        /// </summary>
        public void AddType<T>()
        {
            AddType(typeof(T));
        }

        protected virtual IEntityInfo AddType(Type entityType)
        {
            if (entityType == null)
                return null;

            return EntityInfo.Create(FieldPropertyFactory, Entities, entityType);
        }

        /// <summary>
        /// Add generic type to managed entities type in current datastore
        /// this method ensure that datastore is ready to manage entity type.
        /// If not all needed operation are made.
        /// </summary>
        public void AddTypeSafe<T>()
        {
            AddTypeSafe(typeof(T));
        }

        protected virtual IEntityInfo AddTypeSafe(Type entityType)
        {
            if (entityType == null)
                return null;

            var map = EntityInfo.Create(FieldPropertyFactory, Entities, entityType);
            EnsureCompatibility(entityType);
            return map;
        }

        /// <summary>
        /// Get entity info for specified entity object
        /// </summary>
        /// <param name="entity">Entity to analyse</param>
        /// <returns>Associated Entity Info</returns>
        public IEntityInfo GetEntityInfo(IEntity entity)
        {
            return GetEntityInfo(entity.GetType());
        }

        /// <summary>
        /// Get entity info for specified entity object type
        /// </summary>
        /// <param name="entityType">Entity type to analyse</param>
        /// <returns>Associated Entity Info</returns>
        public IEntityInfo GetEntityInfo(Type entityType)
        {
            return Entities[entityType];
        }

        /// <summary>
        /// Create new condition
        /// </summary>
        /// <typeparam name="TEntity">Type of entity associated to column name</typeparam>
        /// <param name="columnName">Column Name involve</param>
        /// <param name="value">Value to compare to column name</param>
        /// <param name="filterOperator">Operator to apply</param>
        /// <returns>New build condition</returns>
        public ICondition Condition<TEntity>(string columnName, object value, FilterOperator filterOperator)
        {
            return FilterFactory.Condition<TEntity>(columnName, value, filterOperator);
        }

        /// <summary>
        /// Create new condition
        /// </summary>
        /// <param name="entityType">Type of entity associated to column name</param>
        /// <param name="columnName">Column Name involve</param>
        /// <param name="value">Value to compare to column name</param>
        /// <param name="filterOperator">Operator to apply</param>
        /// <returns>New build condition</returns>
        public ICondition Condition(Type entityType, string columnName, object value, FilterOperator filterOperator)
        {
            return FilterFactory.Condition(entityType, columnName, value, filterOperator);
        }

        /// <summary>
        /// Create new condition
        /// </summary>
        /// <typeparam name="TEntity1">Type of entity associated to column name 1</typeparam>
        /// <typeparam name="TEntity2">Type of entity associated to column name 2</typeparam>
        /// <param name="columnName1">First column Name involve</param>
        /// <param name="columnName2">Second column Name to compare</param>
        /// <param name="filterOperator">Operator to apply</param>
        /// <returns>New build condition</returns>
        public ICondition Condition<TEntity1, TEntity2>(string columnName1, string columnName2, FilterOperator filterOperator)
        {
            return FilterFactory.Condition<TEntity1, TEntity2>(columnName1, columnName2, filterOperator);
        }

        /// <summary>
        /// Create new condition
        /// </summary>
        /// <param name="entityType1">Type of entity associated to column name 1</param>
        /// <param name="entityType2">Type of entity associated to column name 2</param>
        /// <param name="columnName1">First column Name involve</param>
        /// <param name="columnName2">Second column Name to compare</param>
        /// <param name="filterOperator">Operator to apply</param>
        /// <returns>New build condition</returns>
        public ICondition Condition(Type entityType1, Type entityType2, string columnName1, string columnName2, FilterOperator filterOperator)
        {
            return FilterFactory.Condition(entityType1, entityType2, columnName1, columnName2, filterOperator);
        }

        /// <summary>
        /// Create new condition
        /// </summary>
        /// <typeparam name="TEntity">Type of entity on wich condition should be applied</typeparam>
        /// <param name="columnName">Column Name involve</param>
        /// <param name="scalarValue">Scalar operation value</param>
        /// <param name="filterOperator">Operator to apply</param>
        /// <returns>New build condition</returns>
        public ICondition Condition<TEntity>(string columnName, IFilter scalarValue, FilterOperator filterOperator)
        {
            return FilterFactory.Condition<TEntity>(columnName, scalarValue, filterOperator);
        }

        /// <summary>
        /// Create new condition
        /// </summary>
        /// <param name="leftPart">First paert involve in filter</param>
        /// <param name="value">Value to compare to column name</param>
        /// <param name="filterOperator">Operator to apply</param>
        /// <returns>New build condition</returns>
        public ICondition Condition(IFilter leftPart, object value, FilterOperator filterOperator)
        {
            return FilterFactory.Condition(leftPart, value, filterOperator);
        }

        /// <summary>
        /// Create new condition
        /// </summary>
        /// <param name="leftPart">First paert involve in filter</param>
        /// <param name="value">Value to compare to column name</param>
        /// <param name="filterOperator">Operator to apply</param>
        /// <returns>New build condition</returns>
        public ICondition Condition(ColumnValue leftPart, object value, FilterOperator filterOperator)
        {
            return FilterFactory.Condition(leftPart, value, filterOperator);
        }

        /// <summary>
        /// Create a new ScalarValue wich will add specified date on specified column
        /// </summary>
        /// <typeparam name="TEntity">Type of entity on wich scalar operation should be applied</typeparam>
        /// <param name="date">Date to add on each column value</param>
        /// <param name="columnName">Column Name involve</param>
        /// <returns>New scalar operation</returns>
        public IFilter AddDay<TEntity>(DateTime date, string columnName)
        {
            return FilterFactory.AddDay<TEntity>(date, columnName);
        }

        /// <summary>
        /// Create a new ScalarValue wich will apply scalar operator on specified column
        /// </summary>
        /// <typeparam name="TEntity1">Type of entity associated to column name 1</typeparam>
        /// <typeparam name="TEntity2">Type of entity associated to column name 2</typeparam>
        /// <param name="columnName1">First column Name involve</param>
        /// <param name="columnName2">Second column Name involve</param>
        /// <param name="scalarOperator">Operator to apply between two field</param>
        /// <returns>New scalar operation</returns>
        public IFilter Scalar<TEntity1, TEntity2>(string columnName1, string columnName2, ScalarOperator scalarOperator)
        {
            return FilterFactory.Scalar<TEntity1, TEntity2>(columnName1, columnName2, scalarOperator);
        }

        /// <summary>
        /// Create a new ScalarValue wich will apply scalar operator on specified column
        /// </summary>
        /// <param name="entityType1">Type of entity associated to column name 1</param>
        /// <param name="entityType2">Type of entity associated to column name 2</param>
        /// <param name="columnName1">First column Name involve</param>
        /// <param name="columnName2">Second column Name involve</param>
        /// <param name="scalarOperator">Operator to apply between two field</param>
        /// <returns>New scalar operation</returns>
        public IFilter Scalar(Type entityType1, Type entityType2, string columnName1, string columnName2, ScalarOperator scalarOperator)
        {
            return FilterFactory.Scalar(entityType1, entityType2, columnName1, columnName2, scalarOperator);
        }

        /// <summary>
        /// Create a new Filter part for a condition in according with specified filter operator
        /// </summary>
        /// <param name="leftPart">Left part involve in condition</param>
        /// <param name="rightPart">Right part involve in condition</param>
        /// <param name="scalarOperator">Operator to apply between two field</param>
        /// <returns>Corresponding filter</returns>
        public IFilter Scalar(IFilter leftPart, IFilter rightPart, ScalarOperator scalarOperator)
        {
            return FilterFactory.Scalar(leftPart, rightPart, scalarOperator);
        }

        /// <summary>
        /// Create a new column wich will be used on a request
        /// </summary>
        /// <typeparam name="TEntity">Type of entity owner</typeparam>
        /// <param name="columnName">Column Name</param>
        /// <returns>New column value statement</returns>
        public ColumnValue GetColumn<TEntity>(string columnName)
        {
            return FilterFactory.GetColumn<TEntity>(columnName);
        }

        /// <summary>
        /// Create a new Filter part for a value
        /// </summary>
        /// <param name="value">A ref to a column value</param>
        /// <returns>Corresponding filter</returns>
        public ObjectValue ToObjectValue(object value)
        {
            return FilterFactory.ToObjectValue(value);
        }

        /// <summary>
        /// Create a new Filter part for a column ref
        /// </summary>
        /// <param name="entity">Entity which own column name</param>
        /// <param name="columnName">String name of column</param>
        /// <returns>Corresponding filter</returns>
        public ColumnValue ToColumnValue(IEntityInfo entity, string columnName)
        {
            return FilterFactory.ToColumnValue(entity, columnName);
        }
    }
}
