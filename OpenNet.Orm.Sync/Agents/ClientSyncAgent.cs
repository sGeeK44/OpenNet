using System;
using System.Linq;
using OpenNet.Orm.Filters;
using OpenNet.Orm.Interfaces;
using OpenNet.Orm.Sql;
using OpenNet.Orm.Sync.Changes;
using OpenNet.Orm.Sync.Conflicts;
using OpenNet.Orm.Sync.Entity;

// ReSharper disable ArrangeAccessorOwnerBody

namespace OpenNet.Orm.Sync.Agents
{
    public class ClientSyncAgent : SyncAgent
    {
        public ClientSyncAgent(IDataStore dataStore, ISyncTransport transport, IDateTimeManager dateTimeManager, IOrmLogger logger)
            : base(dataStore, transport, dateTimeManager, logger) { }

        protected override string Name
        {
            get { return "Client"; }
        }

        public void Synchronize(IOrmSyncObserver syncObserver)
        {
            SyncStart(syncObserver);
            InitSync();
            SendClientChange();
            DetermineTypeOfSync();
            SyncServerChange();
            FinalizeSync();
        }

        private void SyncServerChange()
        {
            if (SyncType == SyncTypes.OneWay)
                return;

            GetRemoteChanges();
            ApplyRemoteChange(new IgnoreAllConflicts());
            var conflicts = Transport.Receive<ResolveAllConflicts>();
            StatProvider.SetConflicts(conflicts);
            conflicts.ApplyRemoteResolution(DataStore as ISqlDataStore, SyncSession, StatProvider);
        }

        private void SendClientChange()
        {
            Transport.Send(LocalEntitiesChangeset);
        }

        protected override void EstablishNewSession()
        {
            base.EstablishNewSession();
            WaitDesktopIsReady();
            DataStore.AddTypeSafe<SyncSessionInfo>();
            SyncSession = SyncSessionInfo.Create(DataStore, DateTimeManager);
            Transport.Send(SyncSession as SyncSessionInfo);
        }

        private void WaitDesktopIsReady()
        {
            Transport.Receive<bool>();
        }

        protected override void DetermineTypeOfSync()
        {
            base.DetermineTypeOfSync();
            SyncType = Transport.Receive<SyncTypes>();
        }

        protected override EntitiesChangeset CreateEntitiesChangeset()
        {
            return new ClientEntitiesChangeset(DataStore as ISqlDataStore, SyncSession);
        }

        protected override void CloseSession()
        {
            StatProvider.SetNewState(SyncStates.ClosingSession);

            // TODO Should be manage by client sync not by SyncMode
            if (SyncType != SyncTypes.OneWay)
            {
                SyncSession.HasSuccess = true;
                var syncSessionRepo = new SyncSessionInfoRepository(DataStore);
                syncSessionRepo.Save(SyncSession);
            }

            Transport.Send<bool>(true);
        }

        protected override void CleanEntities()
        {
            StatProvider.SetNewState(SyncStates.CleanEntites);
            CleanEntities(DataStore, SyncSession.HighBoundaryAnchor);
        }

        public static void CleanEntities(IDataStore dataStore, DateTime now)
        {
            foreach (var entity in dataStore.Entities.Reverse())
            {
                var syncableEntity = SyncEntity.Create(entity);
                if (syncableEntity == null || !syncableEntity.ShouldBeCleannedOnClient())
                    continue;

                var createdAt = dataStore.ToColumnValue(entity, syncableEntity.CreationTrackingColumn);
                var retentionDate = now.AddDays(-1 * syncableEntity.ClientRetentionTime);
                var createdBeforeRetentionLimit = dataStore.Condition(createdAt, retentionDate, FilterOperator.LessThanOrEqual);
                var legacyRow = dataStore.Condition(createdAt, null, FilterOperator.Equals);
                dataStore.Select(entity.EntityType)
                    .Where(createdBeforeRetentionLimit.Or(legacyRow))
                    .Delete();
            }
        }
    }
}
