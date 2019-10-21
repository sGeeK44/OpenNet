using OpenNet.Orm.Entity.Fields;

namespace OpenNet.Orm.Entity.Constraints
{
    public class CustomIndex : Index
    {
        public CustomIndex(string name, string entityName, Field field)
            : base(name, entityName, field) { }

        protected override string GetVariablePartName()
        {
            return Name;
        }
    }
}