using System;

namespace OpenNet.Orm.Attributes
{
    [AttributeUsage(AttributeTargets.Class)]
    public class EntityAttribute : Attribute
    {
        private string _nameInStore;

        /// <summary>
        /// Override default entity name in store
        /// </summary>
        public string NameInStore { get; set; }

        /// <summary>
        /// Override default entity serializer
        /// </summary>
        public Type Serializer { get; set; }

        /// <summary>
        /// Get name in store for specified entity type.
        /// </summary>
        /// <param name="entityType">Type of entity</param>
        /// <returns>Name in store</returns>
        public virtual string GetNameInStore(Type entityType)
        {
            if (_nameInStore != null)
                return _nameInStore;

            _nameInStore = NameInStore ?? entityType.Name;
            return _nameInStore;
        }
    }
}
