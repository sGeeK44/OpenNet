using System;
using OpenNet.Orm.Interfaces;
using OpenNet.Orm.Queries;

// ReSharper disable UseStringInterpolation

namespace OpenNet.Orm.Filters
{
    /// <summary>
    /// Provide methods to build sql filter
    /// </summary>
    public class FilterFactory : IFilterFactory
    {
        private readonly ISqlFactory _sqlFactory;
        private readonly EntityInfoCollection _entities;

        public FilterFactory(IDataStore dataStore)
            : this(dataStore.SqlFactory, dataStore.Entities) { }

        public FilterFactory(ISqlFactory sqlFactory, EntityInfoCollection entities)
        {
            _sqlFactory = sqlFactory;
            _entities = entities;
        }

        /// <summary>
        /// Create a new Filter part for a column ref
        /// </summary>
        /// <param name="entity">Entity which own column name</param>
        /// <param name="columnName">String name of column</param>
        /// <returns>Corresponding filter</returns>
        public ColumnValue ToColumnValue(IEntityInfo entity, string columnName)
        {
            return new ColumnValue(entity, columnName);
        }

        /// <summary>
        /// Create a new Filter part for a value
        /// </summary>
        /// <param name="value">A ref to a column value</param>
        /// <returns>Corresponding filter</returns>
        public ObjectValue ToObjectValue(object value)
        {
            return new ObjectValue(_sqlFactory, value);
        }

        /// <summary>
        /// Create a new Filter part for an equality between two other parts
        /// </summary>
        /// <param name="leftPart">Column involve in equality condition</param>
        /// <param name="rightPart">Other column part involve in equality condition</param>
        /// <returns>Corresponding filter</returns>
        public ICondition Equal(IFilter leftPart, IFilter rightPart)
        {
            return new EqualCondition(this, leftPart, rightPart);
        }

        /// <summary>
        /// Create a new Filter part for an equality between two other parts
        /// </summary>
        /// <param name="leftPart">Left part involve in equality condition</param>
        /// <param name="rightPart">Right part involve in equality condition</param>
        /// <returns>Corresponding filter</returns>
        public ICondition Equal(ColumnValue leftPart, ObjectValue rightPart)
        {
            return new EqualCondition(this, leftPart, rightPart);
        }

        /// <summary>
        /// Create a new Filter part for an non equality between two other parts
        /// </summary>
        /// <param name="leftPart">Column involve involve in non equality condition</param>
        /// <param name="rightPart">Other column involve in non equality condition</param>
        /// <returns>Corresponding filter</returns>
        public ICondition NotEqual(IFilter leftPart, IFilter rightPart)
        {
            return new NotEqualCondition(this, leftPart, rightPart);
        }

        /// <summary>
        /// Create a new Filter part for an non equality between two other parts
        /// </summary>
        /// <param name="leftPart">Column involve involve in non equality condition</param>
        /// <param name="rightPart">Object value involve in non equality condition</param>
        /// <returns>Corresponding filter</returns>
        public ICondition NotEqual(ColumnValue leftPart, ObjectValue rightPart)
        {
            return new NotEqualCondition(this, leftPart, rightPart);
        }

        /// <summary>
        /// Create a new Filter part for an partial equality between two other parts
        /// </summary>
        /// <param name="leftPart">Column involve involve in partial condition</param>
        /// <param name="rightPart">Other column involve in partial condition</param>
        /// <returns>Corresponding filter</returns>
        public ICondition Like(IFilter leftPart, IFilter rightPart)
        {
            return new LikeCondition(this, leftPart, rightPart);
        }

        /// <summary>
        /// Create a new Filter part for an partial equality between two other parts
        /// </summary>
        /// <param name="leftPart">Column involve involve in partial condition</param>
        /// <param name="rightPart">Object value involve in partial condition</param>
        /// <returns>Corresponding filter</returns>
        public ICondition Like(ColumnValue leftPart, ObjectValue rightPart)
        {
            return new LikeCondition(this, leftPart, rightPart);
        }

        /// <summary>
        /// Create a new Filter part for an In sql condition
        /// </summary>
        /// <param name="leftPart">Column involve in In condition</param>
        /// <param name="rightPart">Collection filter that specified left part should be</param>
        /// <returns>Corresponding filter</returns>
        public ICondition In(IFilter leftPart, IFilter rightPart)
        {
            return new InCondition(this, leftPart, rightPart);
        }
        
