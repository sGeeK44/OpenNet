using System;
using System.Data;
using OpenNet.Orm.Caches;
using OpenNet.Orm.Entity.Serializers;
using OpenNet.Orm.Interfaces;

namespace OpenNet.Orm.Testkit.Entities
{
    public class AuthorSerializer : IEntitySerializer
    {
        public IEntityInfo Entity { get; set; }
        public IEntityCache EntityCache { get; set; }

        public AuthorSerializer(IEntityInfo entity)
        {
            Entity = entity;
        }

        public bool UseFullName { get; set; }

        public object Deserialize(IDataRecord dbResult)
        {
            var item = new Author();

            for (int i = 0; i < Entity.Fields.Count; i++)
            {
                var field = Entity.Fields[i];
                var value = dbResult[UseFullName ? field.AliasFieldName : field.FieldName];
                // ReSharper disable once UnusedVariable
                var val = dbResult[i];

                switch (field.FieldName)
                {
                    case "Name":
                        item.Name = value == DBNull.Value ? null : (string)value;
                        break;
                        // fill in any additional properties here
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