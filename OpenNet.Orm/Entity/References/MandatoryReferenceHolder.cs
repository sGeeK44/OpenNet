using OpenNet.Orm.Repositories;

namespace OpenNet.Orm.Entity.References
{
    /// <summary>
    /// Wrap behaviour Id and entity object for a reference
    /// </summary>
    /// <typeparam name="TReference"></typeparam>
    public class MandatoryReferenceHolder<TReference> : ReferenceHolder<TReference> where TReference : class, IDistinctableEntity
    {
        private long _id = EntityBase<TReference>.NullId;
        private TReference _reference;

        /// <summary>
        /// Create a new instance of Reference holder
        /// </summary>
        /// <param name="repository">Repository used when need to get object instance by id</param>
        public MandatoryReferenceHolder(IRepository<TReference> repository)
            : base(repository)
        {
            Id = EntityBase<TReference>.NullId;
        }

        /// <summary>
        /// Get or set new reference Id
        /// </summary>
        public long Id
        {
            get
            {
                if (_reference != null)
                    return _reference.Id;

                return _id;
            }
            set
            {
                _id = value;
                _reference = null;
                SetObject(_id);
            }
        }

        protected override void SetId(TReference value)
        {
            _reference = value;
            _id = value != null ? value.Id : EntityBase<TReference>.NullId;
        }
    }
}