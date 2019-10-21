using System;
using System.Linq;
using OpenNet.Orm.Attributes;

namespace OpenNet.Orm.Sync.Entity
{
    public class EntityTombstoneAttribute : EntityAttribute
    {
        public const string DefaultTableSuffixe = "_tombstone";
        private readonly string _suffixe;
        private string _nameInStore;

        public EntityTombstoneAttribute()
            : this(DefaultTableSuffixe) { }
        
        public EntityTombstoneAttribute(string suffixe)
        {
            _suffixe = suffixe;
        }

        public override string GetNameInStore(Type entityType)
        {
            if (_nameInStore != null)
                return _nameInStore;

            var entityTrackedType = entityType.GetGenericArguments().First();

            var attr = entityTrackedType.GetCustomAttributes(typeof(EntityAttribute), true)
                                        .FirstOrDefault() as EntityAttribute;

            if (attr == null)
                throw new ArgumentException(string.Format("Type '{0}' does not have an EntityAttribute", entityType.Name));

            var entityTrackedName = attr.GetNameInStore(entityTrackedType);
            _nameInStore = string.Concat(entityTrackedName, _suffixe);
            return _nameInStore;
        }
    }
}
