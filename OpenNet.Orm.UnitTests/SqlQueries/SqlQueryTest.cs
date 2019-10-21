using System.Data;
using Moq;
using NUnit.Framework;
using OpenNet.Orm.Entity.Fields;
using OpenNet.Orm.Filters;
using OpenNet.Orm.Interfaces;
using OpenNet.Orm.Queries;
using OpenNet.Orm.Sql.Queries;
using OpenNet.Orm.Testkit.Entities;

namespace OpenNet.Orm.UnitTests.SqlQueries
{
    [TestFixture]
    public class SqlQueryTest
    {
        private const string Select = "FROM [Author]";
        private const string SelectJoin = "FROM [Author] JOIN [Book] ON [Author].[Id] = [Book].[AuthorId]";
        private const string SelectJoinChain = "FROM [Author] JOIN [Book] ON [Author].[Id] = [Book].[AuthorId] JOIN [BookVersion] ON [Book].[id] = [BookVersion].[BookId]";

        [SetUp]
        public void Init()
        {
            var entityCollection = new EntityInfoCollection();
            var factory = new Mock<IFieldPropertyFactory>();
            EntityInfo.Create(factory.Object, entityCollection, typeof(Author));
            EntityInfo.Create(factory.Object, entityCollection, typeof(Book));
            EntityInfo.Create(factory.Object, entityCollection, typeof(BookVersion));

            var dataStore = new Mock<IDataStore>();
            var sqlFactory = new Mock<ISqlFactory>();
            var param = new Mock<IDataParameter>();
            sqlFactory.Setup(_ => _.CreateParameter()).Returns(param.Object);
            dataStore.Setup(_ => _.SqlFactory).Returns(sqlFactory.Object);
            SqlQuery = new Selectable<Author>(dataStore.Object, entityCollection);
        }

        public Selectable<Author> SqlQuery { get; set; }

        [Test]
        public void ToStatement_SelectAll_ShouldReturnExpectedSqlString()
        {
            Assert.AreEqual(Select + ";", SqlQuery.ToStatement(null));
        }

        [Test]
        public void ToStatement_SelectWithWhere_ShouldReturnExpectedSqlString()
        {
            Assert.AreEqual(Select + ";", SqlQuery.ToStatement(null));
        }

        [Test]
        public void ToStatement_SelectJoin_ShouldReturnExpectedSqlString()
        {
            SqlQuery.Join<Author, Book>();
            Assert.AreEqual(SelectJoin + ";", SqlQuery.ToStatement(null));
        }

        [Test]
        public void ToStatement_SelectWithWhereAndOrderBy_ShouldReturnExpectedSqlString()
        {
            var sqlOrderBySql = SetupOrderByCondition();
            Assert.AreEqual(Select + sqlOrderBySql + ";", SqlQuery.ToStatement(null));
        }

        [Test]
        public void ToStatement_SelectWithWhereAndOrderByDesc_ShouldReturnExpectedSqlString()
        {
            var sqlOrderByDescSql = SetupOrderByDescCondition();
            Assert.AreEqual(Select + sqlOrderByDescSql + ";", SqlQuery.ToStatement(null));
        }

        [Test]
        public void ToStatement_SelectWithMultipleJoin_ShouldReturnExpectedSqlString()
        {
            SqlQuery.Join<Author, Book>().Join<Book, BookVersion>();
            Assert.AreEqual(SelectJoinChain + ";", SqlQuery.ToStatement(null));
        }

        private string SetupOrderByCondition()
        {
            const string entityName = "XX";
            var entity = new Mock<IEntityInfo>();
            entity.Setup(_ => _.GetNameInStore()).Returns(entityName);
            const string field1Value = "A";
            const string field2Value = "B";
            var field1 = new ColumnValue(entity.Object, field1Value);
            var field2 = new ColumnValue(entity.Object, field2Value);
            SqlQuery.OrderBy(field1).ThenBy(field2);
            return " ORDER BY [" + entityName + "].[" + field1Value + "], [" + entityName + "].[" + field2Value + "]";
        }

        private string SetupOrderByDescCondition()
        {
            const string entityName = "XX";
            var entity = new Mock<IEntityInfo>();
            entity.Setup(_ => _.GetNameInStore()).Returns(entityName);
            const string field1Value = "A";
            const string field2Value = "B";
            var field1 = new ColumnValue(entity.Object, field1Value);
            var field2 = new ColumnValue(entity.Object, field2Value);
            SqlQuery.OrderByDesc(field1).ThenByDesc(field2);
            return " ORDER BY [" + entityName + "].[" + field1Value + "] DESC, [" + entityName + "].[" + field2Value + "] DESC";
        }
    }
}
