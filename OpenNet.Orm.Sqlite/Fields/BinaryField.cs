using OpenNet.Orm.Entity.Fields;

namespace OpenNet.Orm.Sqlite.Fields
{
    public class BinaryField : FieldProperties
    {
        public override string GetDataTypeDefinition()
        {
            return "BLOB";
        }
    }
}