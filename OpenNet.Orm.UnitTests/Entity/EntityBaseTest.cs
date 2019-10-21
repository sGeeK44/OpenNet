using System.Collections.Generic;
using System.Linq;
using Moq;
using NUnit.Framework;
using OpenNet.Orm.Entity;
using OpenNet.Orm.Entity.Fields;
using OpenNet.Orm.Repositories;
using OpenNet.Orm.Sql;
using OpenNet.Orm.Testkit.Entities;

namespace OpenNet.Orm.UnitTests.Entity
{
    [TestFixture]
    public class EntityBaseTest
    {
        private Mock<ISqlDataStore> Store { get; set; }

        private IRepository<ConcreteEntity> Repository { get; set; }

        private Mock<IOrmObserver> Observer { get; set; }

        [SetUp]
        public void Setup()
        {
            Store = new Mock<ISqlDataStore>();
            Observer = new Mock<IOrmObserver>();
            Repository = new Repository<ConcreteEntity, ConcreteEntity>(Store.Object);
        }

        [Test]
        public void Save_NoObserver_ShouldDoNoThrowException()
        {
            var entity = CreateInitializedEntity();

            entity.Save();
        }

        [Test]
        public void Delete_NoObserver_ShouldDoNoThrowException()
        {
            var entity = CreateInitializedEntity();

            entity.Save();
            entity.Delete();
        }

        [Test]
        public void Save_OneObserver_ObserverShouldBeNotified()
        {
            var entity = CreateInitializedEntity();
            var observer = new Mock<IEntityObserver>();
            observer.Setup(_ => _.Notify(EntityEvent.Saved, entity));
            entity.Subscribe(observer.Object);

            entity.Save();

            observer.VerifyAll();
        }

        [Test]
        public void Delete_OneObserver_ObserverShouldBeNotified()
        {
            var entity = CreateInitializedEntity();
            entity.Save();

            var observer = new Mock<IEntityObserver>();
            observer.Setup(_ => _.Notify(EntityEvent.Deleted, entity));

            entity.Subscribe(observer.Object);
            entity.Delete();

            observer.VerifyAll();
        }

        [Test]
        public void RemoveObserver_OneObserver_ObserverShouldNotBeNotifiedAnyMore()
        {
            var entity = CreateInitializedEntity();
            var observer = new Mock<IEntityObserver>();
            entity.Subscribe(observer.Object);

            entity.Unsubscribe(observer.Object);

            entity.Save();
            entity.Delete();
            observer.Verify(_ => _.Notify(EntityEvent.Saved, entity), Times.Never());
            observer.Verify(_ => _.Notify(EntityEvent.Deleted, entity), Times.Never());
        }

        [Test]
        public void RefreshFromDb_ShouldGetAllNewValueFromDatastore()
        {
            var entity = CreateInitializedEntity();
            entity.Name = "New";
            Store.Setup(_ => _.Select<ConcreteEntity>(It.IsAny<long>())).Returns(entity);

            var same = CreateInitializedEntity();
            same.RefreshFromDb();

            Assert.AreEqual(entity.Name, same.Name);
        }

        [Test]
        public void RemoveAllObserver_TwoObserver_ObserverShouldNotBeNotifiedAnyMore()
        {
            var entity = CreateInitializedEntity();
            var observer1 = new Mock<IEntityObserver>();
            var observer2 = new Mock<IEntityObserver>();
            entity.Subscribe(observer1.Object);
            entity.Subscribe(observer2.Object);

            entity.UnsubscribeAll();

            entity.Save();
            entity.Delete();
            observer1.Verify(_ => _.Notify(EntityEvent.Saved, entity), Times.Never());
            observer2.Verify(_ => _.Notify(EntityEvent.Saved, entity), Times.Never());
            observer1.Verify(_ => _.Notify(EntityEvent.Deleted, entity), Times.Never());
            observer2.Verify(_ => _.Notify(EntityEvent.Deleted, entity), Times.Never());
        }

