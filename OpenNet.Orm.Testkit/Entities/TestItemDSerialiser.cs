using System;
using System.Data;
using OpenNet.Orm.Caches;
using OpenNet.Orm.Entity.Serializers;
using OpenNet.Orm.Interfaces;

namespace OpenNet.Orm.Testkit.Entities
{
    public class TestItemDSerialiser : IEntitySerializer
    {
        public IEntityInfo Entity { get; set; }
        public IEntityCache EntityCache { get; set; }

        public TestItemDSerialiser(IEntityInfo entity, IEntityCache entityCache)
        {
            Entity = entity;
            EntityCache = entityCache;
        }

        public bool UseFullName { get; set; }

        public object Deserialize(IDataRecord dbResult)
        {

            var item = new TestItemD();

            for (var i = 0; i < Entity.Fields.Count; i++)
            {
                var field = Entity.Fields[i];
                var value = dbResult[i];

                switch (field.FieldName)
                {
                    case "ID":
                        item.ID = value == DBNull.Value ? 0 : (int)value;
                        break;
                    case "Name":
                        item.Name = value == DBNull.Value ? null : (string)value;
                        break;
                    case "UUID":
                        item.UUID = value == DBNull.Value ? null : (Guid?)value;
                        break;
                    case "ITest":
                        item.ITest = value == DBNull.Value ? 0 : (int)value;
                        break;
                    case "Address":
                        item.Address = value == DBNull.Value ? null : (string)value;
                        break;
                    case "FTest":
                        item.FTest = value == DBNull.Value ? 0 : (float)value;
                        break;
                    case "DBTest":
                        item.DBTest = value == DBNull.Value ? 0 : (double)value;
                        break;
                    case "DETest":
                        item.DETest = value == DBNull.Value ? 0 : (decimal)value;
                        break;
                    case "TestDate":
                        item.TestDate = value == DBNull.Value ? DateTime.MinValue : ((DateTime)value);
                        break;
                    case "BigString":
                        item.BigString = value == DBNull.Value ? null : (string)value;
                        break;
                }
            }

            return item;
        }

        public void PopulateFields(object item, IDataRecord dbResult)
        {
            throw new NotImplementedException();
        }
    }
}