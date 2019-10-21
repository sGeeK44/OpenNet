using Moq;
using OpenNet.Orm.Sync;
using OpenNet.Orm.Sync.Agents;

namespace OpenNet.Orm.Testkit
{
    public class SyncableClient : SyncableStore
    {
        public SyncableClient(IDateTimeManager timeManager, LocalBoundTransport transport, string name)
            : base(timeManager, transport, name)
        { }

        public ClientSyncAgent Agent { get; set; }

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
            Agent = new ClientSyncAgent(DataStore, Transport, TimeManager, new Mock<IOrmLogger>().Object);
        }
    }
}