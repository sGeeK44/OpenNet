using NUnit.Framework;
using OpenNet.Orm.Sql;
using OpenNet.Orm.Testkit;
using OpenNet.Orm.Testkit.Entities;

namespace OpenNet.Orm.Sqlite.UnitTests.Entity
{
    [TestFixture]
    public class IndexesTest : DatastoreForTest
    {
        private SqliteSchemaChecker SchemaChecker { get; set; }

        [SetUp]
        public override void SetUp()
        {
            base.SetUp();
            SchemaChecker = new SqliteSchemaChecker(DataStore);
        }

        [TearDown]
        public override void CleanUp()
        {
            base.CleanUp();
        }

        protected override void AddTypes()
        {
            DataStore.AddType<IndexedClass>();
        }

        protected override ISqlDataStore CreateStore(string datasource)
        {
            return SqliteFactory.CreateStore(datasource);
        }

        [Test]
        public void CreateTable_WithOneIndexAttribute_ShouldCreateIndex()
        {
            AssertIndex("ORM_IDX_IndexedClass_MonIndex_ASC", 4);
            AssertIndex("ORM_IDX_IndexedClass_Searchable_ASC", 1);
            AssertIndex("ORM_IDX_IndexedClass_Unique_ASC", 1);
            AssertIndex("ORM_IDX_IndexedClass_SearchableAndUnique_ASC", 1);
        }

        private void AssertIndex(string indexName, int countFieldInvolve)
        {
            var fieldInvolve = SchemaChecker.IsIndexExist(indexName);
            Assert.AreEqual(fieldInvolve, countFieldInvolve);
        }
    }
}
