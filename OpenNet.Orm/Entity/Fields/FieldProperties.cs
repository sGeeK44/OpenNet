using System;
using System.Text;
using OpenNet.Orm.Constants;

namespace OpenNet.Orm.Entity.Fields
{
    public abstract class FieldProperties
    {
        private const string DefaultDateGenerator = "GETDATE()";

        /// <summary>
        /// Get type of field property
        /// </summary>
        public Type PropertyType { get; set; }

        /// <summary>
        /// Add default value for null value if specified.
        /// </summary>
        public object DefaultValue { get; set; }

        /// <summary>
        /// Add not Null constraint on field in database if value equal false.
        /// Default value is true.
        /// </summary>
        public bool AllowsNulls { get; set; }

        /// <summary>
        /// Indicate if property can be setted or managed by sgbd
        /// </summary>
        public virtual bool IsSettable { get { return true; } }

        /// <summary>
        /// Return string that repesent sql type definition
        /// </summary>
        /// <returns>Sql type definition</returns>
        public string GetDefinition()
        {
            return GetDataTypeDefinition();
        }

        public abstract string GetDataTypeDefinition();

        public virtual void GetFieldCreationAttributes(StringBuilder definition)
        {
        }

        public string GetFieldCreationAttributes()
        {
            var sb = new StringBuilder();

            GetFieldCreationAttributes(sb);

            if (DefaultValue != null)
            {
                if (DefaultType.CurrentDateTime.Equals(DefaultValue))
                {
                    sb.AppendFormat(" DEFAULT {0}", DefaultDateGenerator);
                }
                else if (DefaultValue is string)
                {
                    sb.AppendFormat(" DEFAULT '{0}'", DefaultValue);
                }
                else
                {
                    sb.AppendFormat(" DEFAULT {0}", DefaultValue);
                }
            }

            if (!AllowsNulls)
            {
                sb.Append(" NOT NULL");
            }

            return sb.ToString();
        }

        public virtual string GetIdentity()
        {
            throw new DefinitionException(string.Format("Data Type '{0}' cannot be marked as an Identity field", GetType()));
        }

        public virtual object Convert(object value)
        {
            if (!PropertyType.IsNullable())
                return PropertyType.IsEnum ? Enum.ToObject(PropertyType, value) : value;

            var args = PropertyType.GetGenericArguments();
            if (args.Length != 1)
                throw new NotSupportedException(
                    string.Format("Converter doesn't support this type of nullable. Type:{0}", PropertyType));

            return args[0].IsEnum ? Enum.ToObject(args[0], value) : value;
        }

        public virtual object ToSqlValue(object instanceValue, out bool needToUpdateInstance)
        {
            needToUpdateInstance = false;
            if (instanceValue == DBNull.Value && DefaultValue != null)
            {
                instanceValue = DefaultValue;
            }

            return instanceValue ?? DBNull.Value;
        }
    }
}