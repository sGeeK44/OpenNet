using System;
using NUnit.Framework;
using OpenNet.Orm.Attributes;
using OpenNet.Orm.Entity;
using OpenNet.Orm.Sql;
using OpenNet.Orm.Sql.Schema;
using OpenNet.Orm.Testkit;
// ReSharper disable UseStringInterpolation

namespace OpenNet.Orm.SqlCe.UnitTests.Entity
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
                    expectedTableDefinition.AddColumn("id", 1, false, "bigint");
                    expectedTableDefinition.AddColumn("BooleanField", 2, false, "bit");
                    expectedTableDefinition.AddColumn("ShortField", 3, true, "smallint");
                    expectedTableDefinition.AddColumn("IntegerField", 4, true, "int");
                    expectedTableDefinition.AddColumn("LongField", 5, true, "bigint");
                    expectedTableDefinition.AddColumn("FloatField", 6, true, "real");
                    expectedTableDefinition.AddColumn("DoubleField", 7, true, "float");
                    expectedTableDefinition.AddColumn("DecimalField", 8, true, "numeric");
                    expectedTableDefinition.AddColumn("DateTimeField", 9, true, "datetime");
                    expectedTableDefinition.AddColumn("StringField", 10, true, "nvarchar");
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
            return SqlCeFactory.CreateStore(datasource);
        }

        [Test]
        public void CreateTableDefinition()
        {
            var schemaChecker = new SqlCeSchemaChecker(DataStore);
            var tableDefinition = schemaChecker.GetTableFormat("EntityDefinition");

            Assert.AreEqual(EntityDefinition.TableDefinition, tableDefinition);
        }

        [Test]
        public void CreateTableDefinition_WithMaxLenght_FieldDataTypeShouldBeOne()
        {
            var maxLanght = AssertFieldDefinition("character_maximum_length", "StringField");
            Assert.AreEqual(1, maxLanght);
        }

        [Test]
        public void CreateTableDefinition_WithNumericScal_FieldDataTypeShouldBeOne()
        {
            var numScale = AssertFieldDefinition("numeric_precision", "DecimalField");
            Assert.AreEqual(1, numScale);
        }

        [Test]
        public void CreateTableDefinition_WithNumericPrecision_FieldDataTypeShouldBeOne()
        {
            var numPrecision = AssertFieldDefinition("numeric_scale", "DecimalField");
            Assert.AreEqual(1, numPrecision);
        }

        private object AssertFieldDefinition(string fieldProperty, string columname)
        {
            var sql = string.Format("SELECT {0} FROM information_schema.columns WHERE table_name = 'EntityDefinition' AND column_name = '{1}'", fieldProperty, columname);
            var result = DataStore.ExecuteReader(sql);
            result.Read();
            return result[0];
        }
    }
}