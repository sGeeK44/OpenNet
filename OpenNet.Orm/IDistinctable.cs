namespace OpenNet.Orm
{
    public interface IDistinctable
    {
        /// <summary>
        /// A unique string key to identify an object in collection
        /// </summary>
        string Key { get; }
    }
}