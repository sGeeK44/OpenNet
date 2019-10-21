using Moq;
using NUnit.Framework;
using OpenNet.Orm.Entity.Constraints;
using OpenNet.Orm.Entity.Fields;
using OpenNet.Orm.Sync.Entity;
using OpenNet.Orm.Testkit.Entities;

namespace OpenNet.Orm.Sync.UnitTests
{
    [TestFixture]
    public class EntityInfoTest
    {
        [Test]
        public void CreateTombstone_PrimaryKeyShouldHaveKeySchemToNone()
        {
            var entities = new EntityInfoCollection();
            var factory = new Mock<IFieldPropertyFactory>();
            var entityInfo = EntityInfo.Create(factory.Object, entities, typeof(EntitySync));
            var entitySync = SyncEntity.Create(entityInfo);
            var tombstoneEntityInfo = entitySync.CreateEntityTombstone();

            Assert.AreEqual(KeyScheme.None , tombstoneEntityInfo.PrimaryKey.KeyScheme);
        }
    }
}
