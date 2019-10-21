using System.Linq;
using OpenNet.Orm.Filters;
using OpenNet.Orm.Interfaces;
using OpenNet.Orm.Repositories;

namespace OpenNet.Orm.Sync
{
    public class SyncSessionInfoRepository : Repository<SyncSessionInfo, ISyncSessionInfo>
    {
        public SyncSessionInfoRepository(IDataStore datastore)
            : base(datastore) { }

        public ISyncSessionInfo GetLastSession()
        {
            var hasSuccess = DataStore.Condition<SyncSessionInfo>(SyncSessionInfo.ColumnNameHasSuccess, true, FilterOperator.Equals);
            var syncEndCol = DataStore.GetColumn<SyncSessionInfo>(SyncSessionInfo.ColumnNameHighBoundaryAnchor);
            return DataStore.Select<SyncSessionInfo>().Where(hasSuccess).OrderByDesc(syncEndCol).Top(1).FirstOrDefault();
        }
    }
}
