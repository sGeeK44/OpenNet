using System;
using NUnit.Framework;
using OpenNet.Orm.Attributes;
using OpenNet.Orm.Entity;
using OpenNet.Orm.Sql;
using OpenNet.Orm.Sql.Schema;
using OpenNet.Orm.Testkit;

// ReSharper disable UseStringInterpolation

namespace OpenNet.Orm.Sqlite.UnitTests.Entity
{
    [TestFixture]
    public class TableDefinitionTest : DatastoreForTest
    {
        [Entity]
        public class EntityDefinition : EntityBase<EntityDefinition>
        {
            [Field(AllowsNulls = false)]
            public bool BooleanField { get; set; }

            [Field(AllowsNulls = true)]
            public short? ShortField { get; set; }

            [Field]
            public int IntegerField { get; set; }

            [Field]
            public long LongField { get; set; }

            [Field]
            public float FloatField { get; set; }

            [Field]
            public double DoubleField { get; set; }

            [Field(Precision = 1, Scale = 1)]
            public decimal DecimalField { get; set; }

            [Field]
            public DateTime DateTimeField { get; set; }

            [Field(Length = 1)]
            public string StringField { get; set; }

            public static TableDefinition TableDefinition
            {
                get
                {
                    var expectedTableDefinition = new TableDefinition("EntityDefinition");
                    expectedTableDefinition.AddColumn("id", 0, false, "INTEGER");
                    expectedTableDefinition.AddColumn("BooleanField", 1, false, "INTEGER");
                    expectedTableDefinition.AddColumn("ShortField", 2, true, "INTEGER");
                    expectedTableDefinition.AddColumn("IntegerField", 3, true, "INTEGER");
                    expectedTableDefinition.AddColumn("LongField", 4, true, "INTEGER");
                    expectedTableDefinition.AddColumn("FloatField", 5, true, "REAL");
                    expectedTableDefinition.AddColumn("DoubleField", 6, true, "REAL");
                    expectedTableDefinition.AddColumn("DecimalField", 7, true, "NUMERIC");
                    expectedTableDefinition.AddColumn("DateTimeField", 8, true, "TEXT");
                    expectedTableDefinition.AddColumn("StringField", 9, true, "TEXT");
                    return expectedTableDefinition;
                }
            }
        }

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
            DataStore.AddType<EntityDefinition>();
        }

        protected override ISqlDataStore CreateStore(string datasource)
        {
            return SqliteFactory.CreateStore(datasource);
        }

        [Test]
        public void CreateTableDefinition()
        {
            var schemaChecker = new SqliteSchemaChecker(DataStore);
            var tableDefinition = schemaChecker.GetTableFormat("EntityDefinition");

            Assert.AreEqual(EntityDefinition.TableDefinition, tableDefinition);
        }
    }
}