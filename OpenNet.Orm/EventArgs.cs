using System;
using OpenNet.Orm.Interfaces;

namespace OpenNet.Orm
{
    public class EntityTypeAddedArgs : EventArgs
    {
        internal EntityTypeAddedArgs(IEntityInfo info) 
        {
            EntityInfo = info;
        }

        public IEntityInfo EntityInfo { get; set; }
    }

    public class EntityUpdateArgs : EventArgs
    {
        internal EntityUpdateArgs(string entityName, object item, string fieldName) 
        {
            EntityName = entityName;
            Item = item;
            FieldName = fieldName;
        }

        public string EntityName { get; set; }
        public object Item { get; set; }
        public string FieldName { get; set; }
    }

    public class EntityInsertArgs : EventArgs
    {
        internal EntityInsertArgs(string entityName, object item)
        {
            EntityName = entityName;
            Item = item;
        }

        public string EntityName { get; set; }
        public object Item { get; set; }
    }

    public class EntityDeleteArgs : EventArgs
    {
        internal EntityDeleteArgs(string entityName, object item)
        {
            EntityName = entityName;
            Item = item;
        }

        public string EntityName { get; set; }
        public object Item { get; set; }
    }
}
