using System;
using System.Collections.Generic;
using System.Threading;
using OpenNet.Orm.Sync.Agents;

namespace OpenNet.Orm.Sync
{
    public class LocalBoundTransport : ISyncTransport
    {
        private Dictionary<Type, object> _objects = new Dictionary<Type, object>();
        private bool _abort;

        public bool IsPairConnected
        {
            get { return true; }
        }

        public long GetLenght<T>(T obj)
        {
            return 0;
        }

        public void Send<T>(T obj)
        {
            Pair._objects.Add(typeof(T), Serialize(obj));
        }

        public T Receive<T>()
        {
            var timeout = 0;
            var type = typeof(T);
            _abort = false;
            while (!_objects.ContainsKey(type))
            {
                if (_abort)
                    return default(T);

                if (timeout >= 10000)
                    throw new TimeoutException();

                Thread.Sleep(50);
                timeout++;
            }
            var result = _objects[type];
            _objects.Remove(type);
            return Deserialize<T>(result);
        }

        public void Initialize() { }

        public void Abort()
        {
            _abort = true;
        }

        public LocalBoundTransport Pair { get; set; }

        protected virtual object Serialize<T>(T item)
        {
            return item;
        }

        protected virtual T Deserialize<T>(object item)
        {
            return (T)item;
        }

        public void AddObserver(SyncStates stateObserve, IOrmSyncObserver observer)
        {

        }
    }
}