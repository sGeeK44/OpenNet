namespace OpenNet.Orm.Entity
{
    public enum EntityEvent
    {
        /// <summary>
        /// Indicate entity was persisted in datastore
        /// </summary>
        Saved,

        /// <summary>
        /// Indicate entity was deleted from datastore
        /// </summary>
        Deleted
    }
}