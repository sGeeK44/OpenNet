using System.Collections.Generic;
using System.Data;
using OpenNet.Orm.Interfaces;

// ReSharper disable AutoPropertyCanBeMadeGetOnly.Local

namespace OpenNet.Orm.Filters
{
    /// <summary>
    /// Encapsulate base behaviour to manage condition between to part filter
    /// </summary>
    public abstract class Condition : ICondition
    {
        private readonly IFilterFactory _filterFactory;

        protected IFilter LeftPart { get; private set; }

        protected IFilter RightPart { get; private set; }

        protected string ConditionValue { get; private set; }

        protected Condition(IFilterFactory filterFactory, IFilter leftPart, IFilter rightPart, string conditionValue)
        {
            _filterFactory = filterFactory;
            LeftPart = leftPart;
            RightPart = rightPart;
            ConditionValue = conditionValue;
        }

        /// <summary>
        /// Convert part to sql string equivalent
        /// </summary>
        /// <param name="params">existing param list to populate in case of part object value</param>
        /// <returns>Sql string representation</returns>
        public virtual string ToStatement(List<IDataParameter> @params)
        {
            return string.Concat(LeftPart.ToStatement(@params), ConditionValue, RightPart.ToStatement(@params));
        }

        /// <summary>
        /// Create an And association with new condition and current 
        /// </summary>
        /// <typeparam name="TEntity">Type of entity associated to column name on new condition</typeparam>
        /// <param name="columnName">Column Name involve on new condition</param>
        /// <param name="value">Value to compare to column name on new condition</param>
        /// <param name="filterOperator">Operator to apply on new condition</param>
        /// <returns>New build condition</returns>
        public ICondition And<TEntity>(string columnName, object value, FilterOperator filterOperator)
        {
            return And(_filterFactory.Condition<TEntity>(columnName, value, filterOperator));
        }

        /// <summary>
        /// Create an And association with new condition and current 
        /// </summary>
        /// <typeparam name="TEntity1">Type of entity associated to column name 1 on new condition</typeparam>
        /// <typeparam name="TEntity2">Type of entity associated to column name 2 on new condition</typeparam>
        /// <param name="columnName1">First column Name involve on new condition</param>
        /// <param name="columnName2">Second column Name to compare on new condition</param>
        /// <param name="filterOperator">Operator to apply on new condition</param>
        /// <returns>New build condition</returns>
        public ICondition And<TEntity1, TEntity2>(string columnName1, string columnName2, FilterOperator filterOperator)
        {
            return And(_filterFactory.Condition<TEntity1, TEntity2>(columnName1, columnName2, filterOperator));
        }

        /// <summary>
        /// Create an And association with specified condition and current 
        /// </summary>
        /// <param name="condition">Condition to associate</param>
        /// <returns>New build condition</returns>
        public ICondition And(ICondition condition)
        {
            return new And(_filterFactory, this, condition);
        }

        /// <summary>
        /// Create an Or association with new condition and current 
        /// </summary>
        /// <typeparam name="TEntity">Type of entity associated to column name on new condition</typeparam>
        /// <param name="columnName">Column Name involve on new condition</param>
        /// <param name="value">Value to compare to column name on new condition</param>
        /// <param name="filterOperator">Operator to apply on new condition</param>
        /// <returns>New build condition</returns>
        public ICondition Or<TEntity>(string columnName, object value, FilterOperator filterOperator)
        {
            return And(_filterFactory.Condition<TEntity>(columnName, value, filterOperator));
        }

        /// <summary>
        /// Create an Or association with new condition and current 
        /// </summary>
        /// <typeparam name="TEntity1">Type of entity associated to column name 1 on new condition</typeparam>
        /// <typeparam name="TEntity2">Type of entity associated to column name 2 on new condition</typeparam>
        /// <param name="columnName1">First column Name involve on new condition</param>
        /// <param name="columnName2">Second column Name to compare on new condition</param>
        /// <param name="filterOperator">Operator to apply on new condition</param>
        /// <returns>New build condition</returns>
        public ICondition Or<TEntity1, TEntity2>(string columnName1, string columnName2, FilterOperator filterOperator)
        {
            return Or(_filterFactory.Condition<TEntity1, TEntity2>(columnName1, columnName2, filterOperator));
        }

        /// <summary>
        /// Create an Or association with specified condition and current 
        /// </summary>
        /// <param name="condition">Condition to associate</param>
        /// <returns>New build condition</returns>
        public ICondition Or(ICondition condition)
        {
            return new Or(_filterFactory, this, condition);
        }

        public IEntityInfo Entity1 { get; set; }
        public IEntityInfo Entity2 { get; set; }
    }
}