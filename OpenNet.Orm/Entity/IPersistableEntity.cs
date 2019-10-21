namespace OpenNet.Orm.Entity
{
    /// <summary>
    /// Expose method to persist entity in repository
    /// </summary>
    public interface IPersistableEntity
    {
        /// <summary>
        /// Save current entity in right repository
        /// </summary>
        void Save();

        /// <summary>
        /// Delete current entity in right repository
        /// </summary>
        void Delete();

        /// <summary>
        /// Refresh current instance with db data
        /// </summary>
        void RefreshFromDb();
    }
}