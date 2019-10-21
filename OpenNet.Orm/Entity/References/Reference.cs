using System;
using System.Collections;
using System.Reflection;
using OpenNet.Orm.Attributes;
using OpenNet.Orm.Entity.Fields;
using OpenNet.Orm.Entity.Serializers;
using OpenNet.Orm.Interfaces;

namespace OpenNet.Orm.Entity.References
{
    public class Reference : Field
    {
        private readonly ReferenceType _type;

        private Reference(IEntityInfo entityInfo, PropertyInfo prop, ReferenceAttribute referenceAttribute)
            : base(entityInfo, prop, null)
        {
            _type = typeof(IList).IsAssignableFrom(prop.PropertyType)
                ? ReferenceType.OneToMany
                : ReferenceType.ManyToOne;
            ReferenceEntityType = referenceAttribute.ReferenceEntityType;
            LocalReferenceField = referenceAttribute.LocalReferenceField;
            ForeignReferenceField = referenceAttribute.ForeignReferenceField;
        }

        public override string Key
        {
            get { return string.Format("{0}{1}", LocalReferenceField, ReferenceEntityType.Name); }
        }

        public IEntityInfo EntityReference { get; set; }

        /// <summary>
        /// The type of the referenced Entity
        /// </summary>
        public Type ReferenceEntityType { get; private set; }

        /// <summary>
        /// The name of the key Field in current Entity (typically the Foreign Key for ManyToOne relation)
        /// </summary>
        public string LocalReferenceField { get; private set; }

        /// <summary>
        /// The name of the key Field in the referenced Entity (typically the Primary Key for ManyToOne relation)
        /// </summary>
        public string ForeignReferenceField { get; set; }

        public ReferenceType ReferenceType
        {
            get { return _type; }
        }

        public IEntitySerializer GetReferenceSerializer()
        {
            return EntityReference == null
                ? null
                : EntityReference.GetSerializer();
        }

        public IList CreateValue()
        {
            return (IList) Activator.CreateInstance(PropertyInfo.PropertyType);
        }

        public static Reference Create(EntityInfo entityInfo, PropertyInfo prop, ReferenceAttribute referenceAttribute)
        {
            return new Reference(entityInfo, prop, referenceAttribute);
        }
    }
}