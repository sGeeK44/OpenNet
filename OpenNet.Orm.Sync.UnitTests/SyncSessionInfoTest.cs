using System;
using NUnit.Framework;

namespace OpenNet.Orm.Sync.UnitTests
{
    [TestFixture]
    public class SyncSessionInfoTest : SyncOneRemoteTest
    {
        protected override void Setup()
        {
            base.Setup();
            SyncSessionRepository = new SyncSessionInfoRepository(Desktop.DataStore);
        }

        public SyncSessionInfoRepository SyncSessionRepository { get; set; }

        [Test]
        public void Create_FirstSync_LowBoundShouldEqualMinSyncDate()
        {
            var result = SyncSessionInfo.Create(Desktop.DataStore, SyncDateTimeProvider);

            Assert.AreEqual(new DateTime(1973, 1, 1, 0, 0, 0, DateTimeKind.Utc), result.LowBoundaryAnchor);
        }

        [Test]
        public void Create_NextSync_LowBoundShouldEqualHighOfLastSuccess()
        {
            var last = SyncSessionInfo.Create(Desktop.DataStore, SyncDateTimeProvider);
            last.HasSuccess = true;
            SyncSessionRepository.Save(last);

            SomeTimeLater();
            var failed = SyncSessionInfo.Create(Desktop.DataStore, SyncDateTimeProvider);
            SyncSessionRepository.Save(failed);

            SomeTimeLater();
            var result = SyncSessionInfo.Create(Desktop.DataStore, SyncDateTimeProvider);

            Assert.AreEqual(last.HighBoundaryAnchor, result.LowBoundaryAnchor);
        }

        [Test]
        public void Create_SecondSync_LowBoundShouldEqualHighOfLastSuccess()
        {
            var last = SyncSessionInfo.Create(Desktop.DataStore, SyncDateTimeProvider);
            last.HasSuccess = true;
            SyncSessionRepository.Save(last);

            SomeTimeLater();
            last = SyncSessionInfo.Create(Desktop.DataStore, SyncDateTimeProvider);
            last.HasSuccess = true;
            SyncSessionRepository.Save(last);

            SomeTimeLater();
            var result = SyncSessionInfo.Create(Desktop.DataStore, SyncDateTimeProvider);

            Assert.AreEqual(last.HighBoundaryAnchor, result.LowBoundaryAnchor);
        }
    }
}