        /// <summary>
        /// Create a new Filter part for an In sql condition
        /// </summary>
        /// <param name="leftPart">Column involve involve and should be contains other part</param>
        /// <param name="rightPart">Collection value that specified column should be</param>
        /// <returns>Corresponding filter</returns>
        public ICondition In(ColumnValue leftPart, ObjectValue rightPart)
        {
            return new InCondition(this, leftPart, rightPart);
        }

        /// <summary>
        /// Create a new Filter part for a comparation between two other parts
        /// </summary>
        /// <param name="leftPart">Column involve involve and should be lower than other part</param>
        /// <param name="rightPart">Other column involve and should be greater than other part</param>
        /// <returns>Corresponding filter</returns>
        public ICondition LessThan(IFilter leftPart, IFilter rightPart)
        {
            return new LessThanCondition(this, leftPart, rightPart);
        }

        /// <summary>
        /// Create a new Filter part for a comparation between two other parts
        /// </summary>
        /// <param name="leftPart">Column involve involve and should be lower than other part</param>
        /// <param name="rightPart">Object value involve and should be greater than other part</param>
        /// <returns>Corresponding filter</returns>
        public ICondition LessThan(ColumnValue leftPart, ObjectValue rightPart)
        {
            return new LessThanCondition(this, leftPart, rightPart);
        }

        /// <summary>
        /// Create a new Filter part for a comparation between two other parts
        /// </summary>
        /// <param name="leftPart">Column involve involve and should be greater than other part</param>
        /// <param name="rightPart">Other column involve and should be lower than other part</param>
        /// <returns>Corresponding filter</returns>
        public ICondition GreaterThan(IFilter leftPart, IFilter rightPart)
        {
            return new GreaterThanCondition(this, leftPart, rightPart);
        }

        /// <summary>
        /// Create a new Filter part for a comparation between two other parts
        /// </summary>
        /// <param name="leftPart">Column involve involve and should be greater than other part</param>
        /// <param name="rightPart">Object value involve and should be lower than other part</param>
        /// <returns>Corresponding filter</returns>
        public ICondition GreaterThan(ColumnValue leftPart, ObjectValue rightPart)
        {
            return new GreaterThanCondition(this, leftPart, rightPart);
        }

        /// <summary>
        /// Create a new Filter part for a comparation between two other parts
        /// </summary>
        /// <param name="leftPart">Column involve involve and should be lower or equal than other part</param>
        /// <param name="rightPart">Other column involve and should be greater or equal than other part</param>
        /// <returns>Corresponding filter</returns>
        public ICondition LessThanOrEqual(IFilter leftPart, IFilter rightPart)
        {
            return new LessThanOrEqualCondition(this, leftPart, rightPart);
        }

        /// <summary>
        /// Create a new Filter part for a comparation between two other parts
        /// </summary>
        /// <param name="leftPart">Column involve involve and should be lower or equal than other part</param>
        /// <param name="rightPart">Object value involve and should be greater or equal than other part</param>
        /// <returns>Corresponding filter</returns>
        public ICondition LessThanOrEqual(ColumnValue leftPart, ObjectValue rightPart)
        {
            return new LessThanOrEqualCondition(this, leftPart, rightPart);
        }

        /// <summary>
        /// Create a new Filter part for a comparation between two other parts
        /// </summary>
        /// <param name="leftPart">Column involve involve and should be greater or equal than other part</param>
        /// <param name="rightPart">Other column involve and should be lower or equal than other part</param>
        /// <returns>Corresponding filter</returns>
        public ICondition GreaterThanOrEqual(IFilter leftPart, IFilter rightPart)
        {
            return new GreaterThanOrEqualCondition(this, leftPart, rightPart);
        }

        /// <summary>
        /// Create a new Filter part for a comparation between two other parts
        /// </summary>
        /// <param name="leftPart">Column involve involve and should be greater or equal than other part</param>
        /// <param name="rightPart">Object value involve and should be lower or equal than other part</param>
        /// <returns>Corresponding filter</returns>
        public ICondition GreaterThanOrEqual(ColumnValue leftPart, ObjectValue rightPart)
        {
            return new GreaterThanOrEqualCondition(this, leftPart, rightPart);
        }

