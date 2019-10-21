using System;
using System.Reflection;
using OpenNet.Orm.Attributes;
using OpenNet.Orm.Entity.Fields;
using OpenNet.Orm.Interfaces;

namespace OpenNet.Orm.Entity.Constraints
{
    public class ForeignKey : Field
    {
        private readonly Lazy<string> _constraintName;

        public ForeignKey(EntityInfoCollection entities, IEntityInfo entityLocal, PropertyInfo prop,
            ForeignKeyAttribute foreignKeyAttribute)
            : base(entityLocal, prop, foreignKeyAttribute)
        {
            _constraintName = new Lazy<string>(ComputeConstraintName);
            Entities = entities;
            ForeignType = foreignKeyAttribute.ForeignType;
        }

        private EntityInfoCollection Entities { get; set; }

        public string ConstraintName
        {
            get { return _constraintName.Value; }
        }

        public string GetCreateSqlQuery()
        {
            return string.Format("ALTER TABLE [{0}] ADD CONSTRAINT {1} {2};",
                Entity.GetNameInStore(), ConstraintName, GetTableCreateSqlQuery());
        }

        public string GetTableCreateSqlQuery()
        {
            return string.Format("FOREIGN KEY ({0}) REFERENCES {1}({2})",
                FieldName, ForeignEntityInfo.GetNameInStore(), ForeignEntityInfo.PrimaryKey.FieldName);
        }

        public Type ForeignType { get; set; }

        public IEntityInfo ForeignEntityInfo { get { return Entities[ForeignType]; } }

        public override bool IsForeignKey
        {
            get { return true; }
        }

        public static ForeignKey Create(EntityInfoCollection entities, IEntityInfo entityLocal, PropertyInfo prop,
            ForeignKeyAttribute foreignKeyAttribute)
        {
            return new ForeignKey(entities, entityLocal, prop, foreignKeyAttribute);
        }

        private string ComputeConstraintName()
        {
            var entityForeignName = Entities.GetNameForType(ForeignType);
            return string.Format("ORM_FK_{0}_{1}", Entity.GetNameInStore(), entityForeignName);
        }
    }
}
