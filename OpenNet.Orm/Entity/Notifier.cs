using System;
using System.Collections.Generic;
using System.Linq;

namespace OpenNet.Orm.Entity
{
    public class Notifier : IEntityObserver, IObservableEntity
    {
        private readonly List<IEntityObserver> _observerList = new List<IEntityObserver>();

        /// <summary>
        /// Add new observer
        /// </summary>
        /// <param name="entity">Observer instance to add</param>
        public void Subscribe(IEntityObserver entity)
        {
            if (_observerList.Contains(entity))
                return;

            _observerList.Add(entity);
        }

        /// <summary>
        /// Remove specified observer
        /// </summary>
        /// <param name="entity">Observer instance to remove</param>
        public void Unsubscribe(IEntityObserver entity)
        {
            _observerList.Remove(entity);
        }

        /// <summary>
        /// Remove all observer
        /// </summary>
        public void UnsubscribeAll()
        {
            _observerList.Clear();
        }

        /// <summary>
        /// Called to indicate event occurs on entity
        /// </summary>
        /// <param name="eventType">Type of change occurs</param>
        /// <param name="args"></param>
        public void Notify(Enum eventType, object args)
        {
            var copy = _observerList.ToList();
            foreach (var observer in copy)
            {
                observer.Notify(eventType, args);
            }
        }
    }
}