        /// <summary>
        /// Create a new Filter part for a condition in according with specified filter operator
        /// </summary>
        /// <param name="leftPart">Left part involve in condition</param>
        /// <param name="rightPart">Right part involve in condition</param>
        /// <param name="filterOperator">Operator to apply for filter</param>
        /// <returns>Corresponding filter</returns>
        public ICondition Condition(ColumnValue leftPart, ObjectValue rightPart,
            FilterOperator filterOperator)
        {
            switch (filterOperator)
            {
                case FilterOperator.Equals:
                    return Equal(leftPart, rightPart);
                case FilterOperator.NotEqual:
                    return NotEqual(leftPart, rightPart);
                case FilterOperator.Like:
                    return Like(leftPart, rightPart);
                case FilterOperator.LessThan:
                    return LessThan(leftPart, rightPart);
                case FilterOperator.GreaterThan:
                    return GreaterThan(leftPart, rightPart);
                case FilterOperator.LessThanOrEqual:
                    return LessThanOrEqual(leftPart, rightPart);
                case FilterOperator.GreaterThanOrEqual:
                    return GreaterThanOrEqual(leftPart, rightPart);
                case FilterOperator.In:
                    return In(leftPart, rightPart);
                default:
                    throw new NotSupportedException();
            }
        }

        /// <summary>
        /// Create a new Filter part for a condition in according with specified filter operator
        /// </summary>
        /// <param name="leftPart">Left part involve in condition</param>
        /// <param name="rightPart">Right part involve in condition</param>
        /// <param name="filterOperator">Operator to apply for filter</param>
        /// <returns>Corresponding filter</returns>
        public ICondition Condition(ColumnValue leftPart, ColumnValue rightPart,
            FilterOperator filterOperator)
        {
            switch (filterOperator)
            {
                case FilterOperator.Equals:
                    return Equal(leftPart, rightPart);
                case FilterOperator.NotEqual:
                    return NotEqual(leftPart, rightPart);
                case FilterOperator.Like:
                    return Like(leftPart, rightPart);
                case FilterOperator.LessThan:
                    return LessThan(leftPart, rightPart);
                case FilterOperator.GreaterThan:
                    return GreaterThan(leftPart, rightPart);
                case FilterOperator.LessThanOrEqual:
                    return LessThanOrEqual(leftPart, rightPart);
                case FilterOperator.GreaterThanOrEqual:
                    return GreaterThanOrEqual(leftPart, rightPart);
                case FilterOperator.In:
                    return In(leftPart, rightPart);
                default:
                    throw new NotSupportedException();
            }
        }

        /// <summary>
        /// Create a new Filter part for a condition in according with specified filter operator
        /// </summary>
        /// <param name="leftPart">Left part involve in condition</param>
        /// <param name="rightPart">Right part involve in condition</param>
        /// <param name="filterOperator">Operator to apply for filter</param>
        /// <returns>Corresponding filter</returns>
        public ICondition Condition(IFilter leftPart, IFilter rightPart, FilterOperator filterOperator)
        {
            switch (filterOperator)
            {
                case FilterOperator.Equals:
                    return Equal(leftPart, rightPart);
                case FilterOperator.NotEqual:
                    return NotEqual(leftPart, rightPart);
                case FilterOperator.Like:
                    return Like(leftPart, rightPart);
                case FilterOperator.LessThan:
                    return LessThan(leftPart, rightPart);
                case FilterOperator.GreaterThan:
                    return GreaterThan(leftPart, rightPart);
                case FilterOperator.LessThanOrEqual:
                    return LessThanOrEqual(leftPart, rightPart);
                case FilterOperator.GreaterThanOrEqual:
                    return GreaterThanOrEqual(leftPart, rightPart);
                case FilterOperator.In:
                    return In(leftPart, rightPart);
                default:
                    throw new NotSupportedException();
            }
        }

        public IFilter AddDay<TEntity>(DateTime date, string columnName)
        {
            return new AddDay(ToObjectValue(date), BuildColumnValue(typeof(TEntity), columnName));
        }

        public ICondition Condition<TEntity>(string columnName, object value, FilterOperator filterOperator)
        {
            return Condition(typeof(TEntity), columnName, value, filterOperator);
        }

        public ICondition Condition(Type entityType, string columnName, object value, FilterOperator filterOperator)
        {
            var leftPart = BuildColumnValue(entityType, columnName);
            var rightPart = ToObjectValue(value);
            return Condition(leftPart, rightPart, filterOperator);
        }

