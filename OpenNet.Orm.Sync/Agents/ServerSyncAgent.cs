using System;
using System.Threading;
using OpenNet.Orm.Interfaces;
using OpenNet.Orm.Sql;
using OpenNet.Orm.Sync.Changes;
using OpenNet.Orm.Sync.Conflicts;

// ReSharper disable ArrangeAccessorOwnerBody

namespace OpenNet.Orm.Sync.Agents
{
    public class ServerSyncAgent : SyncAgent
    {
        private const long MaxChangeForDeltaSync = 500;

        public ServerSyncAgent(IDataStore dataStore, ISyncTransport transport, IDateTimeManager dateTimeManager, IOrmLogger logger)
            : base(dataStore, transport, dateTimeManager, logger) { }

        protected override string Name
        {
            get { return "Server"; }
        }

        public bool IsSyncInProgress { get; private set; }
        public bool ErrorOccurs { get; private set; }
        private ResolveAllConflicts Conflicts { get; set; }

        public void Synchronise(IOrmSyncObserver syncObserver)
        {
            SyncStart(syncObserver);
            InitServer();
            new Thread(Work).Start();
        }

        private void Work()
        {
            try
            {
                InitSync();
                Conflicts = SyncRemoteChange();
                DetermineTypeOfSync();
                SyncServerChange();
                FinalizeSync();
            }
            catch (Exception ex)
            {
                ErrorOccurs = true;
                Logger.Error(ex);
                StatProvider.SetNewState(SyncStates.InError);
            }
            finally
            {
                IsSyncInProgress = false;
            }
        }

        private void InitServer()
        {
            IsSyncInProgress = true;
            ErrorOccurs = false;
        }

        private void SyncServerChange()
        {
            if (SyncType == SyncTypes.OneWay)
                return;

            Conflicts.RemoveInvolve(LocalEntitiesChangeset);

            StatProvider.SetNewState(SyncStates.SendingServerChange, Transport);
            Transport.Send(LocalEntitiesChangeset);

            StatProvider.SetNewState(SyncStates.SendingConflictsSolved, Transport);
            Transport.Send(Conflicts);
        }

        private ResolveAllConflicts SyncRemoteChange()
        {
            GetRemoteChanges();
            var conflicts = new ResolveAllConflicts();
            conflicts.AddDeletedEntityChange(LocalEntitiesChangeset);
            ApplyRemoteChange(conflicts);
            return conflicts;
        }

        protected override void SyncStart(IOrmSyncObserver syncObserver)
        {
            base.SyncStart(syncObserver);
            if (!Transport.IsPairConnected)
                throw new NotSupportedException("Device should be connected.");
        }

        protected override void EstablishNewSession()
        {
            base.EstablishNewSession();
            IndicateRemoteWeAreReady();
            SyncSession = Transport.Receive<SyncSessionInfo>();
        }

        private void IndicateRemoteWeAreReady()
        {
            Transport.Send(true);
        }

        protected override void DetermineTypeOfSync()
        {
            base.DetermineTypeOfSync();

            SyncType = StatProvider.ChangesUploaded > MaxChangeForDeltaSync
                    || Conflicts.ShouldFullSync
                     ? SyncTypes.OneWay : SyncTypes.TwoWay;
            Transport.Send(SyncType);
        }

        protected override void CloseSession()
        {
            Transport.AddObserver(SyncStates.ClosingSession, SyncObserver);
            Transport.Receive<bool>();
        }

        protected override EntitiesChangeset CreateEntitiesChangeset()
        {
            return new ServerEntitiesChangeset(DataStore as ISqlDataStore, SyncSession);
        }

        protected override void CleanEntities() { }
    }
}
