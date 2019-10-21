using OpenNet.Orm.Filters;
using OpenNet.Orm.Interfaces;

namespace OpenNet.Orm.Sql.Queries
{
    public class LeftJoin : Join
    {
        private const string JoinClause = "LEFT JOIN";

        public LeftJoin(IEntityInfo entityRef, IEntityInfo entityJoin)
            : base(JoinClause, entityRef, entityJoin) { }

        public LeftJoin(IEntityInfo entityRef, IEntityInfo entityJoin, IFilter filter)
            : base(JoinClause, entityRef, entityJoin, filter) { }
    }
}