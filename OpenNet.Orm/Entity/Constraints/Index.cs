using System.Text;
using OpenNet.Orm.Constants;
using OpenNet.Orm.Entity.Fields;

namespace OpenNet.Orm.Entity.Constraints
{
    public abstract class Index : IDistinctable
    {
        protected Index(string name, string entityName, Field field)
        {
            Name = name;
            EntityName = entityName;
            Fields = new DistinctCollection<Field> { field };
        }

        protected string Name { get; private set; }
        public bool IsUnique { get; set; }
        protected FieldSearchOrder SearchOrder { get; set; }
        private string EntityName { get; set; }
        protected DistinctCollection<Field> Fields { get; set; }

        /// <summary>
        /// A unique string key to identify an object in collection
        /// </summary>
        public string Key { get { return Name; } }

        public void AddField(Field field)
        {
            Fields.Add(field);
        }

        public string GetNameInStore()
        {
            return string.Format("ORM_IDX_{0}_{1}_{2}", EntityName, GetVariablePartName(), GetSearchOrder());
        }

        public string GetCreateSqlQuery()
        {
            return string.Format("CREATE {0}INDEX {1} ON [{2}] ({3} {4})",
                        IsUnique ? "UNIQUE " : string.Empty,
                        GetNameInStore(),
                        EntityName,
                        GetFieldInvolves(),
                        GetSearchOrder());
        }

        private string GetFieldInvolves()
        {
            StringBuilder result = null;
            foreach (var field in Fields)
            {
                if (result != null)
                    result.AppendFormat(", ");
                else
                    result = new StringBuilder();

                result.AppendFormat("[{0}]", field.FieldName);
            }

            return result == null ? string.Empty : result.ToString();
        }

        protected abstract string GetVariablePartName();

        protected virtual string GetSearchOrder() { return "ASC"; }

        public static Index CreateStandard(string entityName, Field field)
        {
            return new FieldIndex(field.FieldName, entityName, field);
        }

        public static Index CreateCustom(string name, string entityName, Field field)
        {
            return new CustomIndex(name, entityName, field);
        }
    }
}