        [Test]
        public void DeleteByBundle_DeleteEntitiesAndReturnProgress()
        {
            var bundleEntities = CreateBundleOfEntities(50).ToList();

            Repository.DeleteByBundle(bundleEntities, 20, Observer.Object);

            Store.Verify(_ => _.DeleteBulk<ConcreteEntity, ConcreteEntity>(It.IsAny<List<ConcreteEntity>>()), Times.Exactly(3));
            Observer.Verify(_ => _.ReportProgess(0));
            Observer.Verify(_ => _.ReportProgess(40));
            Observer.Verify(_ => _.ReportProgess(80));
            Observer.Verify(_ => _.ReportProgess(100));
        }

        [Test]
        public void CreateEntity_PrimaryKeyShouldInitialize()
        {
            var entities = new EntityInfoCollection();
            var factory = new Mock<IFieldPropertyFactory>();
            var entityInfo = EntityInfo.Create(factory.Object, entities, typeof(Book));

            Assert.IsNotNull(entityInfo.PrimaryKey);
            Assert.AreEqual("ORM_PK_Book", entityInfo.PrimaryKey.ConstraintName);
        }

        [Test]
        public void CreateEntity_ForeignKeyShouldInitialize()
        {
            var entities = new EntityInfoCollection();
            var factory = new Mock<IFieldPropertyFactory>();
            EntityInfo.Create(factory.Object, entities, typeof(Book));
            var entityInfo = EntityInfo.Create(factory.Object, entities, typeof(BookVersion));

            Assert.AreEqual(1, entityInfo.ForeignKeys.Count);
            Assert.AreEqual("ORM_FK_BookVersion_Book", entityInfo.ForeignKeys[0].ConstraintName);
        }

        [Test]
        public void CreateEntity_IndexShouldInitialize()
        {
            var factory = new Mock<IFieldPropertyFactory>();
            var entityInfo = EntityInfo.Create(factory.Object, new EntityInfoCollection(), typeof(IndexedClass));

            Assert.AreEqual(4, entityInfo.Indexes.Count);
            Assert.AreEqual("ORM_IDX_IndexedClass_MonIndex_ASC", entityInfo.Indexes[0].GetNameInStore());
            Assert.AreEqual("ORM_IDX_IndexedClass_Searchable_ASC", entityInfo.Indexes[1].GetNameInStore());
            Assert.AreEqual("ORM_IDX_IndexedClass_Unique_ASC", entityInfo.Indexes[2].GetNameInStore());
            Assert.AreEqual("ORM_IDX_IndexedClass_SearchableAndUnique_ASC", entityInfo.Indexes[3].GetNameInStore());
        }

        [Test]
        public void GetCreateSqlQuery_ShouldRightFormated()
        {
            var factory = new Mock<IFieldPropertyFactory>();
            var entityInfo = EntityInfo.Create(factory.Object, new EntityInfoCollection(), typeof(IndexedClass));

            Assert.AreEqual("CREATE INDEX ORM_IDX_IndexedClass_MonIndex_ASC ON [IndexedClass] ([Id], [Searchable], [Unique], [SearchableAndUnique] ASC)", entityInfo.Indexes[0].GetCreateSqlQuery());
            Assert.AreEqual("CREATE INDEX ORM_IDX_IndexedClass_Searchable_ASC ON [IndexedClass] ([Searchable] ASC)", entityInfo.Indexes[1].GetCreateSqlQuery());
            Assert.AreEqual("CREATE UNIQUE INDEX ORM_IDX_IndexedClass_Unique_ASC ON [IndexedClass] ([Unique] ASC)", entityInfo.Indexes[2].GetCreateSqlQuery());
            Assert.AreEqual("CREATE UNIQUE INDEX ORM_IDX_IndexedClass_SearchableAndUnique_ASC ON [IndexedClass] ([SearchableAndUnique] ASC)", entityInfo.Indexes[3].GetCreateSqlQuery());
        }

        private ConcreteEntity CreateInitializedEntity()
        {
            var result = new ConcreteEntity { Id = 1 };
            result.SetRepository(Repository);
            return result;
        }

        private IEnumerable<ConcreteEntity> CreateBundleOfEntities(int bundleSize)
        {
            var bundle = new List<ConcreteEntity>();
            for (var i = 0; i < bundleSize; i++)
            {
                var entity = new ConcreteEntity {Id = i, Repository = Repository};
                entity.Save();
                bundle.Add(entity);
            }

            return bundle;
        }
    }
}
