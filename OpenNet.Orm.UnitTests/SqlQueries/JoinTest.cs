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
    public class JoinTest
    {
        private const string SelectJoin = "JOIN [Book] ON [Author].[Id] = [Book].[AuthorId]";

        [SetUp]
        public void Init()
        {
            var entityCollection = new EntityInfoCollection();
            var factory = new Mock<IFieldPropertyFactory>();
            Author = EntityInfo.Create(factory.Object, entityCollection, typeof(Author));
            Book = EntityInfo.Create(factory.Object, entityCollection, typeof(Book));
            BookVersion = EntityInfo.Create(factory.Object, entityCollection, typeof(BookVersion));

            var dataStore = new Mock<IDataStore>();
            var sqlFactory = new Mock<ISqlFactory>();
            var param = new Mock<IDataParameter>();
            sqlFactory.Setup(_ => _.CreateParameter()).Returns(param.Object);
            dataStore.Setup(_ => _.SqlFactory).Returns(sqlFactory.Object);
            SqlQuery = new Selectable<Author>(dataStore.Object, entityCollection);
        }

        public IEntityInfo BookVersion { get; set; }

        public IEntityInfo Book { get; set; }

        public IEntityInfo Author { get; set; }

        public Selectable<Author> SqlQuery { get; set; }

        [Test]
        public void ToStatement_JoinOnOneToMany_ShouldReturnExpectedSqlString()
        {
            var join = new Join(Author, Book);

            Assert.AreEqual(SelectJoin, join.ToStatement(null));
        }
    }
}
