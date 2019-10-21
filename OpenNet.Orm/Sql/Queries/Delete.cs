using OpenNet.Orm.Queries;

namespace OpenNet.Orm.Sql.Queries
{
    public class Delete : ISelectable
    {
        public string SelectStatement()
        {
            return "DELETE";
        }
    }
}