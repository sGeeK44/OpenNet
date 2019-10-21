using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using OpenNet.Orm.Filters;
using OpenNet.Orm.Interfaces;
using OpenNet.Orm.Queries;

// ReSharper disable ConvertPropertyToExpressionBody

namespace OpenNet.Orm.Sql.Queries
{
    public class Join : IJoin
    {
        private const string DefaultJoinClause = "JOIN";
        private readonly IEntityInfo _entityRef;
        private readonly IEntityInfo _entityJoin;
        private readonly string _joinClause;
        private readonly IFilter _filter;

        public Join(IEntityInfo entityRef, IEntityInfo entityJoin)
            : this(DefaultJoinClause, entityRef, entityJoin) { }

        public Join(IEntityInfo entitRef, IEntityInfo entityJoin, IFilter filter)
            : this(DefaultJoinClause, entitRef, entityJoin)
        {
            _filter = filter;
        }
        
        public Join(string joinClause, IEntityInfo entityRef, IEntityInfo entityJoin, IFilter filter)
    : this(joinClause, entityRef, entityJoin)
        {
            _filter = filter;
        }

        protected Join(string joinClause, IEntityInfo entityRef, IEntityInfo entityJoin)
        {
            _joinClause = joinClause;
            _entityRef = entityRef;
            _entityJoin = entityJoin;
        }

        public string ToStatement()
        {
            throw new NotImplementedException();
        }

        public string ToStatement(List<IDataParameter> @params)
        {
            var result = new StringBuilder();
            result.Append(BuildJoin());
            result.Append(BuildOn(@params));
            return result.ToString();
        }

        private string BuildJoin()
        {
            return string.Format("{0} [{1}]", _joinClause, _entityJoin.GetNameInStore());
        }

        private string BuildOn(List<IDataParameter> @params)
        {
            var result = "ON ";
            if (_filter != null)
                return result + _filter.ToStatement(@params);

            var reference = _entityRef.References.First(_ => _.ReferenceEntityType == _entityJoin.EntityType);
            var localField = _entityRef.Fields.First(_ => _.FieldName == reference.LocalReferenceField);
            var foreignField = _entityJoin.Fields.First(_ => _.FieldName == reference.ForeignReferenceField);
            return string.Format(" ON {0} = {1}", localField.FullFieldName, foreignField.FullFieldName);
        }

        public Type EntityType1 { get { return _entityRef.EntityType; } }
        public Type EntityType2 { get { return _entityJoin.EntityType; } }
    }
}