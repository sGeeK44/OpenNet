using System;

namespace OpenNet.Orm.Entity
{
    public interface IEntityObserver
    {
        /// <summary>
        /// Called to indicate event occurs on entity
        /// </summary>
        /// <param name="eventType">Type of change occurs</param>
        /// <param name="args">Args send by notifier</param>
        void Notify(Enum eventType, object args);
    }
}