using OpenNet.Orm.Entity.Fields;

namespace OpenNet.Orm.SqlCe.Fields
{
    public abstract class IntegerField : FieldProperties
    {
        public override string GetIdentity()
        {
            return " IDENTITY";
        }
    }
}