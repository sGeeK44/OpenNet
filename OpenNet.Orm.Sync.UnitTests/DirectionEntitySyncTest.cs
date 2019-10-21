using NUnit.Framework;
using OpenNet.Orm.Testkit.Entities;

namespace OpenNet.Orm.Sync.UnitTests
{
    [TestFixture]
    public class DirectionEntitySyncTest : SyncOneRemoteTest
    {
        protected override void Setup()
        {
            base.Setup();
            AddEntityType<DownloadOnlyEntitySync, DownloadOnlyEntitySync>();
            AddEntityType<UpdloadOnlyEntitySync, UpdloadOnlyEntitySync>();
        }

        [Test]
        public void SomeChangeOnServer_ShouldBeDownloaded()
        {
            Desktop.InsertNewEntity<DownloadOnlyEntitySync, DownloadOnlyEntitySync>();

            SyncRemote();

            AssertSyncRepository(Desktop.Repository<DownloadOnlyEntitySync, DownloadOnlyEntitySync>(), FirstRemote.Repository<DownloadOnlyEntitySync, DownloadOnlyEntitySync>());
        }

        [Test]
        public void SomeChangeOnRemote_ShouldNotBeUploaded()
        {
            FirstRemote.InsertNewEntity<DownloadOnlyEntitySync, DownloadOnlyEntitySync>();

            SyncRemote();

            Assert.AreEqual(0, Desktop.Repository<DownloadOnlyEntitySync, DownloadOnlyEntitySync>().Count());
        }

        [Test]
        public void SomeChangeOnServer_ShouldNotBeDownloaded()
        {
            Desktop.InsertNewEntity<UpdloadOnlyEntitySync, UpdloadOnlyEntitySync>();

            SyncRemote();

            Assert.AreEqual(0, FirstRemote.Repository<UpdloadOnlyEntitySync, UpdloadOnlyEntitySync>().Count());
        }

        [Test]
        public void SomeChangeOnRemote_ShouldBeUploaded()
        {
            FirstRemote.InsertNewEntity<UpdloadOnlyEntitySync, UpdloadOnlyEntitySync>();

            SyncRemote();

            AssertSyncRepository(Desktop.Repository<UpdloadOnlyEntitySync, UpdloadOnlyEntitySync>(), FirstRemote.Repository<UpdloadOnlyEntitySync, UpdloadOnlyEntitySync>());
        }
    }
}