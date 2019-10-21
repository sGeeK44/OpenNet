using System.Data;
using Moq;
using NUnit.Framework;
using OpenNet.Orm.Entity.Fields;
using OpenNet.Orm.Interfaces;
using OpenNet.Orm.Queries;
using OpenNet.Orm.Sql.Queries;
using OpenNet.Orm.Testkit.Entities;

namespace OpenNet.Orm.UnitTests.SqlQueries
{
    [TestFixture]
    public class SelectTest
    {
        private const string Select = "SELECT [Author].[Id] AS AuthorId, [Author].[Name] AS AuthorName FROM [Author];";
        private const string SelectJoin = "SELECT [Author].[Id] AS AuthorId, [Author].[Name] AS AuthorName, [Book].[id] AS Bookid, [Book].[AuthorId] AS BookAuthorId, [Book].[Title] AS BookTitle, [Book].[BookType] AS BookBookType, [Book].[RowVersion] AS BookRowVersion FROM [Author] JOIN [Book] ON [Author].[Id] = [Book].[AuthorId];";
        private const string SelectJoinChain = "SELECT [Author].[Id] AS AuthorId, [Author].[Name] AS AuthorName, [Book].[id] AS Bookid, [Book].[AuthorId] AS BookAuthorId, [Book].[Title] AS BookTitle, [Book].[BookType] AS BookBookType, [Book].[RowVersion] AS BookRowVersion, [BookVersion].[id] AS BookVersionid, [BookVersion].[BookId] AS BookVersionBookId FROM [Author] JOIN [Book] ON [Author].[Id] = [Book].[AuthorId] JOIN [BookVersion] ON [Book].[id] = [BookVersion].[BookId];";
        private const string SelectTop = "SELECT TOP(10) [Author].[Id] AS AuthorId, [Author].[Name] AS AuthorName FROM [Author];";

        [SetUp]
        public void Init()
        {
            EntityCollection = new EntityInfoCollection();
            var factory = new Mock<IFieldPropertyFactory>();
            EntityInfo.Create(factory.Object, EntityCollection, typeof(Author));
            EntityInfo.Create(factory.Object, EntityCollection, typeof(Book));
            EntityInfo.Create(factory.Object, EntityCollection, typeof(BookVersion));

            DataStore = new Mock<IDataStore>();
            SqlFactory = new Mock<ISqlFactory>();
            var param = new Mock<IDataParameter>();
            SqlFactory.Setup(_ => _.CreateParameter()).Returns(param.Object);
            DataStore.Setup(_ => _.SqlFactory).Returns(SqlFactory.Object);
            SqlQuery = new Selectable<Author>(DataStore.Object, EntityCollection);
        }

        private EntityInfoCollection EntityCollection { get; set; }
        private Mock<ISqlFactory> SqlFactory { get; set; }
        private Mock<IDataStore> DataStore { get; set; }
        private Selectable<Author> SqlQuery { get; set; }

        [Test]
        public void SelectStatement_SelectAll_ShouldReturnExpectedSqlString()
        {
            SqlQuery.GetValues();
            Assert.AreEqual(Select, SqlQuery.ToStatement(null));
        }

        [Test]
        public void SelectStatement_SelectJoin_ShouldReturnExpectedSqlString()
        {
            SqlQuery.Join<Author, Book>();

            SqlQuery.GetValues();
            Assert.AreEqual(SelectJoin, SqlQuery.ToStatement(null));
        }

        [Test]
        public void SelectStatement_SelectWithMultipleJoin_ShouldReturnExpectedSqlString()
        {
            SqlQuery.Join<Author, Book>().Join<Book, BookVersion>();

            SqlQuery.GetValues();
            Assert.AreEqual(SelectJoinChain, SqlQuery.ToStatement(null));
        }

        [Test]
        public void SelectTopStatement_ShouldReturnExpectedSqlString()
        {
            var sqlQuery = new Selectable<Author>(DataStore.Object, EntityCollection);
            sqlQuery.Top(10);

            Assert.AreEqual(SelectTop, sqlQuery.ToStatement(null));
        }
    }
}
