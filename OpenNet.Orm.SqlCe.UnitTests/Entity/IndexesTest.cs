using NUnit.Framework;
using OpenNet.Orm.Attributes;
using OpenNet.Orm.Constants;
using OpenNet.Orm.Entity.Constraints;
using OpenNet.Orm.Testkit;

namespace OpenNet.Orm.SqlCe.UnitTests.Entity
{
    [TestFixture]
    public class IndexesTest : DatastoreForTest
    {

        [SetUp]
        public override void SetUp()
        {
            base.SetUp();
        }

        [TearDown]
        public override void CleanUp()
        {
            base.CleanUp();
        }

        [Entity]
        public class IndexedClass
        {
            [PrimaryKey(KeyScheme.Identity, Indexes = new[] { "MonIndex" })]
            public int Id { get; set; }

            [Field(SearchOrder = FieldSearchOrder.Ascending, Indexes = new[] { "MonIndex" })]
            public string Searchable { get; set; }

            [Field(RequireUniqueValue = true, Indexes = new[] { "MonIndex" })]
            public string Unique { get; set; }

            [Field(SearchOrder = FieldSearchOrder.Ascending, RequireUniqueValue = true, Indexes = new[] { "MonIndex" })]
            public string SearchableAndUnique { get; set; }
        }

        [Test]
        public void CreateEntity_IndexShouldInitialize()
        {
            var entityInfo = EntityInfo.Create(FieldPropertyFactory, new EntityInfoCollection(), typeof(IndexedClass));

            Assert.AreEqual(4, entityInfo.Indexes.Count);
            Assert.AreEqual("ORM_IDX_IndexedClass_MonIndex_ASC", entityInfo.Indexes[0].GetNameInStore());
            Assert.AreEqual("ORM_IDX_IndexedClass_Searchable_ASC", entityInfo.Indexes[1].GetNameInStore());
            Assert.AreEqual("ORM_IDX_IndexedClass_Unique_ASC", entityInfo.Indexes[2].GetNameInStore());
            Assert.AreEqual("ORM_IDX_IndexedClass_SearchableAndUnique_ASC", entityInfo.Indexes[3].GetNameInStore());
        }

        [Test]
        public void CreateTable_WithOneIndexAttribute_ShouldCreateIndex()
        {
            var store = SqlCeFactory.CreateStore("CreateTable_WithIndexAttribute_ShouldCreateIndex.sdf");
            store.AddType<IndexedClass>();

            if (store.StoreExists)
                store.DeleteStore();

            store.CreateStore();

            var sql = "SELECT COUNT(*) FROM information_schema.indexes WHERE INDEX_NAME = \'ORM_IDX_IndexedClass_MonIndex_ASC\'";
            Assert.AreEqual(4, store.ExecuteScalar(sql));
            sql = "SELECT COUNT(*) FROM information_schema.indexes WHERE INDEX_NAME = \'ORM_IDX_IndexedClass_Searchable_ASC\'";
            Assert.AreEqual(1, store.ExecuteScalar(sql));
            sql = "SELECT COUNT(*) FROM information_schema.indexes WHERE INDEX_NAME = \'ORM_IDX_IndexedClass_Unique_ASC\'";
            Assert.AreEqual(1, store.ExecuteScalar(sql));
            sql = "SELECT COUNT(*) FROM information_schema.indexes WHERE INDEX_NAME = \'ORM_IDX_IndexedClass_SearchableAndUnique_ASC\'";
            Assert.AreEqual(1, store.ExecuteScalar(sql));
        }

        [Test]
        public void GetCreateSqlQuery_ShouldRightFormated()
        {
            var entityInfo = EntityInfo.Create(FieldPropertyFactory, new EntityInfoCollection(), typeof(IndexedClass));
            
            Assert.AreEqual("CREATE INDEX ORM_IDX_IndexedClass_MonIndex_ASC ON [IndexedClass] ([Id], [Searchable], [Unique], [SearchableAndUnique] ASC)", entityInfo.Indexes[0].GetCreateSqlQuery());
            Assert.AreEqual("CREATE INDEX ORM_IDX_IndexedClass_Searchable_ASC ON [IndexedClass] ([Searchable] ASC)", entityInfo.Indexes[1].GetCreateSqlQuery());
            Assert.AreEqual("CREATE UNIQUE INDEX ORM_IDX_IndexedClass_Unique_ASC ON [IndexedClass] ([Unique] ASC)", entityInfo.Indexes[2].GetCreateSqlQuery());
            Assert.AreEqual("CREATE UNIQUE INDEX ORM_IDX_IndexedClass_SearchableAndUnique_ASC ON [IndexedClass] ([SearchableAndUnique] ASC)", entityInfo.Indexes[3].GetCreateSqlQuery());
        }
    }
}
