using System.Collections.Generic;
using System.Data;
using OpenNet.Orm.Filters;
using OpenNet.Orm.Interfaces;
using OpenNet.Orm.Queries;
using OpenNet.Orm.Sync.Entity;

// ReSharper disable AutoPropertyCanBeMadeGetOnly.Local
// ReSharper disable UseStringInterpolation

namespace OpenNet.Orm.Sync.SyncQueries
{
    public abstract class SyncQuery : IClause
    {
        private readonly IDataStore _datastore;
        private readonly ISyncSessionInfo _syncSession;

        protected SyncQuery(IDataStore datastore, ISyncableEntity entity, ISyncSessionInfo syncSession)
        {
            _datastore = datastore;
            _syncSession = syncSession;
            Entity = entity;
        }

        protected ISyncableEntity Entity { get; private set; }

        protected abstract string TrackingColumn { get; }

        public string ToStatement(List<IDataParameter> @params)
        {
            var lastAnchor = _syncSession.LowBoundaryAnchor;
            var newAnchor = _syncSession.HighBoundaryAnchor;

            return CreateSelect(TrackingColumn, lastAnchor, newAnchor, @params);
        }

        private string CreateSelect(string columnName, object lastAnchor, object newAnchor, List<IDataParameter> @params)
        {
            var condition = CreateFilterCondition(columnName, lastAnchor, newAnchor);
            return string.Format("SELECT * FROM [{0}] WHERE {1}", Entity.GetNameInStore(), condition.ToStatement(@params));
        }

        private ICondition CreateFilterCondition(string columnName, object lastAnchor, object newAnchor)
        {
            var trackedColumn = _datastore.ToColumnValue(Entity, columnName);
            var greatherThanLowBound = _datastore.Condition(trackedColumn, lastAnchor, FilterOperator.GreaterThan);
            var lowerOrEqualThanUpBound = _datastore.Condition(trackedColumn, newAnchor, FilterOperator.LessThanOrEqual);
            return greatherThanLowBound.And(lowerOrEqualThanUpBound);
        }
    }
}