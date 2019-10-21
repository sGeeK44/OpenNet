using System;
using OpenNet.Orm.Sync;

namespace OpenNet.Orm.Testkit
{
    public class SyncSessionInfoMock : ISyncSessionInfo
    {
        public SyncSessionInfoMock()
        {
            LowBoundaryAnchor = new DateTime(1973, 1, 1);
            HighBoundaryAnchor = DateTime.UtcNow;
        }

        public long Id { get; set; }
        public DateTime LowBoundaryAnchor { get; set; }
        public DateTime HighBoundaryAnchor { get; set; }
        public bool? HasSuccess { get; set; }
    }
}