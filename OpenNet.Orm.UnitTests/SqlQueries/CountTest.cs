using Moq;
using NUnit.Framework;
using OpenNet.Orm.Filters;
using OpenNet.Orm.Interfaces;
using OpenNet.Orm.Sql.Queries;

namespace OpenNet.Orm.UnitTests.SqlQueries
{
    [TestFixture]
    public class CountTest
    {
        [Test]
        public void CreateTableCount_ShouldReturnExpectedSqlString()
        {
            var select = Count.CreateTableCount();

            Assert.AreEqual("SELECT COUNT(*)", select.SelectStatement());
        }

        [Test]
        public void CreateTableCount_WithSpecifiedColumn_ShouldReturnExpectedString()
        {
            var entity = new Mock<IEntityInfo>();
            entity.Setup(_ => _.GetNameInStore()).Returns("X");
            var column = new ColumnValue(entity.Object, "Y");
            var select = Count.CreateTableCount(column);

            Assert.AreEqual("SELECT COUNT(*), [X].[Y]", select.SelectStatement());
        }

        [Test]
        public void CreateColumnCount_ShouldReturnExpectedString()
        {
            var entity = new Mock<IEntityInfo>();
            entity.Setup(_ => _.GetNameInStore()).Returns("X");
            var column = new ColumnValue(entity.Object, "Y");
            var select = Count.CreateColumnCount(column);

            Assert.AreEqual("SELECT COUNT([X].[Y])", select.SelectStatement());
        }
    }
}
