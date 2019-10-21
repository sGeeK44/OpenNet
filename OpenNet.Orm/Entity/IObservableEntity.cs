namespace OpenNet.Orm.Entity
{
    public interface IObservableEntity
    {
        /// <summary>
        /// Add new observer
        /// </summary>
        /// <param name="entity">Observer instance to add</param>
        void Subscribe(IEntityObserver entity);


        /// <summary>
        /// Remove specified observer
        /// </summary>
        /// <param name="entity">Observer instance to remove</param>
        void Unsubscribe(IEntityObserver entity);
        
        /// <summary>
        /// Remove all observer
        /// </summary>
        void UnsubscribeAll();
    }
}