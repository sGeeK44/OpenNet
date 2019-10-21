using OpenNet.Orm.Repositories;

// ReSharper disable MergeConditionalExpression

namespace OpenNet.Orm.Entity.References
{
    /// <summary>
    /// Wrap behaviour Id and entity object for a reference
    /// </summary>
    /// <typeparam name="TReference"></typeparam>
    public abstract class ReferenceHolder<TReference> where TReference : class, IDistinctableEntity
    {
        private readonly IRepository<TReference> _repository;
        private Lazy<TReference> _object;

        /// <summary>
        /// Create a new instance of Reference holder
        /// </summary>
        /// <param name="repository">Repository used when need to get object instance by id</param>
        protected ReferenceHolder(IRepository<TReference> repository)
        {
            _repository = repository;
            SetObjectNull();
        }

        /// <summary>
        /// Get or set new reference object
        /// </summary>
        public TReference Object
        {
            get { return _object.Value; }
            set
            {
                _object = new Lazy<TReference>(value);
                SetId(value);
            }
        }

        protected abstract void SetId(TReference value);

        protected void SetObject(long? id)
        {
            if (id == null)
            {
                SetObjectNull();
                return;
            }

            SetObject(id.Value);
        }

        private void SetObjectNull()
        {
            _object = new Lazy<TReference>((TReference) null);
        }

        protected void SetObject(long id)
        {
            _object = new Lazy<TReference>(() => _repository != null ? _repository.GetById(id) : null);
        }
    }
}
