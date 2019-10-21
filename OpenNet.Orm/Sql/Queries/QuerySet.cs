using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using OpenNet.Orm.Filters;
using OpenNet.Orm.Interfaces;
using OpenNet.Orm.Queries;

// ReSharper disable UseStringInterpolation
// ReSharper disable ConvertPropertyToExpressionBody
// ReSharper disable AutoPropertyCanBeMadeGetOnly.Local

namespace OpenNet.Orm.Sql.Queries
{
    public class QuerySet
    {
        private IClause _where;
        private readonly OrderBy _orderBy;
        private readonly GroupBy _groupBy;

        internal QuerySet(EntityInfoCollection entities, Type entityInvolve)
        {
            _where = new Where();
            _orderBy = new OrderBy();
            _groupBy = new GroupBy();
            Entities = entities;
            EntityInvolve = new Dictionary<string, IEntityInfo>();
            Entity = AddInvolveEntity(entityInvolve);
            JoinList = new List<IJoin>();
        }

        public IEntityInfo Entity { get; private set; }

        public List<IJoin> JoinList { get; private set; }

        public EntityInfoCollection Entities { get; set; }

        public Dictionary<string, IEntityInfo> EntityInvolve { get; private set; }

        public void Where(IFilter filter)
        {
            Where(new Where(filter));
        }

        public void Where(IClause sqlPartClause)
        {
            _where = sqlPartClause ?? new Where();
        }

        public void Join<TEntity1, TEntity2>()
        {
            var entityRef = AddInvolveEntity<TEntity1>();
            var entityJoin = AddInvolveEntity<TEntity2>();
            JoinList.Add(new Join(entityRef, entityJoin));
        }

        public void Join(ICondition condition)
        {
            var entityRef = AddInvolveEntity(condition.Entity1.EntityType);
            var entityJoin = AddInvolveEntity(condition.Entity2.EntityType);
            JoinList.Add(new Join(entityRef, entityJoin, condition));
        }

        public void LeftJoin<TEntity1, TEntity2>()
        {
            var entityRef = AddInvolveEntity<TEntity1>();
            var entityJoin = AddInvolveEntity<TEntity2>();
            JoinList.Add(new LeftJoin(entityRef, entityJoin));
        }

        public void LeftJoin(ICondition condition)
        {
            var entityRef = AddInvolveEntity(condition.Entity1.EntityType);
            var entityJoin = AddInvolveEntity(condition.Entity2.EntityType);
            JoinList.Add(new LeftJoin(entityRef, entityJoin, condition));
        }

        public string ToStatement(List<IDataParameter> @params)
        {
            var result = new StringBuilder();
            result.Append(FromStatement());
            result.Append(JoinStatement(@params));
            result.Append(_where.ToStatement(@params));
            result.Append(_groupBy.ToStatement());
            result.Append(_orderBy.ToStatement());
            var sql = result.Append(";").ToString();
            OrmDebug.Trace(sql);
            return sql;
        }

        protected string FromStatement()
        {
            return string.Format("FROM [{0}]", Entity.GetNameInStore());
        }

        protected string JoinStatement(List<IDataParameter> @params)
        {
            return JoinList.Aggregate(string.Empty, (current, join) => current + " " + join.ToStatement(@params));
        }

        private IEntityInfo AddInvolveEntity<TEntity>()
        {
            return AddInvolveEntity(typeof(TEntity));
        }

        private IEntityInfo AddInvolveEntity(Type entityType)
        {
            var entityName = Entities.GetNameForType(entityType);
            if (string.IsNullOrEmpty(entityName))
                throw new NotSupportedException(string.Format("Entity type must be added in datastore. Type:{0}.",
                    entityType));

            var entity = Entities[entityName];
            if (!EntityInvolve.ContainsKey(entityName))
                EntityInvolve.Add(entityName, entity);
            return entity;
        }

        public void AddOrderBy(ColumnValue field)
        {
            _orderBy.AddField(field);
        }

        public void AddOrderByDesc(ColumnValue field)
        {
            _orderBy.AddFieldDesc(field);
        }

        public void AddGroupBy(ColumnValue[] columns)
        {
            _groupBy.AddColumn(columns);
        }
    }
}