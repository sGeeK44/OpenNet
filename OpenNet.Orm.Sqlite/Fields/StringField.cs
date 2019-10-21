using OpenNet.Orm.Entity.Fields;

namespace OpenNet.Orm.Sqlite.Fields
{
    public class StringField : FieldProperties
    {
        public override string GetDataTypeDefinition()
        {
            return "TEXT";
        }
    }
}