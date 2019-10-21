using System;
using System.Linq;
using OpenNet.Orm.Sync.Changes;
using OpenNet.Orm.Sync.Conflicts;
// ReSharper disable MergeConditionalExpression
// ReSharper disable UseStringInterpolation

namespace OpenNet.Orm.Sync.Agents
{
    public class SyncStatProvider : ISyncStatProvider
    {
        private readonly IOrmLogger _logger;
        private readonly IOrmSyncObserver _syncObserver;
        private readonly IDateTimeManager _dateTimeManager;

        public string Source { private get; set; }

        private DateTime SyncStartTime { get; set; }

        private DateTime SyncCompleteTime { get; set; }

        private int ChangesDownloaded { get; set; }

        public int ChangesUploaded { get; set; }

        private int Conflicts { get; set; }

        public SyncStatProvider(IOrmSyncObserver syncObserver, IDateTimeManager dateTimeManager, IOrmLogger logger)
        {
            _syncObserver = syncObserver;
            _dateTimeManager = dateTimeManager;
            _logger = logger;
        }

        public void Info(string infoFormat, params object[] args)
        {
            _logger.Info(string.Format("{0} - ", Source) + string.Format(infoFormat, args));
        }

        public void Start()
        {
            SyncStartTime = _dateTimeManager.UtcNow;
            SetNewState(SyncStates.SyncInProgress);
            Info("{0} - Syncrhonisation start at:{1}", Source, SyncStartTime);
        }

        public void SetLocalChanges(EntitiesChangeset localChange)
        {
            ChangesUploaded = GetStats(localChange);
            Info("{0} - Changes uploaded:{1}", Source, ChangesUploaded);
        }

        public void SetConflicts(IConflictsManager conflicts)
        {
            Conflicts = conflicts.Count;
            Info("{0} - Conflict detected:{1}", Source, Conflicts);
        }

        public void SetRemoteChanges(EntitiesChangeset remoteChanges)
        {
            ChangesDownloaded = GetStats(remoteChanges);
            Info("{0} - Changes downloaded:{1}", Source, ChangesDownloaded);
        }

        public void EndSync()
        {
            SyncCompleteTime = _dateTimeManager.UtcNow;
            SetNewState(SyncStates.Idle);
            Info("{0} - Syncrhonisation end at:{1}", Source, SyncCompleteTime);
            Info("{0} - Syncrhonisation elapsed:{1}.", Source, SyncCompleteTime - SyncStartTime);
        }

        public void SetNewState(SyncStates newState)
        {
            _syncObserver.OnNewState(newState, false);
        }

        public void SetNewState(SyncStates newState, IObservableProgession observableProgression)
        {
            _syncObserver.OnNewState(newState, true);
            observableProgression.AddObserver(newState, _syncObserver);
        }

        private static int GetStats(EntitiesChangeset localChange)
        {
            return localChange != null
                 ? localChange.EntityChangeset.Select(_ => _.Insert.Count + _.Update.Count + _.Delete.Count).Sum()
                 : 0;
        }
    }
}