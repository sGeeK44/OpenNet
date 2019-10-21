using NUnit.Framework;
using OpenNet.Orm.Sql;
using OpenNet.Orm.Testkit;
using OpenNet.Orm.Testkit.Entities;

namespace OpenNet.Orm.Sqlite.UnitTests.Entity
{
    [TestFixture]
    public class ForeignKeyTest : DatastoreForTest
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

        protected override void AddTypes()
        {
            DataStore.AddType<Book>();
            DataStore.AddType<BookVersion>();
        }

        protected override ISqlDataStore CreateStore(string datasource)
        {
            return SqliteFactory.CreateStore(datasource);
        }

        [Test]
        public void CreateTable_WithOneForeignKeyAttribute_ShouldCreateConstraint()
        {
            var schemaChecker = new SqliteSchemaChecker(DataStore);
            Assert.IsTrue(schemaChecker.IsForeignKeyExist("BookVersion", "Book"));
        }
    }
}
