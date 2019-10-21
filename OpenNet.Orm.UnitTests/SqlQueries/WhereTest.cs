using System.Collections.Generic;
using System.Data;
using Moq;
using NUnit.Framework;
using OpenNet.Orm.Entity.Fields;
using OpenNet.Orm.Filters;
using OpenNet.Orm.Interfaces;
using OpenNet.Orm.Queries;
using OpenNet.Orm.Sql.Queries;
using OpenNet.Orm.Testkit.Entities;

// ReSharper disable MergeConditionalExpression

namespace OpenNet.Orm.UnitTests.SqlQueries
{
    [TestFixture]
    public class WhereTest
    {
        [SetUp]
        public void Init()
        {
            Entities = new EntityInfoCollection();
            var factory = new Mock<IFieldPropertyFactory>();
            Entity = EntityInfo.Create(factory.Object, Entities, typeof(Author));
        }

        private IEntityInfo Entity { get; set; }
        private EntityInfoCollection Entities { get; set; }

        [Test]
        public void ToStatement_Empty_ShouldReturnExpectedSql()
        {
            var where = new Where();

            var @params = new List<IDataParameter>();
            Assert.AreEqual(string.Empty, where.ToStatement(@params));
            Assert.AreEqual(0, @params.Count);
        }

        [Test]
        public void ToStatement_OneFilter_ShouldReturnExpectedSqlString()
        {
            const string paramValue = "XXX";
            var @params = new List<IDataParameter>();
            var sqlFactory = new Mock<ISqlFactory>();
            sqlFactory.Setup(_ => _.AddParam(1, @params)).Returns(paramValue);
            var filterFactory = new FilterFactory(sqlFactory.Object, Entities);
            var primaryKey = filterFactory.ToColumnValue(Entity, Author.PrimaryKeyName);
            var value = filterFactory.ToObjectValue(1);
            var filter = filterFactory.Equal(primaryKey, value);
            var where = new Where(filter);

            var result = where.ToStatement(@params);

            Assert.AreEqual($" WHERE [Author].[Id] = {paramValue}", result);
        }
    }
}
