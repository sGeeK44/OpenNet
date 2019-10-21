using System;
// ReSharper disable MergeConditionalExpression

namespace OpenNet.Orm.Attributes
{
    public class ReferenceAttribute : EntityFieldAttribute, IEquatable<ReferenceAttribute>, IDistinctable
    {
        public string Key { get { return string.Format("{0}{1}{2}", LocalReferenceField, ReferenceEntityType.Name, ForeignReferenceField); } }

        /// <summary>
        /// The type of the referenced Entity
        /// </summary>
        public Type ReferenceEntityType { get; private set; }

        /// <summary>
        /// The name of the key Field in the referenced Entity (typically the Primary Key for ManyToOne relation)
        /// </summary>
        public string ForeignReferenceField { get; private set; }

        /// <summary>
        /// The name of the key Field in current Entity (typically the Foreign Key for ManyToOne relation)
        /// </summary>
        public string LocalReferenceField { get; private set; }


        public ReferenceAttribute(Type referenceEntityType, string foreignReferenceField, string localReferenceField)
        {
            ReferenceEntityType = referenceEntityType;
            LocalReferenceField = localReferenceField;
            ForeignReferenceField = foreignReferenceField;
        }

        public bool Equals(ReferenceAttribute other)
        {
            if (other == null
             || ReferenceEntityType != other.ReferenceEntityType)
                return false;

            return string.Compare(ForeignReferenceField, other.ForeignReferenceField, StringComparison.InvariantCultureIgnoreCase) == 0;
        }
    }
}
