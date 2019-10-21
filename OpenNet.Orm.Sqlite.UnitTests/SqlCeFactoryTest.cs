using System.Collections.Generic;
using System.Data;
using NUnit.Framework;

namespace OpenNet.Orm.Sqlite.UnitTests
{
    [TestFixture]
    public class SqlCeFactoryTest
    {
        [Test]
        public void ToStatement_OneFilter_ShouldReturnExpectedSqlString()
        {
            var factory = new SqliteFactory();
            var @params = new List<IDataParameter>();

            var paramValue = factory.AddParam(1, @params);

            Assert.AreEqual("@p0", paramValue);
            Assert.AreEqual(1, @params.Count);
        }
    }
}
