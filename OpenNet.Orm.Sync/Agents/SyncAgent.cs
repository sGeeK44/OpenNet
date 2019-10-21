using OpenNet.Orm.Interfaces;
using OpenNet.Orm.Sql;
using OpenNet.Orm.Sync.Changes;
using OpenNet.Orm.Sync.Conflicts;
// ReSharper disable ArrangeAccessorOwnerBody
// ReSharper disable AutoPropertyCanBeMadeGetOnly.Local

namespace OpenNet.Orm.Sync.Agents
{
    public abstract class SyncAgent
    {
        protected readonly IDateTimeManager DateTimeManager;
        protected readonly ISyncTransport Transport;

        protected SyncAgent(IDataStore dataStore, ISyncTransport transport, IDateTimeManager dateTimeManager, IOrmLogger logger)
        {
            Transport = transport;
            DateTimeManager = dateTimeManager;
            DataStore = dataStore;
            Logger = logger;
        }

        protected abstract string Name { get; }

        protected IDataStore DataStore { get; private set; }

        protected ISyncSessionInfo SyncSession { get; set; }

        protected SyncStatProvider StatProvider { get; set; }

        public EntitiesChangeset LocalEntitiesChangeset { get; set; }

        public EntitiesChangeset RemoteChanges { get; set; }

        public SyncTypes SyncType { get; set; }

        protected IOrmLogger Logger { get; private set; }

        public void AbortSync()
        {
            Transport.Abort();
        }

        public void Initialize()
        {
            Transport.Initialize();
        }

        protected virtual void SyncStart(IOrmSyncObserver syncObserver)
        {
            SyncObserver = syncObserver;
            StatProvider = new SyncStatProvider(syncObserver, DateTimeManager, Logger)
            {
                Source = Name
            };
            StatProvider.Start();
        }

        public IOrmSyncObserver SyncObserver { get; set; }

        protected void InitSync()
        {
            EstablishNewSession();
            ComputeLocalChange();
        }

        protected void FinalizeSync()
        {
            CleanEntities();
            CloseSession();
            SyncEnd();
        }

        private void SyncEnd()
        {
            StatProvider.EndSync();
        }

        protected virtual void EstablishNewSession()
        {
            StatProvider.SetNewState(SyncStates.EstablishNewSession);
        }

        protected virtual void DetermineTypeOfSync()
        {
            StatProvider.SetNewState(SyncStates.DetermineTypeOfSync);
        }

        public void ComputeLocalChange()
        {
            LocalEntitiesChangeset = CreateEntitiesChangeset();
            LocalEntitiesChangeset.Build(StatProvider);
        }

        protected abstract EntitiesChangeset CreateEntitiesChangeset();

        protected void GetRemoteChanges()
        {
            Transport.AddObserver(SyncStates.GettingRemoteChange, SyncObserver);
            RemoteChanges = Transport.Receive<EntitiesChangeset>();
            RemoteChanges.SetSyncSession(SyncSession);
            StatProvider.SetRemoteChanges(RemoteChanges);
        }

        protected void ApplyRemoteChange(IConflictsManager conflictsManager)
        {
            RemoteChanges.ApplyChanges(DataStore as ISqlDataStore, StatProvider, conflictsManager);
            StatProvider.SetConflicts(conflictsManager);
        }

        protected abstract void CleanEntities();

        protected abstract void CloseSession();

        public static void InitSyncSessionInfo(IDataStore datastore, IDateTimeManager dateTimeProvider)
        {
            var session = SyncSessionInfo.Create(dateTimeProvider, null);
            session.HasSuccess = true;
            var syncSessionRepo = new SyncSessionInfoRepository(datastore);
            datastore.AddTypeSafe<SyncSessionInfo>();
            syncSessionRepo.Save(session);
        }
    }
}