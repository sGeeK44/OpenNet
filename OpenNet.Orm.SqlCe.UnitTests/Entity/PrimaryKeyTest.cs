using NUnit.Framework;
using OpenNet.Orm.Testkit.Entities;

namespace OpenNet.Orm.SqlCe.UnitTests.Entity
{
    [TestFixture]
    public class PrimaryKeyTest
    {
        [Test]
        public void CreateTable_WithOnePrimaryKeyAttribute_ShouldCreateConstraint()
        {
            var store = SqlCeFactory.CreateStore("CreateTable_WithOnePrimaryKeyAttribute_ShouldCreateConstraint.sdf");
            store.AddType<Book>();

            if (store.StoreExists)
                store.DeleteStore();

            store.CreateStore();

            var sql = "SELECT COUNT(*) FROM information_schema.table_constraints WHERE CONSTRAINT_NAME = \'ORM_PK_Book\'";
            Assert.AreEqual(1, store.ExecuteScalar(sql));
        }
    }
}