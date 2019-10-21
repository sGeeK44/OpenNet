using System;
using OpenNet.Orm.Interfaces;

namespace OpenNet.Orm.Filters
{
    public interface IFilterFactory
    {
        /// <summary>
        /// Build scalar to add the specified number of units of time to a specified date
        /// </summary>
        /// <typeparam name="TEntity">Type of entity associated to column name</typeparam>
        /// <param name="date">Date on wich amount of day will be added</param>
        /// <param name="columnName">Column name wich hold amount of day to add</param>
        /// <returns>IScalar object</returns>
        IFilter AddDay<TEntity>(DateTime date, string columnName);

        /// <summary>
        /// Create new condition
        /// </summary>
        /// <typeparam name="TEntity">Type of entity associated to column name</typeparam>
        /// <param name="columnName">Column Name involve</param>
        /// <param name="value">Value to compare to column name</param>
        /// <param name="filterOperator">Operator to apply</param>
        /// <returns>New build condition</returns>
        ICondition Condition<TEntity>(string columnName, object value, FilterOperator filterOperator);

        /// <summary>
        /// Create new condition
        /// </summary>
        /// <param name="entityType">Type of entity associated to column name</param>
        /// <param name="columnName">Column Name involve</param>
        /// <param name="value">Value to compare to column name</param>
        /// <param name="filterOperator">Operator to apply</param>
        /// <returns>New build condition</returns>
        ICondition Condition(Type entityType, string columnName, object value, FilterOperator filterOperator);

        /// <summary>
        /// Create new condition
        /// </summary>
        /// <typeparam name="TEntity1">Type of entity associated to column name 1</typeparam>
        /// <typeparam name="TEntity2">Type of entity associated to column name 2</typeparam>
        /// <param name="columnName1">First column Name involve</param>
        /// <param name="columnName2">Second column Name to compare</param>
        /// <param name="filterOperator">Operator to apply</param>
        /// <returns>New build condition</returns>
        ICondition Condition<TEntity1, TEntity2>(string columnName1, string columnName2, FilterOperator filterOperator);

        /// <summary>
        /// Create new condition
        /// </summary>
        /// <param name="entityType1">Type of entity associated to column name 1</param>
        /// <param name="entityType2">Type of entity associated to column name 2</param>
        /// <param name="columnName1">First column Name involve</param>
        /// <param name="columnName2">Second column Name to compare</param>
        /// <param name="filterOperator">Operator to apply</param>
        /// <returns>New build condition</returns>
        ICondition Condition(Type entityType1, Type entityType2, string columnName1, string columnName2, FilterOperator filterOperator);

        /// <summary>
        /// Create new condition
        /// </summary>
        /// <typeparam name="TEntity">Type of entity associated to column name</typeparam>
        /// <param name="columnName">Column Name involve</param>
        /// <param name="scalarValue">Value to compare to column name</param>
        /// <param name="filterOperator">Operator to apply</param>
        /// <returns>New build condition</returns>
        ICondition Condition<TEntity>(string columnName, IFilter scalarValue, FilterOperator filterOperator);

        /// <summary>
        /// Create new condition
        /// </summary>
        /// <param name="leftPart">First paert involve in filter</param>
        /// <param name="value">Value to compare to column name</param>
        /// <param name="filterOperator">Operator to apply</param>
        /// <returns>New build condition</returns>
        ICondition Condition(IFilter leftPart, object value, FilterOperator filterOperator);

        /// <summary>
        /// Create new condition
        /// </summary>
        /// <param name="leftPart">First paert involve in filter</param>
        /// <param name="value">Value to compare to column name</param>
        /// <param name="filterOperator">Operator to apply</param>
        /// <returns>New build condition</returns>
        ICondition Condition(ColumnValue leftPart, object value, FilterOperator filterOperator);

        /// <summary>
        /// Create a new ScalarValue wich will apply scalar operator on specified column
        /// </summary>
        /// <typeparam name="TEntity1">Type of entity associated to column name 1</typeparam>
        /// <typeparam name="TEntity2">Type of entity associated to column name 2</typeparam>
        /// <param name="columnName1">First column Name involve</param>
        /// <param name="columnName2">Second column Name involve</param>
        /// <param name="scalarOperator">Operator to apply between two field</param>
        /// <returns>New scalar operation</returns>
        IFilter Scalar<TEntity1, TEntity2>(string columnName1, string columnName2, ScalarOperator scalarOperator);

        /// <summary>
        /// Create a new ScalarValue wich will apply scalar operator on specified column
        /// </summary>
        /// <param name="entityType1">Type of entity associated to column name 1</param>
        /// <param name="entityType2">Type of entity associated to column name 2</param>
        /// <param name="columnName1">First column Name involve</param>
        /// <param name="columnName2">Second column Name involve</param>
        /// <param name="scalarOperator">Operator to apply between two field</param>
        /// <returns>New scalar operation</returns>
        IFilter Scalar(Type entityType1, Type entityType2, string columnName1, string columnName2, ScalarOperator scalarOperator);

        /// <summary>
        /// Create a new Filter part for a condition in according with specified filter operator
        /// </summary>
        /// <param name="leftPart">Left part involve in condition</param>
        /// <param name="rightPart">Right part involve in condition</param>
        /// <param name="scalarOperator">Operator to apply between two field</param>
        /// <returns>Corresponding filter</returns>
        IFilter Scalar(IFilter leftPart, IFilter rightPart, ScalarOperator scalarOperator);

        /// <summary>
        /// Create a new column wich will be used on a request
        /// </summary>
        /// <typeparam name="TEntity">Type of entity owner</typeparam>
        /// <param name="columnName">Column Name</param>
        /// <returns>New column value statement</returns>
        ColumnValue GetColumn<TEntity>(string columnName);

        /// <summary>
        /// Create a new Filter part for a value
        /// </summary>
        /// <param name="value">A ref to a column value</param>
        /// <returns>Corresponding filter</returns>
        ObjectValue ToObjectValue(object value);

        /// <summary>
        /// Create a new Filter part for a column ref
        /// </summary>
        /// <param name="entity">Entity which own column name</param>
        /// <param name="columnName">String name of column</param>
        /// <returns>Corresponding filter</returns>
        ColumnValue ToColumnValue(IEntityInfo entity, string columnName);
    }
}