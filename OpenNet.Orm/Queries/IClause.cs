using System.Collections.Generic;
using System.Data;

namespace OpenNet.Orm.Queries
{
    public interface IClause
    {
        string ToStatement(List<IDataParameter> @params);
    }
}