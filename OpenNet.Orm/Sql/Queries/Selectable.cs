using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using OpenNet.Orm.Filters;
using OpenNet.Orm.Interfaces;
using OpenNet.Orm.Queries;

// ReSharper disable ConvertPropertyToExpressionBody
// ReSharper disable AutoPropertyCanBeMadeGetOnly.Local

namespace OpenNet.Orm.Sql.Queries
{
    public class Selectable<TIEntity> : IClause, IJoinable<TIEntity> where TIEntity : class
    {
        private IDataStore Datastore { get; set; }
        private QuerySet Query { get; set; }
        private ISelectable SelectStatement { get; set; }

        public Selectable(IDataStore datastore, EntityInfoCollection entities)
        {
            Datastore = datastore;
            Query = new QuerySet(entities, typeof(TIEntity));
        }

        public Selectable(IDataStore datastore, EntityInfoCollection entities, Type entityInvolve)
        {
            Datastore = datastore;
            Query = new QuerySet(entities, entityInvolve);
        }

        public IEntityInfo Entity {  get { return Query.Entity; } }

        public List<IJoin> JoinList { get { return Query.JoinList; } }

        public EntityInfoCollection Entities { get { return Query.Entities; } }

        public Dictionary<string, IEntityInfo> EntityInvolve { get { return Query.EntityInvolve; } }


        public string ToStatement(List<IDataParameter> @params)
        {
            var result = new StringBuilder();
            if (SelectStatement != null)
            {
                result.Append(SelectStatement.SelectStatement());
                result.Append(" ");
            }

            result.Append(Query.ToStatement(@params));
            return result.ToString();
        }

        public IJoinable<TIEntity> Join<TEntity1, TEntity2>()
        {
            Query.Join<TEntity1, TEntity2>();
            return this;
        }

        public IJoinable<TIEntity> Join(ICondition condition)
        {
            Query.Join(condition);
            return this;
        }

        public IJoinable<TIEntity> LeftJoin<TEntity1, TEntity2>()
        {
            Query.LeftJoin<TEntity1, TEntity2>();
            return this;
        }

        public IJoinable<TIEntity> LeftJoin(ICondition condition)
        {
            Query.LeftJoin(condition);
            return this;
        }

        public IAggragableQuery<TIEntity> Where(IFilter filter)
        {
            Query.Where(filter);
            return this;
        }

        public IOrderableQuery<TIEntity> GroupBy(params ColumnValue[] columns)
        {
            Query.AddGroupBy(columns);
            return this;
        }

        public IOrderedQuery<TIEntity> OrderBy(ColumnValue field)
        {
            Query.AddOrderBy(field);
            return this;
        }

        public IOrderedQuery<TIEntity> OrderByDesc(ColumnValue field)
        {
            Query.AddOrderByDesc(field);
            return this;
        }

        public IOrderedQuery<TIEntity> ThenBy(ColumnValue field)
        {
            Query.AddOrderBy(field);
            return this;
        }

        public IOrderedQuery<TIEntity> ThenByDesc(ColumnValue field)
        {
            Query.AddOrderByDesc(field);
            return this;
        }

        public IEnumerable<TIEntity> GetValues()
        {
            var select = new Select<TIEntity>(this);
            SelectStatement = select;
            return Datastore.ExecuteQuery(this, select);
        }

        public IEnumerable<TIEntity> Top(int quantity)
        {
            var select = new SelectTop<TIEntity>(this, quantity);
            SelectStatement = select;
            return Datastore.ExecuteQuery(this, select);
        }

        public int Count()
        {
            var countedEntity = Query.Entity;
            var columnValue = Datastore.ToColumnValue(countedEntity, countedEntity.PrimaryKey.FieldName);
            SelectStatement = Queries.Count.CreateColumnCount(columnValue);
            return Datastore.ExecuteScalar(this);
        }

        public int Delete()
        {
            SelectStatement = new Delete();
            return Datastore.ExecuteNonQuery(this);
        }

        public IEnumerable<KeyValuePair<object[], long>> Count(params ColumnValue[] columns)
        {
            SelectStatement = Queries.Count.CreateTableCount(columns);
            return Datastore.ExecuteQuery(this);
        }
    }
}