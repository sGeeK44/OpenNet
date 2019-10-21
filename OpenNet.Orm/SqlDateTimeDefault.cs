using OpenNet.Orm.Constants;
using OpenNet.Orm.Interfaces;

namespace OpenNet.Orm
{
    public class SqlDateTimeDefault : IDefaultValue
    {
        public DefaultType DefaultType
        {
            get { return DefaultType.CurrentDateTime; }
        }

        public object GetDefaultValue()
        {
            return "GETDATE()";
        }

        public static SqlDateTimeDefault Value
        {
            get { return new SqlDateTimeDefault(); }
        }
    }
}
