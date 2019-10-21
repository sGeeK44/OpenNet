using System;
using System.Reflection;
using OpenNet.Orm.Attributes;
using OpenNet.Orm.Entity.Fields;
using OpenNet.Orm.Interfaces;
using OpenNet.Orm.Sql;

namespace OpenNet.Orm.Entity.Constraints
{
    public class PrimaryKey : Field
    {
        private readonly Lazy<string> _constraintName;

        private PrimaryKey(IEntityInfo entity, PropertyInfo prop, PrimaryKeyAttribute pkAttribute)
            : base(entity, prop, pkAttribute)
        {
            _constraintName = new Lazy<string>(ComputeConstraintName);
            KeyScheme = pkAttribute.KeyScheme;
        }

        public KeyScheme KeyScheme { get; set; }

        public string ConstraintName
        {
            get { return _constraintName.Value; }
        }

        public string GetCreateSqlQuery()
        {
            return string.Format("ALTER TABLE [{0}] ADD CONSTRAINT [{1}] PRIMARY KEY ([{2}]);",
                Entity.GetNameInStore(), ConstraintName, FieldName);
        }

        private string ComputeConstraintName()
        {
            return string.Format("ORM_PK_{0}", Entity.GetNameInStore());
        }

        protected override string GetFieldCreationAttributes()
        {
            var result = base.GetFieldCreationAttributes();
            if (KeyScheme == KeyScheme.Identity)
            {
                return result + FieldProperties.GetIdentity();
            }
            return result;
        }

        public override object ToSqlValue(object item)
        {
            var instanceValue = GetEntityValue(item);
            switch (KeyScheme)
            {
                case KeyScheme.GUID:
                    if (instanceValue.Equals(Guid.Empty))
                    {
                        instanceValue = Guid.NewGuid();
                        SetEntityValue(item, instanceValue);
                    }
                    break;
            }

            return instanceValue;
        }

        public static PrimaryKey Create(IEntityInfo entity, PropertyInfo prop, PrimaryKeyAttribute primaryKeyAttribute)
        {
            return new PrimaryKey(entity, prop, primaryKeyAttribute);
        }
    }
}