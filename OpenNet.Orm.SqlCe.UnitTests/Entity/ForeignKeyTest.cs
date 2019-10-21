using NUnit.Framework;
using OpenNet.Orm.Testkit;
using OpenNet.Orm.Testkit.Entities;

namespace OpenNet.Orm.SqlCe.UnitTests.Entity
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

        [Test]
        public void CreateTable_WithOneForeignKeyAttribute_ShouldCreateConstraint()
        {
            var schemaChecker = new SqlCeSchemaChecker(DataStore);
            Assert.IsTrue(schemaChecker.IsForeignKeyExist("ORM_FK_BookVersion_Book"));
        }
    }
}
