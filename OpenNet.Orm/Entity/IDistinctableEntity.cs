namespace OpenNet.Orm.Entity
{
    /// <summary>
    /// Expose one property used to distinct same entity type
    /// </summary>
    public interface IDistinctableEntity
    {
        /// <summary>
        /// Get unique object identifier
        /// </summary>
        long Id { get; set; }
    }
}