using System;

namespace OpenNet.Orm.Attributes
{
    public class ForeignKeyAttribute : FieldAttribute
    {
        public Type ForeignType { get; set; }

        public ForeignKeyAttribute(Type foreignType)
        {
            ForeignType = foreignType;
            IsForeignKey = true;
        }
    }
}