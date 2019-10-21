using System;

namespace OpenNet.Orm.Entity
{
    /// <summary>
    /// Used this class to delay object reference loading
    /// </summary>
    /// <typeparam name="T">Type of object to load</typeparam>
    internal class Lazy<T> where T : class
    {
        private readonly object _locker = new object();
        private readonly Func<T> _valueFactory;
        private bool _loaded;
        private T _instance;
        
        /// <summary>
        /// Initialize new instance with specified instance.
        /// </summary>
        public Lazy(T value)
        {
            _instance = value;
            _loaded = true;
        }

        /// <summary>
        /// Initialize new instance. When lazy load occurs, specified value factory is used.
        /// </summary>
        /// <param name="valueFactory">Delegate call to create value on lazy load.</param>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="valueFactory" /> est null.</exception>
        public Lazy(Func<T> valueFactory)
        {
            if (valueFactory == null)
                throw new ArgumentNullException("valueFactory");

            _valueFactory = valueFactory;
        }

        /// <summary>
        /// Get value lazy loaded.
        /// </summary>
        public T Value
        {
            get
            {
                lock (_locker)
                {
                    if (_loaded)
                        return _instance;

                    _instance = _valueFactory();
                    _loaded = true;
                    return _instance;
                }
            }
        }
    }
}