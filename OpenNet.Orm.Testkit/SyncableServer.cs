using Moq;
using OpenNet.Orm.Sync;
using OpenNet.Orm.Sync.Agents;

namespace OpenNet.Orm.Testkit
{
    public class SyncableServer : SyncableStore
    {
        private readonly IDateTimeManager _timeManager;

        public SyncableServer(IDateTimeManager timeManager, LocalBoundTransport transport, string name)
            : base(timeManager, transport, name)
        {
            _timeManager = timeManager;
        }

        public ServerSyncAgent Agent { get; set; }

        public override void SetUp()
        {
            base.SetUp();
            InitializeAgent();
        }

        protected override void OnNewDatastore()
        {
            base.OnNewDatastore();
            InitializeAgent();
        }

        private void InitializeAgent()
        {
            Agent = new ServerSyncAgent(DataStore, Transport, _timeManager, new Mock<IOrmLogger>().Object);
        }
    }
}