        public ICondition Condition<TEntity1, TEntity2>(string columnName1, string columnName2, FilterOperator filterOperator)
        {
            return Condition(typeof(TEntity1), typeof(TEntity2), columnName1, columnName2, filterOperator);
        }

        public ICondition Condition(Type entityType1, Type entityType2, string columnName1, string columnName2, FilterOperator filterOperator)
        {
            var leftPart = BuildColumnValue(entityType1, columnName1);
            var rightPart = BuildColumnValue(entityType2, columnName2);
            var result = Condition(leftPart, rightPart, filterOperator);
            result = AddEntitiesToCondition(result, entityType1, entityType2);
            return result;
        }

        public ICondition Condition<TEntity>(string columnName, IFilter scalarValue, FilterOperator filterOperator)
        {
            if (scalarValue == null) // scalar value can not be null and sign method is same as object condition. So redirect to it
                return Condition<TEntity>(columnName, (object) null, filterOperator);

            var leftPart = BuildColumnValue(typeof(TEntity), columnName);
            return Condition((IFilter)leftPart, scalarValue, filterOperator);
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
            var rightPart = ToObjectValue(value);
            return Condition(leftPart, rightPart, filterOperator);
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
            var rightPart = ToObjectValue(value);
            return Condition(leftPart, rightPart, filterOperator);
        }

        /// <summary>
        /// Create a new column wich will be used on a request
        /// </summary>
        /// <typeparam name="TEntity">Type of entity owner</typeparam>
        /// <param name="columnName">Column Name</param>
        /// <returns>New column value statement</returns>
        public ColumnValue GetColumn<TEntity>(string columnName)
        {
            return BuildColumnValue(typeof(TEntity), columnName);
        }

        private ColumnValue BuildColumnValue(Type entityType, string columnName)
        {
            var entityName = _entities.GetNameForType(entityType);
            var entityInfo = _entities[entityName];
            return ToColumnValue(entityInfo, columnName);
        }

        private ICondition AddEntitiesToCondition(ICondition condition, Type entityType1, Type entityType2)
        {
            condition = AddEntityToCondition(condition, entityType1);
            condition = AddEntityToCondition(condition, entityType2);

            return condition;
        }

        private ICondition AddEntityToCondition(ICondition condition, Type entityType)
        {
            var entityName = _entities.GetNameForType(entityType);
            var entityInfo = _entities[entityName];
            if (condition.Entity1 == null)
            {
                condition.Entity1 = entityInfo;
            }
            else
            {
                condition.Entity2 = entityInfo;
            }

            return condition;
        }

        /// <summary>
        /// Create a new ScalarValue wich will apply scalar operator on specified column
        /// </summary>
        /// <typeparam name="TEntity1">Type of entity associated to column name 1</typeparam>
        /// <typeparam name="TEntity2">Type of entity associated to column name 2</typeparam>
        /// <param name="columnName1">First column Name involve</param>
        /// <param name="columnName2">Second column Name to compare</param>
        /// <param name="scalarOperator">Operator to apply between two field</param>
        /// <returns>New scalar operation</returns>
        public IFilter Scalar<TEntity1, TEntity2>(string columnName1, string columnName2, ScalarOperator scalarOperator)
        {
            return Scalar(typeof(TEntity1), typeof(TEntity2), columnName1, columnName2, scalarOperator);
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
        public IFilter Scalar(Type entityType1, Type entityType2, string columnName1, string columnName2,
            ScalarOperator scalarOperator)
        {
            var leftPart = BuildColumnValue(entityType1, columnName1);
            var rightPart = BuildColumnValue(entityType2, columnName2);
            return Scalar(leftPart, rightPart, scalarOperator);
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
            switch (scalarOperator)
            {
                case ScalarOperator.Subtract:
                    return SubtractOperation(leftPart, rightPart);
                default:
                    throw new NotSupportedException();
            }
        }

        /// <summary>
        /// Create a new Filter part for an Subtract Operation between two other parts
        /// </summary>
        /// <param name="leftPart">Column involve in subtract condition</param>
        /// <param name="rightPart">Other column part involve in equality condition</param>
        /// <returns>Corresponding filter</returns>
        public ICondition SubtractOperation(IFilter leftPart, IFilter rightPart)
        {
            return new SubtractOperation(this, leftPart, rightPart);
        }
    }
}