using System;
using OpenNet.Orm.Entity.Constraints;
using OpenNet.Orm.Entity.Fields;
using OpenNet.Orm.Entity.References;
using OpenNet.Orm.Entity.Serializers;

namespace OpenNet.Orm.Interfaces
{
    public interface IEntityInfo
    {
        /// <summary>
        /// Get all entites info
        /// </summary>
        EntityInfoCollection Entities { get; }

        /// <summary>
        ///  Get typeof entity
        /// </summary>
        Type EntityType { get; }

        /// <summary>
        /// Get entity primary key
        /// </summary>
        PrimaryKey PrimaryKey { get; }

        /// <summary>
        /// Get entity foreign keys
        /// </summary>
        DistinctCollection<ForeignKey> ForeignKeys { get; }

        /// <summary>
        /// Get entity fields (Including PK and FK)
        /// </summary>
        DistinctCollection<Field> Fields { get; }

        /// <summary>
        /// Get references attributes
        /// </summary>
        DistinctCollection<Reference> References { get; }

        /// <summary>
        /// Get Indexes existing on current entity
        /// </summary>
        DistinctCollection<Index> Indexes { get; }

        /// <summary>
        /// Return factory use to create specific fields db engine
        /// </summary>
        IFieldPropertyFactory FieldPropertyFactory { get; }

        /// <summary>
        /// Get entity name in store
        /// </summary>
        string GetNameInStore();

        /// <summary>
        /// Return serializer to use to convert back entity from db
        /// </summary>
        /// <returns>Entity serializer</returns>
        IEntitySerializer GetSerializer();

        /// <summary>
        /// Get reference attribute associated to specified object type
        /// </summary>
        /// <param name="refType">Type of reference</param>
        /// <returns>Reference attribute found, else null</returns>
        Reference GetReference(Type refType);

        /// <summary>
        /// Looking for specified attribute on entity class
        /// </summary>
        /// <typeparam name="T">Type of attribute searched.</typeparam>
        /// <returns>Attribute found or null.</returns>
        T GetAttribute<T>();

        /// <summary>
        /// Create a new instance of entity
        /// </summary>
        /// <returns>New entity</returns>
        object CreateNewInstance();
    }
}
