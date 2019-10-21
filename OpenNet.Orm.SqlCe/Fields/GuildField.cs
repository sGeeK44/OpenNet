using OpenNet.Orm.Entity.Fields;

namespace OpenNet.Orm.SqlCe.Fields
{
    public class GuildField : FieldProperties
    {
        public override string GetDataTypeDefinition()
        {
            return "uniqueidentifier";
        }

        public override string GetIdentity()
        {
            return "ROWGUIDCOL ";
        }
    }
}