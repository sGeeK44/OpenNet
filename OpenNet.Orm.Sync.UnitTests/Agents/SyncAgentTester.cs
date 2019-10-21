using Moq;
using OpenNet.Orm.Interfaces;
using OpenNet.Orm.Sql;
using OpenNet.Orm.Sync.Agents;
using OpenNet.Orm.Sync.Changes;
using OpenNet.Orm.Testkit;

namespace OpenNet.Orm.Sync.UnitTests.Agents
{
    public class SyncAgentTester : SyncAgent
    {
        public SyncAgentTester(IDataStore dataStore)
            : base(dataStore, null, null, null)
        {
        }

        protected override string Name
        {
            get { return "SyncAgentTester"; }
        }

        protected override EntitiesChangeset CreateEntitiesChangeset()
        {
            return new EntitiesChangeset(DataStore as ISqlDataStore, SyncSession);
        }

        protected override void CleanEntities()
        {
        }

        protected override void CloseSession()
        {
            
        }

        public void SetSyncSession(SyncSessionInfoMock syncSession)
        {
            SyncSession = syncSession;
        }

        public void CreateStatProvider()
        {
            StatProvider = new SyncStatProvider(new Mock<IOrmSyncObserver>().Object, new Mock<IDateTimeManager>().Object, new Mock<IOrmLogger>().Object);
        }
    }
}