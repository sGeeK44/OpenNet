using System;
using System.Collections.Generic;
using System.Threading;
using Moq;
using OpenNet.Orm.Entity;
using OpenNet.Orm.Sync;
using OpenNet.Orm.Sync.Agents;
using OpenNet.Orm.Sync.Entity;

namespace OpenNet.Orm.Testkit
{
    public class Synchronizator
    {
        private readonly ISyncableActorsFactory _factory;
        private readonly IDateTimeSynchronizer _dateTimeSynchronizer;

        public Synchronizator(ISyncableActorsFactory factory):this(factory, new DateTimeSynchronizer())
        {
        }

        public Synchronizator(ISyncableActorsFactory factory, IDateTimeSynchronizer dateTimeSynchronizer)
        {
            _factory = factory;
            _dateTimeSynchronizer = dateTimeSynchronizer;
            _dateTimeSynchronizer.SetNewDate(new DateTime(2017, 03, 13, 08, 47, 23));
            InitSyncable();
        }

        public DateTime Now { get; set; }
        public SyncableServer Desktop { get; set; }
        public List<SyncableClient> Remotes { get; set; }
        public IDateTimeManager DateTimeManager { get { return _dateTimeSynchronizer.DateTimeManager; } }

        /// <summary>
        /// Add multiple remote to the sync manager
        /// </summary>
        /// <param name="numberOfRemote"></param>
        public void AddMultipleRemote(int numberOfRemote)
        {
            for (var i = 0; i < numberOfRemote; i++)
            {
                AddRemote();
            }
        }

        /// <summary>
        /// Add a remote to synchronization manager
        /// </summary>
        public void AddRemote()
        {
            var newRemote = CreateRemote("remote " + Remotes.Count + 1);
            Remotes.Add(newRemote);
        }

        public void SaveOnAllRemotes<TEntity, TIEntity>(TIEntity entity)
            where TEntity : SyncableEntity<TIEntity>, TIEntity, new()
            where TIEntity : class, IDistinctableEntity
        {
            Remotes.ForEach(remote => remote.Save<TEntity, TIEntity>(entity));
        }

        /// <summary>
        /// Get specific remote
        /// </summary>
        /// <param name="remoteId">Remote id (Index start at 0)</param>
        public SyncableClient GetRemote(int remoteId)
        {
            if (remoteId < 0 || remoteId >= Remotes.Count)
                return null;

            return Remotes[remoteId];
        }

        /// <summary>
        /// Synchronize specific remote by it id
        /// </summary>
        /// <param name="remoteId">Remote id (Index start at 0)</param>
        public void SyncRemote(int remoteId)
        {
            var remote = GetRemote(remoteId);
            if (remote == null)
                return;

            OrmDebug.Info(string.Format("Synchronize Desktop with remote {0}.", remoteId));
            SyncRemote(remote);
        }

        public void Clean()
        {
            Desktop.Dispose();
            Remotes.ForEach(remote=>remote.Dispose());
            Remotes.Clear();
        }

        private void InitSyncable()
        {
            Remotes = new List<SyncableClient>();
            Desktop = CreateServer("desktop");
        }

        private void SyncRemote(SyncableClient remote)
        {
            InitTransportLayer(Desktop.Transport, remote.Transport);
            SyncStore(Desktop.Agent, remote.Agent);

            if (Desktop.Agent.SyncType != SyncTypes.OneWay)
                return;

            remote.BuildFromDb(Desktop);
            SyncAgent.InitSyncSessionInfo(remote.DataStore, DateTimeManager);
        }

        private void InitTransportLayer(LocalBoundTransport desktopTransport, LocalBoundTransport remoteTransport)
        {
            remoteTransport.Pair = desktopTransport;
            desktopTransport.Pair = remoteTransport;
        }

        private void SyncStore(ServerSyncAgent server, ClientSyncAgent client)
        {
            SomeTimeLater();
            var observer = new Mock<IOrmSyncObserver>();
            server.Synchronise(observer.Object);
            client.Synchronize(observer.Object);
            while (server.IsSyncInProgress) Thread.Sleep(500);
            SomeTimeLater();
        }

        public void SomeTimeLater()
        {
            AddTime(0, 0, 0, 0, 0, 5);
        }

        public void SomeDayLater(int days)
        {
            AddTime(0, 0, days, 0, 0, 0);
        }

        private void AddTime(int years, int months, int days, int hours, int minutes, int seconds)
        {
            var newDate = DateTimeManager.UtcNow.AddYears(years).AddMonths(months).AddDays(days).AddHours(hours).AddMinutes(minutes).AddSeconds(seconds);
            _dateTimeSynchronizer.SetNewDate(newDate);
        }

        private SyncableClient CreateRemote(string name)
        {
            var remote = _factory.CreateClient(DateTimeManager, name);
            remote.SetUp();
            return remote;
        }

        private SyncableServer CreateServer(string name)
        {
            var remote = _factory.CreateServer(DateTimeManager, name);
            remote.SetUp();
            return remote;
        }
    }
}
