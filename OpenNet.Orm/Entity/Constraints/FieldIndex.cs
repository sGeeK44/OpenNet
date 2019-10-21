using System.Linq;
using OpenNet.Orm.Constants;
using OpenNet.Orm.Entity.Fields;

namespace OpenNet.Orm.Entity.Constraints
{
    public class FieldIndex : Index
    {
        public FieldIndex(string name, string entityName, Field field)
            : base(name, entityName, field)
        {
            IsUnique = field.RequireUniqueValue;
            SearchOrder = field.SearchOrder != FieldSearchOrder.NotSearchable
                        ? field.SearchOrder
                        : FieldSearchOrder.Ascending;
        }

        protected override string GetVariablePartName()
        {
            return Fields.First().FieldName;
        }

        protected override string GetSearchOrder()
        {
            return SearchOrder == FieldSearchOrder.Descending ? "DESC" : "ASC";
        }
    }
}